using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole
{
    internal class PositionedCell: Cell
    {
        /// <summary>
        /// The desired X location of the cell.
        /// </summary>
        private int x;

        /// <summary>
        /// The desired Y location of the cell.
        /// </summary>
        private int y;

        /// <summary>
        /// The desired X location of the cell.
        /// </summary>
        public virtual int X { get { return x; } set { x = value; ActualX = value; } }

        /// <summary>
        /// The desired Y location of the cell.
        /// </summary>
        public virtual int Y { get { return y; } set { y = value; ActualY = value; } }

        /// <summary>
        /// The X location of the cell.
        /// </summary>
        public virtual int ActualX { get; set; }

        /// <summary>
        /// The Y location of the cell.
        /// </summary>
        public virtual int ActualY { get; set; }

        /// <summary>
        /// Resets the positioned cell with default values;
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            X = Y = 0;
        }

        /// <summary>
        /// Copies this positioned cell's values to another positioned cell.
        /// </summary>
        /// <param name="destination">The cell to write to.</param>
        public void Copy(PositionedCell destination)
        {
            base.Copy(destination);

            destination.X = this.X;
            destination.Y = this.Y;
            destination.ActualX = this.ActualX;
            destination.ActualY = this.ActualY;
        }
    }
}
