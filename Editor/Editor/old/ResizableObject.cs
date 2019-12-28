using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using SadConsole.Entities;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor
{
    class ResizableObject<TData>: ResizableObject
    {
        public TData Data;

        public ResizableObject(ObjectType objectType, Entity Entity, TData data): base(objectType, Entity)
        {
            Data = data;
        }
    }

    class ResizableObject
    {
        private Entity overlay;
        private Entity Entity;
        private ObjectType objectType;
        private Point renderOffset;

        private bool isSelected;

        public ResizeRules Rules;

        private static SadConsole.Renderers.SadConsole.Renderers.Basic tempRenderer = new SadConsole.Renderers.SadConsole.Renderers.Basic();

        public Entity Entity { get { return Entity; } }

        public Entity Overlay { get { return overlay; } }

        public ObjectType Type { get { return objectType; } }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; ProcessOverlay(); }
        }

        public ResizableObject(ObjectType objectType, Entity Entity)
        {
            this.objectType = objectType;
            this.Entity = Entity;
            
            renderOffset = Entity.PositionOffset;

            Rules = ResizeRules.Get(objectType);
            ProcessOverlay();
        }

        public Point RenderOffset
        {
            get { return renderOffset; }
            set { renderOffset = value; ProcessOverlay(); }
        }

        public string Name
        {
            get { return Entity.Name; }
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

                    var newAnimation = new AnimatedSurface(Entity.Animation.Name, width, height);
                    SadConsoleEditor.Settings.QuickEditor.TextSurface = newAnimation.CreateFrame();
                    SadConsoleEditor.Settings.QuickEditor.Fill(Color.White, backColor, 0);
                    SadConsoleEditor.Settings.QuickEditor.Print(0, 0, Name, Color.DarkGray);

                    Entity.Animation = newAnimation;
                    Entity.Update(SadConsole.Global.GameTimeUpdate.ElapsedGameTime);

                    Entity.Position = new Point(positionX ?? Entity.Position.X, positionY ?? Entity.Position.Y);

                    ProcessOverlay();
                }
            }
        }

        public virtual void Recolor(Color color)
        {
            SadConsoleEditor.Settings.QuickEditor.TextSurface = Entity.Animation.CurrentFrame;
            SadConsoleEditor.Settings.QuickEditor.Fill(Color.White, color, 0);
            SadConsoleEditor.Settings.QuickEditor.Print(0, 0, Name, Color.DarkGray);
        }

        public void DrawZone()
        {
            SadConsoleEditor.Settings.QuickEditor.TextSurface = Entity.Animation.CurrentFrame;
            SadConsoleEditor.Settings.QuickEditor.Fill(Color.White, Entity.Animation.CurrentFrame[0].Background, 0);
            SadConsoleEditor.Settings.QuickEditor.Print(0, 0, Name, Color.DarkGray);
        }

        public void Draw()
        {
            // This steals the code from the renderer. This DRAW method is called in the middle of
            // an existing draw call chain, so the existing state of the Global.SpriteBatch is reused.
            Entity.PositionOffset = RenderOffset;
            Entity.Draw(SadConsole.Global.GameTimeUpdate.ElapsedGameTime);
            //tempRenderer.RenderCells(Entity.Animation, true);

            if (isSelected)
            {
                overlay.PositionOffset = Entity.PositionOffset = renderOffset;
                overlay.Position = Entity.Position - Entity.Animation.Center - new Point(1);
                //tempRenderer.RenderCells(overlay.Animation, true);
                overlay.Draw(SadConsole.Global.GameTimeUpdate.ElapsedGameTime);
            }

        }

        public void ProcessOverlay()
        {
            if (overlay == null)
                overlay = new Entity(1, 1, Settings.Config.ScreenFont);

            overlay.PositionOffset = Entity.PositionOffset = renderOffset;
            overlay.Position = Entity.Position - Entity.Animation.Center - new Point(1);

            if (overlay.Animation.Width != Entity.Animation.Width + 2 || overlay.Animation.Height != Entity.Animation.Height + 2)
            {
                overlay.Animation = new AnimatedSurface("default", Entity.Animation.Width + 2, Entity.Animation.Height + 2, Settings.Config.ScreenFont);
                SadConsoleEditor.Settings.QuickEditor.TextSurface = overlay.Animation.CreateFrame();
            }
            else
                SadConsoleEditor.Settings.QuickEditor.TextSurface = overlay.Animation.CurrentFrame;

            var box = SadConsole.Shapes.Box.GetDefaultBox();
            box.Width = overlay.Animation.Width;
            box.Height = overlay.Animation.Height;
            box.Fill = false;
            box.TopLeftCharacter = box.TopSideCharacter = box.TopRightCharacter = box.LeftSideCharacter = box.RightSideCharacter = box.BottomLeftCharacter = box.BottomSideCharacter = box.BottomRightCharacter = 177;

            var centers = new Point(box.Width / 2, box.Height / 2);
            

            box.Draw(SadConsoleEditor.Settings.QuickEditor);

            if (Rules.AllowLeftRight && Rules.AllowTopBottom)
            {
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(0, 0, 254);
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(SadConsoleEditor.Settings.QuickEditor.Width - 1, 0, 254);
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(SadConsoleEditor.Settings.QuickEditor.Width - 1, SadConsoleEditor.Settings.QuickEditor.Height - 1, 254);
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(0, SadConsoleEditor.Settings.QuickEditor.Height - 1, 254);
            }

            if (Rules.AllowLeftRight)
            {
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(0, centers.Y, 254);
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(SadConsoleEditor.Settings.QuickEditor.Width - 1, centers.Y, 254);
            }

            if (Rules.AllowTopBottom)
            {
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(centers.X, 0, 254);
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(centers.X, SadConsoleEditor.Settings.QuickEditor.Height - 1, 254);
            }

            if (objectType == ObjectType.Entity)
            {
                SadConsoleEditor.Settings.QuickEditor.SetGlyph(Entity.Animation.Center.X + 1, Entity.Animation.Center.Y + 1, '*');
                SadConsoleEditor.Settings.QuickEditor.SetBackground(Entity.Animation.Center.X + 1, Entity.Animation.Center.Y + 1, Color.Black);
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
