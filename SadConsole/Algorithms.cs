using System;
using System.Collections.Generic;
using System.Numerics;
using SadRogue.Primitives;

namespace SadConsole;
/// <summary>
/// Provides a few minor helper methods related to filling.
/// </summary>
public static class Algorithms
{
    /// <summary>
    /// A very slow 4-way fill algorithm to change items from one type to another.
    /// </summary>
    /// <typeparam name="TNode">The item type that is changed.</typeparam>
    /// <param name="node">The item to change.</param>
    /// <param name="shouldNodeChange">Determines if the node should change.</param>
    /// <param name="changeNode">After it is determined if the node should change, this changes the node.</param>
    /// <param name="getNodeConnections">Gets any other nodes connected to this node.</param>
    public static void FloodFill<TNode>(TNode node, Func<TNode, bool> shouldNodeChange, Action<TNode> changeNode, Func<TNode, NodeConnections<TNode>> getNodeConnections)
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
    public static void GradientFill(Point cellSize, Point position, int strength, int angle, Rectangle area, Gradient gradient, Action<int, int, Color> applyAction)
    {
        double radians = angle * Math.PI / 180; // = Math.Atan2(x1 - x2, y1 - y2);

        Vector2 angleVector = new Vector2((float)(Math.Sin(radians) * strength), (float)(Math.Cos(radians) * strength)) / 2;
        var location = new Vector2(position.X, position.Y);

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

                lerp = MathHelpers.Clamp((float)lerp, 0f, 1.0f);

                int counter;
                for (counter = 0; counter < gradient.Stops.Length && gradient.Stops[counter].Stop < (float)lerp; counter++)
                {
                    ;
                }

                counter--;
                counter = (int)MathHelpers.Clamp(counter, 0, gradient.Stops.Length - 2);

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
    {
        /// <summary>
        /// The west or left node.
        /// </summary>
        public TNode? West;

        /// <summary>
        /// The east or right node.
        /// </summary>
        public TNode? East;

        /// <summary>
        /// The north or up node.
        /// </summary>
        public TNode? North;

        /// <summary>
        /// The south or down node.
        /// </summary>
        public TNode? South;

        /// <summary>
        /// When <see langword="true"/> indicates the <see cref="West"/> connection is valid; otherwise <see langword="false"/>.
        /// </summary>
        public bool HasWest;

        /// <summary>
        /// When <see langword="true"/> indicates the <see cref="East"/> connection is valid; otherwise <see langword="false"/>.
        /// </summary>
        public bool HasEast;

        /// <summary>
        /// When <see langword="true"/> indicates the <see cref="North"/> connection is valid; otherwise <see langword="false"/>.
        /// </summary>
        public bool HasNorth;

        /// <summary>
        /// When <see langword="true"/> indicates the <see cref="South"/> connection is valid; otherwise <see langword="false"/>.
        /// </summary>
        public bool HasSouth;

        /// <summary>
        /// Creates a new instance of this object with the specified connections.
        /// </summary>
        /// <param name="west">The west connection.</param>
        /// <param name="east">The east connection.</param>
        /// <param name="north">The north connection.</param>
        /// <param name="south">The south connection.</param>
        /// <param name="isWest">When <see langword="true"/> indicates the <see cref="West"/> connection is valid; otherwise <see langword="false"/>.</param>
        /// <param name="isEast">When <see langword="true"/> indicates the <see cref="East"/> connection is valid; otherwise <see langword="false"/>.</param>
        /// <param name="isNorth">When <see langword="true"/> indicates the <see cref="North"/> connection is valid; otherwise <see langword="false"/>.</param>
        /// <param name="isSouth">When <see langword="true"/> indicates the <see cref="South"/> connection is valid; otherwise <see langword="false"/>.</param>
        public NodeConnections(TNode west, TNode east, TNode north, TNode south, bool isWest, bool isEast, bool isNorth, bool isSouth)
        {
            West = west;
            East = east;
            North = north;
            South = south;

            (HasWest, HasEast, HasNorth, HasSouth) = (isWest, isEast, isNorth, isSouth);
        }

        /// <summary>
        /// Creates a new instance of this object with all connections set to <see langword="null"/>.
        /// </summary>
        public NodeConnections()
        {
        }
    }
}
