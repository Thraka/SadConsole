using System;
using SadConsoleEditor.Tools;
using SadConsole.Input;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;
using SadConsoleEditor.Consoles;
using SadConsoleEditor.Panels;
using SadConsole.GameHelpers;
using SadConsole.Consoles;
using System.Collections.Generic;

namespace SadConsoleEditor.Editors
{
    class GameScreenEditor : IEditor
    {
        private int _width;
        private int _height;
        private LayeredConsole _consoleLayers;
        private SadConsole.Consoles.CellsRenderer _objectsSurface;
        private bool _displayObjectLayer;

        public event EventHandler<MouseEventArgs> MouseEnter;
        public event EventHandler<MouseEventArgs> MouseExit;
        public event EventHandler<MouseEventArgs> MouseMove;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public EditorSettings Settings { get { return SadConsoleEditor.Settings.Config.GameScreenEditor; } }

        public LayeredConsole Surface { get { return _consoleLayers; } }

        public GameObjectCollection SelectedGameObjects { get; set; }

        public List<GameObjectCollection> GameObjects;

        public bool DisplayObjectLayer { set { _displayObjectLayer = value; } }

        public const string ID = "GAME";

        public string ShortName { get { return "Game"; } }

        public string Id { get { return ID; } }

        public string Title { get { return "Game Screen"; } }

        public string FileExtensionsLoad { get { return "*.screen"; } }
        public string FileExtensionsSave { get { return "*.screen"; } }
        public CustomPanel[] ControlPanels { get; private set; }

        public string[] Tools
        {
            get
            {
                return new string[] { PaintTool.ID, RecolorTool.ID, FillTool.ID, TextTool.ID, SelectionTool.ID, LineTool.ID, BoxTool.ID, CircleTool.ID, ObjectTool.ID };
            }
        }

        private EventHandler<MouseEventArgs> _mouseMoveHandler;
        private EventHandler<MouseEventArgs> _mouseEnterHandler;
        private EventHandler<MouseEventArgs> _mouseExitHandler;


        public GameScreenEditor()
        {
            Reset();
        }

        public void Reset()
        {
            ControlPanels = new CustomPanel[] { EditorConsoleManager.Instance.ToolPane.FilesPanel, EditorConsoleManager.Instance.ToolPane.LayersPanel, EditorConsoleManager.Instance.ToolPane.ToolsPanel };

            if (_consoleLayers != null)
            {
                _consoleLayers.MouseMove -= _mouseMoveHandler;
                _consoleLayers.MouseEnter -= _mouseEnterHandler;
                _consoleLayers.MouseExit -= _mouseExitHandler;
            }

            _objectsSurface = new SadConsole.Consoles.Console(25, 10);
            _objectsSurface.Font = SadConsoleEditor.Settings.Config.ScreenFont;
            _objectsSurface.Data.DefaultForeground = Color.White;
            _objectsSurface.Data.DefaultBackground = Color.Transparent;
            _objectsSurface.Data.Clear();
            _objectsSurface.BeforeRenderHandler = (cr) => cr.Batch.Draw(SadConsole.Engine.BackgroundCell, cr.RenderBox, null, new Color(0, 0, 0, 0.5f)); 

            _consoleLayers = new LayeredConsole(1, 25, 10);
            _consoleLayers.Font = SadConsoleEditor.Settings.Config.ScreenFont;
            _consoleLayers.CanUseMouse = true;
            _consoleLayers.CanUseKeyboard = true;
            _consoleLayers.GetLayerMetadata(0).Name = "Root";
            _consoleLayers.GetLayerMetadata(0).IsRemoveable = false;
            _consoleLayers.GetLayerMetadata(0).IsMoveable = false;
            
            _width = 25;
            _height = 10;

            SelectedGameObjects = new GameObjectCollection();
            GameObjects = new List<GameObjectCollection>();
            GameObjects.Add(SelectedGameObjects);

            _mouseMoveHandler = (o, e) => { if (this.MouseMove != null) this.MouseMove(_consoleLayers.ActiveLayer, e); EditorConsoleManager.Instance.ToolPane.SelectedTool.MouseMoveSurface(e.OriginalMouseInfo, _consoleLayers.ActiveLayer); };
            _mouseEnterHandler = (o, e) => { if (this.MouseEnter != null) this.MouseEnter(_consoleLayers.ActiveLayer, e); EditorConsoleManager.Instance.ToolPane.SelectedTool.MouseEnterSurface(e.OriginalMouseInfo, _consoleLayers.ActiveLayer); };
            _mouseExitHandler = (o, e) => { if (this.MouseExit != null) this.MouseExit(_consoleLayers.ActiveLayer, e); EditorConsoleManager.Instance.ToolPane.SelectedTool.MouseExitSurface(e.OriginalMouseInfo, _consoleLayers.ActiveLayer); };

            _consoleLayers.MouseMove += _mouseMoveHandler;
            _consoleLayers.MouseEnter += _mouseEnterHandler;
            _consoleLayers.MouseExit += _mouseExitHandler;

        }

