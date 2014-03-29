using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole
{
    /// <summary>
    /// The Bresenham algorithm collection
    /// </summary>
    public static class Algorithms
    {
        /// <summary>
        /// Swaps two references.
        /// </summary>
        /// <typeparam name="T">The type being swapped.</typeparam>
        /// <param name="lhs">Left value.</param>
        /// <param name="rhs">Right value.</param>
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y10
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        public static void Line(int x0, int y0, int x1, int y1, Func<int, int, bool> plot)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? plot(y, x) : plot(x, y))) return;
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }
        }

        /// <summary>
        /// Uses a 4-way fill algorithm to change items from one type to another.
        /// </summary>
        /// <typeparam name="TNode">The item type that is changed.</typeparam>
        /// <param name="node">The item to change.</param>
        /// <param name="shouldNodeChange">Determines if the node should change.</param>
        /// <param name="changeNode">After it is determined if the node should change, this changes the node.</param>
        /// <param name="getNodeConnections">Gets any other nodes connected to this node.</param>
        public static void FloodFill<TNode>(TNode node, Func<TNode, bool> shouldNodeChange, Action<TNode> changeNode, Func<TNode, NodeConnections<TNode>> getNodeConnections)
            where TNode: class
        {
            Queue<TNode> queue = new Queue<TNode>();

            TNode workingNode = node;

            if (shouldNodeChange(workingNode))
            {
                queue.Enqueue(workingNode);

                while (true)
                {
                    workingNode = queue.Dequeue();

                    if (shouldNodeChange(workingNode))
                    {
                        changeNode(workingNode);

                        var connections = getNodeConnections(workingNode);

                        if (connections.West != null)
                            queue.Enqueue(connections.West);

                        if (connections.East != null)
                            queue.Enqueue(connections.East);

                        if (connections.North != null)
                            queue.Enqueue(connections.North);

                        if (connections.South != null)
                            queue.Enqueue(connections.South);
                    }

                    if (queue.Count == 0)
                        break;
                }
            }
        }

        /// <summary>
        /// Processes an area and applies a gradient calculation to each part of the area.
        /// </summary>
        /// <param name="position">The center of the gradient.</param>
        /// <param name="strength">The width of the gradient spread.</param>
        /// <param name="angle">The angle to apply the gradient.</param>
        /// <param name="area">The area to calculate.</param>
        /// <param name="applyAction">The callback called for each part of the area.</param>
        public static void GradientFill(Point cellSize, Point position, int strength, int angle, Rectangle area, ColorGradient gradient, Action<int, int, Color> applyAction)
        {
            double radians = angle * Math.PI / 180; // = Math.Atan2(x1 - x2, y1 - y2);
            
            Vector2 angleVector = new Vector2((float)(Math.Sin(radians) * strength), (float)(Math.Cos(radians) * strength)) / 2;
            Vector2 location = new Vector2(position.X, position.Y);

            if (cellSize.X > cellSize.Y)
                angleVector.Y *= cellSize.X / cellSize.Y;

            else if (cellSize.X < cellSize.Y)
                angleVector.X *= cellSize.Y / cellSize.X;

            Vector2 endingPoint = location + angleVector;
            Vector2 startingPoint = location - angleVector;

            double x1 = (startingPoint.X / (double)area.Width) * 2.0f - 1.0f;
            double y1 = (startingPoint.Y / (double)area.Height) * 2.0f - 1.0f;
            double x2 = (endingPoint.X / (double)area.Width) * 2.0f - 1.0f;
            double y2 = (endingPoint.Y / (double)area.Height) * 2.0f - 1.0f;

            double start = x1 * angleVector.X + y1 * angleVector.Y;
            double end = x2 * angleVector.X + y2 * angleVector.Y;

            for (int x = area.X; x < area.Width; x++)
            {
                for (int y = area.Y; y < area.Height; y++)
                {
                    // but we need vectors from (-1, -1) to (1, 1)
                    // instead of pixels from (0, 0) to (width, height)
                    double u = (x / (double)area.Width) * 2.0f - 1.0f;
                    double v = (y / (double)area.Height) * 2.0f - 1.0f;

                    double here = u * angleVector.X + v * angleVector.Y;

                    double lerp = (start - here) / (start - end);

                    //lerp = Math.Abs((lerp - (int)lerp));

                    lerp = MathHelper.Clamp((float)lerp, 0f, 1.0f);

                    int counter;
                    for (counter = 0; counter < gradient.Stops.Length && gradient.Stops[counter].Stop < (float)lerp; counter++) ;

                    counter--;
                    counter = (int)MathHelper.Clamp(counter, 0, gradient.Stops.Length - 2);

                    float newLerp = (gradient.Stops[counter].Stop - (float)lerp) / (gradient.Stops[counter].Stop - gradient.Stops[counter + 1].Stop);

                    applyAction(x, y, Color.Lerp(gradient.Stops[counter].Color, gradient.Stops[counter + 1].Color, newLerp));
                }
            }
        }


        /// <summary>
        /// Describes the 4-way connections of a node.
        /// </summary>
        /// <typeparam name="TNode">The type of object the node and its connections are.</typeparam>
        public class NodeConnections<TNode>
            where TNode: class
        {
            public TNode West;
            public TNode East;
            public TNode North;
            public TNode South;

            public NodeConnections(TNode west, TNode east, TNode north, TNode south)
            {
                West = west;
                East = east;
                North = north;
                South = south;
            }

            public NodeConnections()
            {
                West = null;
                East = null;
                North = null;
                South = null;
            }
        }
    }
}
