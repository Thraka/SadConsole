namespace SadConsoleEditor.Tools
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;
    using System.Collections.Generic;
    using SadConsole.Entities;
    using System.Linq;

    class SceneObjectMoveResizeTool : ITool
    {
        private Entity _boundingBox;

        private BoxToolPanel _settingsPanel;

        private Point clickOffset;
        private bool isDragging;
        private ResizableObject movingEntity;

        private Point resizeStartPosition;
        private Point resizeBounds;
        private bool lastLeftMouseDown = false;


        private bool moveRight;
        private bool moveTopRight;
        private bool moveBottomRight;

        private bool moveLeft;
        private bool moveTopLeft;
        private bool moveBottomLeft;

        private bool moveTop;
        private bool moveBottom;

        private bool isMoving;
        private bool isSelected;
        private bool isResizing;

        public const string ID = "SCENE-ENT-MOVE";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Move/Resize Object"; }
        }

        public char Hotkey { get { return 'm'; } }


        public CustomPanel[] ControlPanels { get; private set; }

        public override string ToString()
        {
            return Title;
        }

        public SceneObjectMoveResizeTool()
        {

        }

        public void OnSelected()
        {
            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).ShowDarkLayer = true;
            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).HighlightType = Editors.SceneEditor.HighlightTypes.Entity;

            MainScreen.Instance.Brush = new Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
        }

        public void OnDeselected()
        {
            ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).ShowDarkLayer = false;
        }

        public void RefreshTool()
        {
        }

        public void Update()
        {
        }

        public bool ProcessKeyboard(Keyboard info, SurfaceBase surface)
        {
            return false;
        }

        public void ProcessMouse(MouseConsoleState info, SurfaceBase surface, bool isInBounds)
        {
            var editor = MainScreen.Instance.ActiveEditor as Editors.SceneEditor;

            if(editor != null && info.IsOnConsole && !isResizing && !isMoving)
            {
                var zones = editor.Zones.ToList(); zones.Reverse();
                var allObjects = editor.Objects.Union(zones).ToList();

                for (int i = 0; i < allObjects.Count; i++)
                {
                    var area = allObjects[i].Entity.Animation.RenderArea;
                    area.Offset(allObjects[i].Entity.Position - allObjects[i].Entity.Animation.Center);
                    area.Inflate(1, 1);

                    var mousePosition = info.CellPosition;

                    if (!area.Contains(mousePosition))
                        continue;

                    if (movingEntity != null)
                        movingEntity.IsSelected = false;

                    movingEntity = allObjects[i];
                    movingEntity.IsSelected = true;

                    if (!info.Mouse.LeftButtonDown)
                        return;

                    // Select the zone in the list box
                    if (movingEntity.Type == ResizableObject.ObjectType.Zone && editor.ZonesPanel.SelectedEntity != movingEntity)
                        editor.ZonesPanel.SelectedEntity = movingEntity;
                    else if (movingEntity.Type == ResizableObject.ObjectType.Entity && editor.EntityPanel.SelectedEntity != movingEntity)
                        editor.EntityPanel.SelectedEntity = movingEntity;

                    var Entity = movingEntity.Entity;
                    var overlay = movingEntity.Overlay;
                    var rules = movingEntity.Rules;
                    
                    lastLeftMouseDown = true;

                    moveRight = false;
                    moveTopRight = false;
                    moveBottomRight = false;

                    moveLeft = false;
                    moveTopLeft = false;
                    moveBottomLeft = false;

                    moveTop = false;
                    moveBottom = false;

                    // Check and see if mouse is over me
                    if (rules.AllowMove &&
                        info.CellPosition.X >= Entity.Position.X && info.CellPosition.X <= Entity.Position.X + Entity.Animation.Width - 1 &&
                        info.CellPosition.Y >= Entity.Position.Y && info.CellPosition.Y <= Entity.Position.Y + Entity.Animation.Height - 1)
                    {
                        if (movingEntity.Type == ResizableObject.ObjectType.Entity)
                            editor.EntityPanel.SelectedEntity = movingEntity;

                        clickOffset = info.CellPosition - Entity.Position;
                        isMoving = true;
                        return;
                    }

                    else if (rules.AllowLeftRight && rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y + overlay.Animation.Height - 1 && info.CellPosition.X == overlay.Position.X + overlay.Animation.Width - 1)
                    {
                        isResizing = true;
                        moveBottomRight = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X, Entity.Position.Y);
                        return;
                    }
                    else if (rules.AllowLeftRight && rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y && info.CellPosition.X == overlay.Position.X + overlay.Animation.Width - 1)
                    {
                        isResizing = true;
                        moveTopRight = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X, Entity.Position.Y + Entity.Animation.Height - 1);
                        return;
                    }
                    else if (rules.AllowLeftRight &&
                        info.CellPosition.X == overlay.Position.X + overlay.Animation.Width - 1)
                    {
                        isResizing = true;
                        moveRight = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X, 0);
                        return;
                    }
                    else if (rules.AllowLeftRight && rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y + overlay.Animation.Height - 1 && info.CellPosition.X == overlay.Position.X)
                    {
                        isResizing = true;
                        moveBottomLeft = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X + Entity.Animation.Width - 1, Entity.Position.Y);
                        return;
                    }
                    else if (rules.AllowLeftRight && rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y && info.CellPosition.X == overlay.Position.X)
                    {
                        isResizing = true;
                        moveTopLeft = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X + Entity.Animation.Width - 1, Entity.Position.Y + Entity.Animation.Height - 1);
                        return;
                    }
                    else if (rules.AllowLeftRight &&
                        info.CellPosition.X == overlay.Position.X)
                    {
                        isResizing = true;
                        moveLeft = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(Entity.Position.X + Entity.Animation.Width - 1, 0);
                        return;
                    }
                    else if (rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y)
                    {
                        isResizing = true;
                        moveTop = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(0, Entity.Position.Y + Entity.Animation.Height - 1);
                        return;
                    }
                    else if (rules.AllowTopBottom &&
                        info.CellPosition.Y == overlay.Position.Y + overlay.Animation.Height - 1)
                    {
                        isResizing = true;
                        moveBottom = true;
                        resizeStartPosition = info.CellPosition;
                        resizeBounds = new Point(0, Entity.Position.Y);
                        return;
                    }
                }

                if (!info.Mouse.LeftButtonDown && movingEntity != null)
                {
                    movingEntity.IsSelected = false;
                    movingEntity = null;
                }
            }

            if (isResizing)
            {

                if (!info.Mouse.LeftButtonDown)
                {
                    isResizing = false;
                    return;
                }

                if (MainScreen.Instance.InnerEmptyBounds.Contains(info.WorldPosition))
                {
                    var cellPosition = info.ConsolePosition + surface.RenderArea.Location;

                    if (moveRight && cellPosition.X > resizeBounds.X)
                    {
                        movingEntity.ResizeObject(cellPosition.X - movingEntity.Entity.Position.X, movingEntity.Entity.Animation.Height);
                    }
                    else if (moveBottomRight && cellPosition.X > resizeBounds.X && cellPosition.Y > resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(cellPosition.X - movingEntity.Entity.Position.X, cellPosition.Y - movingEntity.Entity.Position.Y);
                    }
                    else if (moveTopRight && cellPosition.X > resizeBounds.X && cellPosition.Y < resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(cellPosition.X - movingEntity.Entity.Position.X, movingEntity.Entity.Position.Y + movingEntity.Entity.Animation.Height - (cellPosition.Y + 1), null, cellPosition.Y + 1);
                    }
                    else if (moveLeft && cellPosition.X < resizeBounds.X)
                    {
                        movingEntity.ResizeObject(movingEntity.Entity.Position.X + movingEntity.Entity.Animation.Width - (cellPosition.X + 1), movingEntity.Entity.Animation.Height, cellPosition.X + 1, null);
                    }
                    else if (moveBottomLeft && cellPosition.X < resizeBounds.X && cellPosition.Y > resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(movingEntity.Entity.Position.X + movingEntity.Entity.Animation.Width - (cellPosition.X + 1), cellPosition.Y - movingEntity.Entity.Position.Y, cellPosition.X + 1, null);
                    }
                    else if (moveTopLeft && cellPosition.X < resizeBounds.X && cellPosition.Y < resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(movingEntity.Entity.Position.X + movingEntity.Entity.Animation.Width - (cellPosition.X + 1), movingEntity.Entity.Position.Y + movingEntity.Entity.Animation.Height - (cellPosition.Y + 1), cellPosition.X + 1, cellPosition.Y + 1);
                    }
                    else if (moveTop && cellPosition.Y < resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(movingEntity.Entity.Animation.Width, movingEntity.Entity.Position.Y + movingEntity.Entity.Animation.Height - (cellPosition.Y + 1), null, cellPosition.Y + 1);
                    }
                    else if (moveBottom && cellPosition.Y > resizeBounds.Y)
                    {
                        movingEntity.ResizeObject(movingEntity.Entity.Animation.Width, cellPosition.Y - movingEntity.Entity.Position.Y);
                    }
                }

                return;
            }
            else if (isMoving)
            {
                if (!info.Mouse.LeftButtonDown)
                {
                    isMoving = false;
                    return;
                }

                if (MainScreen.Instance.InnerEmptyBounds.Contains(info.WorldPosition))
                {
                    var cellPosition = info.ConsolePosition + surface.RenderArea.Location;
                    movingEntity.Entity.Position = cellPosition - clickOffset;
                    movingEntity.ProcessOverlay();
                }
            }

            lastLeftMouseDown = info.Mouse.LeftButtonDown;

            return;
        }
    }
}
