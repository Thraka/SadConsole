using SadRogue.Primitives;
using SadConsole;
using SadConsole.Input;
using System;
using SadConsoleEditor.Panels;
using SadConsole.Entities;
using Console = SadConsole.Console;

namespace SadConsoleEditor
{
    public class ResizableObject<TData>: ResizableObject
    {
        public TData Data;

        public ResizableObject(ObjectType objectType, Entity Entity, TData data): base(objectType, Entity)
        {
            Data = data;
        }
    }

    public class ResizableObject
    {
        private Entity overlay;
        private Entity _entity;
        private ObjectType objectType;
        private Point renderOffset;

        private bool isSelected;

        public ResizeRules Rules;

        //private static SadConsole.Renderers.SadConsole.Renderers.Basic tempRenderer = new SadConsole.Renderers.SadConsole.Renderers.Basic();

        public Entity Entity => _entity;

        public Entity Overlay => overlay;

        public ObjectType Type => objectType;

        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; ProcessOverlay(); }
        }

        public ResizableObject(ObjectType objectType, Entity Entity)
        {
            this.objectType = objectType;
            _entity = Entity;
            
            renderOffset = Entity.PositionOffset;

            Rules = ResizeRules.Get(objectType);
            ProcessOverlay();
        }

        public Point RenderOffset
        {
            get => renderOffset;
            set { renderOffset = value; ProcessOverlay(); }
        }

        public string Name
        {
            get => Entity.Name;
            set { Entity.Name = value; if (objectType == ObjectType.Zone) DrawZone(); }
        }
        

        public void ResizeObject(int width, int height, int? positionX = null, int? positionY = null)
        {
            if (objectType == ObjectType.Zone)
            {
                if (Entity.Animation.Width != width || Entity.Animation.Height != height)
                {
                    var animation = Entity.Animation;
                    var backColor = animation.CurrentFrame[0].Background;

                    var newAnimation = new AnimatedScreenSurface(Entity.Animation.Name, width, height);
                    var frame = newAnimation.CreateFrame();
                    frame.Fill(Color.White, backColor, 0);
                    frame.Print(0, 0, Name, Color.DarkGray);

                    Entity.Animation = newAnimation;
                    Entity.Update();

                    Entity.Position = new Point(positionX ?? Entity.Position.X, positionY ?? Entity.Position.Y);

                    ProcessOverlay();
                }
            }
        }

        public virtual void Recolor(Color color)
        {
            Entity.Animation.CurrentFrame.Fill(Color.White, color, 0);
            Entity.Animation.CurrentFrame.Print(0, 0, Name, Color.DarkGray);
        }

        public void DrawZone()
        {
            Entity.Animation.CurrentFrame.Fill(Color.White, Entity.Animation.CurrentFrame[0].Background, 0);
            Entity.Animation.CurrentFrame.Print(0, 0, Name, Color.DarkGray);
        }

        public void Draw()
        {
            // This steals the code from the renderer. This DRAW method is called in the middle of
            // an existing draw call chain, so the existing state of the Global.SpriteBatch is reused.
            Entity.PositionOffset = RenderOffset;
            Entity.Draw();
            //tempRenderer.RenderCells(Entity.Animation, true);

            if (isSelected)
            {
                overlay.PositionOffset = Entity.PositionOffset = renderOffset;
                overlay.Position = Entity.Position - Entity.Animation.Center - new Point(1, 1);
                //tempRenderer.RenderCells(overlay.Animation, true);
                overlay.Draw();
            }

        }

        public void ProcessOverlay()
        {
            if (overlay == null)
                overlay = new Entity(1, 1, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);

            overlay.PositionOffset = Entity.PositionOffset = renderOffset;
            overlay.Position = Entity.Position - Entity.Animation.Center - new Point(1, 1);

            if (overlay.Animation.Width != Entity.Animation.Width + 2 || overlay.Animation.Height != Entity.Animation.Height + 2)
            {
                overlay.Animation = new AnimatedScreenSurface("default", Entity.Animation.Width + 2, Entity.Animation.Height + 2, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
                overlay.Animation.CreateFrame();
            }

            overlay.Animation.CurrentFrame.DrawBox(new Rectangle(0, 0, overlay.Animation.Width, overlay.Animation.Height), new ColoredGlyph(overlay.Animation.CurrentFrame.DefaultForeground, overlay.Animation.CurrentFrame.DefaultBackground, 177));

            var centers = new Point(overlay.Animation.Width / 2, overlay.Animation.Height / 2);

            if (Rules.AllowLeftRight && Rules.AllowTopBottom)
            {
                overlay.Animation.CurrentFrame.SetGlyph(0, 0, 254);
                overlay.Animation.CurrentFrame.SetGlyph(overlay.Animation.Width - 1, 0, 254);
                overlay.Animation.CurrentFrame.SetGlyph(overlay.Animation.Width - 1, overlay.Animation.Height - 1, 254);
                overlay.Animation.CurrentFrame.SetGlyph(0, overlay.Animation.Height - 1, 254);
            }

            if (Rules.AllowLeftRight)
            {
                overlay.Animation.CurrentFrame.SetGlyph(0, centers.Y, 254);
                overlay.Animation.CurrentFrame.SetGlyph(overlay.Animation.Width - 1, centers.Y, 254);
            }

            if (Rules.AllowTopBottom)
            {
                overlay.Animation.CurrentFrame.SetGlyph(centers.X, 0, 254);
                overlay.Animation.CurrentFrame.SetGlyph(centers.X, overlay.Animation.Height - 1, 254);
            }

            if (objectType == ObjectType.Entity)
            {
                overlay.Animation.CurrentFrame.SetGlyph(Entity.Animation.Center.X + 1, Entity.Animation.Center.Y + 1, '*');
                overlay.Animation.CurrentFrame.SetBackground(Entity.Animation.Center.X + 1, Entity.Animation.Center.Y + 1, Color.Black);
                overlay.Animation.Tint = Color.Black * 0.3f;

                //MainScreen.Instance.Brush = overlay;
                //MainScreen.Instance.UpdateBrush();
            }
        }




        public enum ObjectType
        {
            Zone,
            Entity,
            SelectionBox,
            ControlText
        }

        public struct ResizeRules
        {
            public bool AllowLeftRight;
            public bool AllowTopBottom;
            public bool AllowMove;

            //public bool AllowLeft;
            //public bool AllowRight;
            //public bool AllowTop;
            //public bool AllowBottom;

            public ResizeRules(bool allowLeftRight, bool allowTopBottom, bool allowMove)
            {
                AllowLeftRight = allowLeftRight;
                AllowTopBottom = allowTopBottom;
                AllowMove = allowMove;
            }

            public static ResizeRules Get(ObjectType objectType)
            {
                switch (objectType)
                {
                    case ObjectType.Zone:
                        return new ResizeRules(true, true, true);
                    case ObjectType.ControlText:
                        return new ResizeRules(true, true, true);
                    case ObjectType.Entity:
                        return new ResizeRules(false, false, true);
                    case ObjectType.SelectionBox:
                        return new ResizeRules(true, true, true);
                }

                return new ResizeRules(false, false, false);
            }
        }
    }
}