        public override string ToString()
        {
            return Title;
        }

        public void ProcessKeyboard(KeyboardInfo info)
        {
            EditorConsoleManager.Instance.ToolPane.SelectedTool.ProcessKeyboard(info, _consoleLayers.ActiveLayer);
        }

        public void ProcessMouse(MouseInfo info)
        {
            _consoleLayers.ProcessMouse(info);

            if (_consoleLayers.IsMouseOver)
            {
                EditorConsoleManager.Instance.ToolPane.SelectedTool.ProcessMouse(info, _consoleLayers.ActiveLayer);
                EditorConsoleManager.Instance.SurfaceMouseLocation = info.ConsoleLocation;
            }
            else
                EditorConsoleManager.Instance.SurfaceMouseLocation = Point.Zero;
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;

            _consoleLayers.Resize(width, height);
            _objectsSurface.Data.Resize(width, height);

            // inform the outer box we've changed size
            EditorConsoleManager.Instance.UpdateBox();
        }

        public void Position(int x, int y)
        {
            _consoleLayers.Move(new Point(x, y));
            _objectsSurface.Position = new Point(x, y);
        }

        public void Position(Point newPosition)
        {
            _consoleLayers.Move(newPosition);
            _objectsSurface.Position = newPosition;
        }

        public Point GetPosition()
        {
            return _consoleLayers.Position;
        }

        public void Render()
        {
            Surface.Render();

            if (_displayObjectLayer)
            {
                _objectsSurface.Render();
            }
        }

        public void Save(string file)
        {
            GameConsole tempConsole = new GameConsole(_consoleLayers.Layers, _consoleLayers.Width, _consoleLayers.Height);

            for (int i = 0; i < _consoleLayers.Layers; i++)
                tempConsole.RemoveLayer(0);

            for (int i = 0; i < _consoleLayers.Layers; i++)
            {
                tempConsole.AddLayer(_consoleLayers[i].CellData);
                var metadataNew = new GameConsoleMetadata();
                var metadataOld = _consoleLayers.GetLayerMetadata(i);

                metadataNew.IsMoveable = metadataOld.IsMoveable;
                metadataNew.IsRemoveable = metadataOld.IsRemoveable;
                metadataNew.IsRenamable = metadataOld.IsRenamable;
                metadataNew.IsVisible = metadataOld.IsVisible;
                metadataNew.Name = metadataOld.Name;
                metadataNew.Index = metadataOld.Index;
                metadataNew.GameObjects = GameObjects[i];
                tempConsole.SetLayerMetadata(i, metadataNew);
            }

            SadConsole.Serializer.Save<GameConsole>(tempConsole, file);
            //tempConsole.Save(file);

            //_consoleLayers.Save(file);
            //GameObjectCollection.SaveCollection(GameObjects, file.Replace(System.IO.Path.GetExtension(file), ".objects"));
        }

