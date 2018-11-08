using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SadConsole.Maps
{
    /// <summary>
    /// Region of a map.
    /// </summary>
    public class Region
    {
        public bool IsRectangle;
        public Rectangle InnerRect;
        public Rectangle OuterRect;
        public List<Point> InnerPoints = new List<Point>();
        public List<Point> OuterPoints = new List<Point>();
        public bool IsLit = true;
        public bool IsVisited;
        public List<Point> Connections = new List<Point>();
    }
}
