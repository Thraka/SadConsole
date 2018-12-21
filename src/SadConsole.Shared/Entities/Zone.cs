using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.DrawCalls;

using SadConsole;

namespace SadConsole.Entities
{
    /// <summary>
    /// Defines an area for a scene.
    /// </summary>
    [DataContract]
    public class Zone: Console
    {
        private const string DefaultDebugTitle = "Zone";

        private string _title = "Zone";
        private Cell _debugAppearance = new Cell(Color.White, Color.Black, 0);
        private DrawCallScreenObject _drawCallScreenObject;
        private DrawCallColoredRect _drawCallZone;
        /// <summary>
        /// The area the zone covers.
        /// </summary>
        [DataMember] public readonly Rectangle Area;

        /// <summary>
        /// A title for the area.
        /// </summary>
        [DataMember] public string DebugTitle
        {
            get => _title;
            set
            {
                _title = value;

                if (string.IsNullOrWhiteSpace(_title))
                    _title = DefaultDebugTitle;

                Rebuild();
            }
        }

        /// <summary>
        /// A visual for the area to help debug.
        /// </summary>
        [DataMember] public Cell DebugAppearance
        {
            get => _debugAppearance;
            set
            {
                _debugAppearance = value;
                Rebuild();
            }
        }

        /// <summary>
        /// Key-value pairs for the zone.
        /// </summary>
        [DataMember] public Dictionary<string, string> Settings = new Dictionary<string, string>();
        
        /// <summary>
        /// Creates a new zone object with the specified area.
        /// </summary>
        /// <param name="area">The area of the zone.</param>
        public Zone(Rectangle area): base(DefaultDebugTitle.Length, 1)
        {
            IsVisible = false;
            UseMouse = false;
            UseKeyboard = false;

            Area = area;
        }
        
        /// <inheritdoc />
        protected override void OnVisibleChanged()
        {
            Rebuild();
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan timeElapsed)
        {
            // TODO verify zone works.
            if (IsVisible && Parent != null)
            {
                if (Parent is IScreenObjectViewPort parent)
                {
                    _drawCallScreenObject.Position = (Area.Location - parent.ViewPort.Location + Parent.CalculatedPosition).ToVector2();
                    _drawCallZone.Rectangle = new Rectangle(Font.GetWorldPosition(Area.Location), Area.Size * Font.Size);

                    if (parent.ViewPort.Intersects(Area))
                    {
                        Global.DrawCalls.Add(_drawCallZone);
                        Global.DrawCalls.Add(_drawCallScreenObject);
                    }

                    //if (_parentSurface.ViewPort.Intersects(Area))
                    //{
                    //    if (_parentSurface.UsePixelPositioning)
                    //    {
                    //        _drawCallScreenObject.Position = (Area.Location - _parentSurface.ViewPort.Location +
                    //                                          _parentSurface.CalculatedPosition).ToVector2();
                    //    }
                    //    else
                    //    {
                    //        _drawCallScreenObject.Position = _debugSurface.Font.GetWorldPosition((Area.Location - _parentSurface.ViewPort.Location +
                    //                                                                              _parentSurface.CalculatedPosition)).ToVector2();
                    //    }

                    //    Global.DrawCalls.Add(_drawCallScreenObject);
                    //}
                }
                else
                {
                    _drawCallScreenObject.Position = (Area.Location + Parent.CalculatedPosition).ToVector2();
                    _drawCallZone.Rectangle = new Rectangle(Font.GetWorldPosition(Area.Location), Area.Size * Font.Size);

                    Global.DrawCalls.Add(_drawCallZone);
                    Global.DrawCalls.Add(_drawCallScreenObject);
                }

            }

            base.Draw(timeElapsed);
        }

        private void Rebuild()
        {
            if (IsVisible)
            {
                Font = Parent?.Font ?? SadConsole.Global.FontDefault;
                DefaultBackground = DebugAppearance.Background;
                DefaultForeground = DebugAppearance.Foreground;
                Resize(_title.Length, 1, true);
                Print(0, 0, _title);
                
                _drawCallScreenObject = new DrawCallScreenObject(this, Point.Zero, UsePixelPositioning);
                _drawCallZone = new DrawCallColoredRect(Area, DebugAppearance.Background);
            }
        }

        /// <inheritdoc />
        protected override void OnParentChanged(Console oldParent, Console newParent)
        {
            Font = newParent?.Font ?? SadConsole.Global.FontDefault;
        }
    }
}
