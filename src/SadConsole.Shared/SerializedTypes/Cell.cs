using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class CellSerialized
    {
        [DataMember]
        public CellDecorator[] Decorators;

        [DataMember]
        public ColorSerialized Foreground;

        [DataMember]
        public ColorSerialized Background;

        [DataMember]
        public int Glyph;

        [DataMember]
        public Microsoft.Xna.Framework.Graphics.SpriteEffects Mirror;

        [DataMember]
        public bool IsVisible;

        [DataMember]
        public CellStateSerialized CellState;
        
        public static implicit operator CellSerialized(Cell cell)
        {
            return new CellSerialized()
            {
                Foreground = cell.Foreground,
                Background = cell.Background,
                Glyph = cell.Glyph,
                IsVisible = cell.IsVisible,
                Mirror = cell.Mirror,
                Decorators = cell.Decorators.ToArray(),
                CellState = cell.State
            };
        }

        public static implicit operator Cell(CellSerialized cell)
        {
            var newCell = new Cell(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror)
            {
                IsVisible = cell.IsVisible,
                Decorators = cell.Decorators != null ? cell.Decorators.ToArray() : new CellDecorator[0]
            };

            if (cell.CellState != null)
                newCell.State = cell.CellState;

            return newCell;
        }

    }

    [DataContract]
    public class CellStateSerialized
    {
        [DataMember]
        public CellDecorator[] Decorators;

        [DataMember]
        public ColorSerialized Foreground;

        [DataMember]
        public ColorSerialized Background;

        [DataMember]
        public int Glyph;

        [DataMember]
        public Microsoft.Xna.Framework.Graphics.SpriteEffects Mirror;

        [DataMember]
        public bool IsVisible = true;

        public static implicit operator CellStateSerialized(CellState cell)
        {
            return new CellStateSerialized()
            {
                Foreground = cell.Foreground,
                Background = cell.Background,
                Glyph = cell.Glyph,
                IsVisible = cell.IsVisible,
                Mirror = cell.Mirror,
                Decorators = cell.Decorators.ToArray()
            };
        }

        public static implicit operator CellState(CellStateSerialized cell)
        {
            return new CellState(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror, cell.IsVisible, cell.Decorators);
        }

    }
}