        public void Load(string file)
        {
            if (System.IO.File.Exists(file))
            {
                if (_consoleLayers != null)
                {
                    _consoleLayers.MouseMove -= _mouseMoveHandler;
                    _consoleLayers.MouseEnter -= _mouseEnterHandler;
                    _consoleLayers.MouseExit -= _mouseExitHandler;
                }


                var tempConsole = SadConsole.Serializer.Load<GameConsole>(file);
                _consoleLayers = new LayeredConsole(tempConsole.Layers, tempConsole.Width, tempConsole.Height);
                _consoleLayers.Font = SadConsoleEditor.Settings.Config.ScreenFont;
                for (int i = 0; i < tempConsole.Layers; i++)
                    _consoleLayers.RemoveLayer(0);


                GameObjects = new List<GameObjectCollection>(tempConsole.Layers);

                for (int i = 0; i < tempConsole.Layers; i++)
                {
                    _consoleLayers.AddLayer(tempConsole[i].CellData);
                    var metadataNew = new LayeredConsoleMetadata();
                    var metadataOld = tempConsole.GetLayerMetadata(i);

                    metadataNew.IsMoveable = metadataOld.IsMoveable;
                    metadataNew.IsRemoveable = metadataOld.IsRemoveable;
                    metadataNew.IsRenamable = metadataOld.IsRenamable;
                    metadataNew.IsVisible = metadataOld.IsVisible;
                    metadataNew.Name = metadataOld.Name;
                    metadataNew.Index = metadataOld.Index;
                    _consoleLayers.SetLayerMetadata(i, metadataNew);
                    GameObjects.Add(metadataOld.GameObjects);
                    _consoleLayers[i].IsVisible = metadataNew.IsVisible;
                }

                _consoleLayers.SetActiveLayer(0);
                _consoleLayers.CanUseMouse = true;
                _consoleLayers.CanUseKeyboard = true;
                _consoleLayers.MouseMove += _mouseMoveHandler;
                _consoleLayers.MouseEnter += _mouseEnterHandler;
                _consoleLayers.MouseExit += _mouseExitHandler;

                _width = _consoleLayers.Width;
                _height = _consoleLayers.Height;

                EditorConsoleManager.Instance.UpdateBox();

                SelectedGameObjects = GameObjects[0];
                SyncObjectsToLayer();



                //_consoleLayers = LayeredConsole.Load<LayeredConsole>(file);
                //_consoleLayers.Font = SadConsoleEditor.Settings.Config.ScreenFont;

                //_consoleLayers.MouseMove += _mouseMoveHandler;
                //_consoleLayers.MouseEnter += _mouseEnterHandler;
                //_consoleLayers.MouseExit += _mouseExitHandler;

                //_width = _consoleLayers.Width;
                //_height = _consoleLayers.Height;

                //EditorConsoleManager.Instance.UpdateBox();

                //if (System.IO.File.Exists(objectsFile))
                //{
                //    GameObjects = new List<GameObjectCollection>(GameObjectCollection.LoadCollection(objectsFile));
                //    SelectedGameObjects = GameObjects[0];
                //    SyncObjectsToLayer();
                //}
                //else
                //{
                //    GameObjects = new List<GameObjectCollection>();

                //    foreach (var layer in _consoleLayers.GetEnumeratorForLayers())
                //    {
                //        GameObjects.Add(new GameObjectCollection());
                //    }
                //    SelectedGameObjects = GameObjects[0];
                //    SyncObjectsToLayer();
                //}
            }
        }


        /// <summary>
        /// Draws all of the game objects to the game object render layer
        /// </summary>
        public void SyncObjectsToLayer()
        {
            _objectsSurface.Data.Clear();

            _objectsSurface.Data.Resize(_width, _height);

            foreach (var item in SelectedGameObjects)
            {
                _objectsSurface.Data.Print(item.Key.X, item.Key.Y, " ", item.Value.Character);
                _objectsSurface.Data.SetCharacter(item.Key.X, item.Key.Y, item.Value.Character.CharacterIndex);
            }
        }

        public void RemoveLayer(int index)
        {
            Surface.RemoveLayer(index);
            GameObjects.RemoveAt(index);
        }

        public void MoveLayerUp(int index)
        {
            Surface.MoveLayer(index, index + 1);

            var gameObject = GameObjects[index];
            GameObjects.RemoveAt(index);
            GameObjects.Insert(index + 1, gameObject);
        }

        public void MoveLayerDown(int index)
        {
            Surface.MoveLayer(index, index - 1);

            var gameObject = GameObjects[index];
            GameObjects.RemoveAt(index);
            GameObjects.Insert(index - 1, gameObject);
        }

        public void AddNewLayer(string name)
        {
            Surface.AddLayer(name);
            GameObjects.Add(new GameObjectCollection());
        }

        public bool LoadLayer(string file)
        {
            if (System.IO.File.Exists(file))
            {
                var surface = SadConsole.TextSurface.Load(file);

                if (surface.Width != EditorConsoleManager.Instance.SelectedEditor.Surface.Width || surface.Height != EditorConsoleManager.Instance.SelectedEditor.Height)
                {
                    var newLayer = EditorConsoleManager.Instance.SelectedEditor.Surface.AddLayer("Loaded");
                    surface.Copy(newLayer.CellData);
                }
                else
                    EditorConsoleManager.Instance.SelectedEditor.Surface.AddLayer(surface);

                string objectFileName = file.Replace(System.IO.Path.GetExtension(file), ".object");
                if (System.IO.File.Exists(objectFileName))
                    GameObjects.Add(GameObjectCollection.Load(objectFileName));
                else
                    GameObjects.Add(new GameObjectCollection());

                return true;
            }
            else
                return false;
        }

        public void SaveLayer(int index, string file)
        {
            EditorConsoleManager.Instance.SelectedEditor.Surface[index].Data.Save(file);
            GameObjectCollection.Save(SelectedGameObjects, file.Replace(System.IO.Path.GetExtension(file), ".object"));
        }

        public void SetActiveLayer(int index)
        {
            Surface.SetActiveLayer(index);
            SelectedGameObjects = GameObjects[index];
            SyncObjectsToLayer();
        }
    }
}
