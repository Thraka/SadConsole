#if XNA
using Microsoft.Xna.Framework;
#else
using System.Numerics;
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;

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
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y1) using steep.
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function, taking x and y. (if this returns false, the algorithm stops early)</param>
        public static void Line(int x0, int y0, int x1, int y1, Func<int, int, bool> plot)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? plot(y, x) : plot(x, y)))
                {
                    return;
                }

                err = err - dY;
                if (err >= 0)
                {
                    continue;
                }

                y += ystep; err += dX;
            }
        }

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y1) using an interpolation derived algorithm.
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        public static void Line2(int x0, int y0, int x1, int y1, Func<int, int, bool> plot)
        {
            int len = Math.Max(Math.Abs(x1 - x0), Math.Abs(y1 - y0));
            for (int i = 0; i < len; i++)
            {
                float t = (float)i / len;
                double x = Math.Round(x0 * (1.0 - t) + x1 * t);
                double y = Math.Round(y0 * (1.0 - t) + y1 * t);
                plot((int)x, (int)y);
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
            where TNode : class
        {
            var queue = new Queue<TNode>();

            TNode workingNode = node;

            if (!shouldNodeChange(workingNode))
            {
                return;
            }

            queue.Enqueue(workingNode);

            while (true)
            {
                workingNode = queue.Dequeue();

                if (shouldNodeChange(workingNode))
                {
                    changeNode(workingNode);

                    NodeConnections<TNode> connections = getNodeConnections(workingNode);

                    if (connections.West != null)
                    {
                        queue.Enqueue(connections.West);
                    }

                    if (connections.East != null)
                    {
                        queue.Enqueue(connections.East);
                    }

                    if (connections.North != null)
                    {
                        queue.Enqueue(connections.North);
                    }

                    if (connections.South != null)
                    {
                        queue.Enqueue(connections.South);
                    }
                }

                if (queue.Count == 0)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Processes an area and applies a gradient calculation to each part of the area.
        /// </summary>
        /// <param name="cellSize">The size of an individual cell. Makes the angle uniform.</param>
        /// <param name="position">The center of the gradient.</param>
        /// <param name="strength">The width of the gradient spread.</param>
        /// <param name="angle">The angle to apply the gradient.</param>
        /// <param name="area">The area to calculate.</param>
        /// <param name="gradient">The color gradient to fill with.</param>
        /// <param name="applyAction">The callback called for each part of the area.</param>
        public static void GradientFill(Point cellSize, Point position, int strength, int angle, Rectangle area, ColorGradient gradient, Action<int, int, Color> applyAction)
        {
            double radians = angle * Math.PI / 180; // = Math.Atan2(x1 - x2, y1 - y2);

            Vector2 angleVector = new Vector2((float)(Math.Sin(radians) * strength), (float)(Math.Cos(radians) * strength)) / 2;
            var location = new Vector2(position.X, position.Y);

            if (cellSize.X > cellSize.Y)
            {
                angleVector.Y *= cellSize.X / cellSize.Y;
            }
            else if (cellSize.X < cellSize.Y)
            {
                angleVector.X *= cellSize.Y / cellSize.X;
            }

            Vector2 endingPoint = location + angleVector;
            Vector2 startingPoint = location - angleVector;

            double x1 = (startingPoint.X / (double)area.Width) * 2.0f - 1.0f;
            double y1 = (startingPoint.Y / (double)area.Height) * 2.0f - 1.0f;
            double x2 = (endingPoint.X / (double)area.Width) * 2.0f - 1.0f;
            double y2 = (endingPoint.Y / (double)area.Height) * 2.0f - 1.0f;

            double start = x1 * angleVector.X + y1 * angleVector.Y;
            double end = x2 * angleVector.X + y2 * angleVector.Y;

            for (int x = area.Left; x < area.Width; x++)
            {
                for (int y = area.Top; y < area.Height; y++)
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
                    for (counter = 0; counter < gradient.Stops.Length && gradient.Stops[counter].Stop < (float)lerp; counter++)
                    {
                        ;
                    }

                    counter--;
                    counter = (int)MathHelper.Clamp(counter, 0, gradient.Stops.Length - 2);

                    float newLerp = (gradient.Stops[counter].Stop - (float)lerp) / (gradient.Stops[counter].Stop - gradient.Stops[counter + 1].Stop);

                    applyAction(x, y, Color.Lerp(gradient.Stops[counter].Color, gradient.Stops[counter + 1].Color, newLerp));
                }
            }
        }

        /// <summary>
        /// Plots the outside of the circle, passing the x,y to <paramref name="plot"/>.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="plot">A method to call on each x,y coordinate of the outside of the circle.</param>
        public static void Circle(int centerX, int centerY, int radius, Action<int, int> plot)
        {
            int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
            do
            {
                plot(centerX - xi, centerY + yi); /*   I. Quadrant */
                plot(centerX - yi, centerY - xi); /*  II. Quadrant */
                plot(centerX + xi, centerY - yi); /* III. Quadrant */
                plot(centerX + yi, centerY + xi); /*  IV. Quadrant */
                radius = err;
                if (radius <= yi)
                {
                    err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */
                }

                if (radius > xi || err > yi)
                {
                    err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
                }
            } while (xi < 0);
        }

        /// <summary>
        /// Plots the outside of an ellipse, passing the x,y to <paramref name="plot"/>.
        /// </summary>
        /// <param name="x0">The X coordinate of the first corner of the ellipse.</param>
        /// <param name="y0">The Y coordinate of the first corner of the ellipse.</param>
        /// <param name="x1">The X coordinate of the second corner of the ellipse.</param>
        /// <param name="y1">The Y coordinate of the second corner of the ellipse.</param>
        /// <param name="plot">A method to call on each x,y coordinate of the outside of the ellipse.</param>
        public static void Ellipse(int x0, int y0, int x1, int y1, Action<int, int> plot)
        {
            int a = Math.Abs(x1 - x0), b = Math.Abs(y1 - y0), b1 = b & 1; /* values of diameter */
            long dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a; /* error increment */
            long err = dx + dy + b1 * a * a, e2; /* error of 1.step */

            if (x0 > x1) { x0 = x1; x1 += a; } /* if called with swapped points */
            if (y0 > y1)
            {
                y0 = y1; /* .. exchange them */
            }

            y0 += (b + 1) / 2; y1 = y0 - b1;   /* starting pixel */
            a *= 8 * a; b1 = 8 * b * b;

            do
            {
                plot(x1, y0); /*   I. Quadrant */
                plot(x0, y0); /*  II. Quadrant */
                plot(x0, y1); /* III. Quadrant */
                plot(x1, y1); /*  IV. Quadrant */
                e2 = 2 * err;
                if (e2 <= dy) { y0++; y1--; err += dy += a; }  /* y step */
                if (e2 >= dx || 2 * err > dy) { x0++; x1--; err += dx += b1; } /* x step */
            } while (x0 <= x1);

            while (y0 - y1 < b)
            {  /* too early stop of flat ellipses a=1 */
                plot(x0 - 1, y0); /* -> finish tip of ellipse */
                plot(x1 + 1, y0++);
                plot(x0 - 1, y1);
                plot(x1 + 1, y1--);
            }
        }

        /// <summary>
        /// Describes the 4-way connections of a node.
        /// </summary>
        /// <typeparam name="TNode">The type of object the node and its connections are.</typeparam>
        public class NodeConnections<TNode>
            where TNode : class
        {
            public TNode West;
            public TNode East;
            public TNode North;
            public TNode South;

            /// <summary>
            /// Creates a new instance of this object with the specified connections.
            /// </summary>
            /// <param name="west">The west connection.</param>
            /// <param name="east">The east connection.</param>
            /// <param name="north">The north connection.</param>
            /// <param name="south">The south connection.</param>
            public NodeConnections(TNode west, TNode east, TNode north, TNode south)
            {
                West = west;
                East = east;
                North = north;
                South = south;
            }

            /// <summary>
            /// Creates a new instance of this object with all connections set to <see langword="null"/>.
            /// </summary>
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
