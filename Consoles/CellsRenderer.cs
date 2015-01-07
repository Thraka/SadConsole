namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Renders cells, or a subset of cells, to the screen.
    /// </summary>
    [DataContract]
    public class CellsRenderer : IRender, IUpdate
    {
        protected Point _cellSize;
        protected Rectangle _renderArea;
        protected Cell[] _renderAreaCells;
        protected Rectangle[] _renderAreaRects;
        [DataMember(Name="Font")]
        protected FontBase _font;
        protected Rectangle _tintArea;
        protected CellSurface _cellData;
        protected Point _position;
        protected bool _isVisible;
        protected bool _isReady;
        protected bool _skipBatchBeginEnd;

        [DataMember(Name="UsingCustomArea")]
        private bool _renderAreaCustomSet;

        /// <summary>
        /// The color tint to apply across the console after the cells have been rendered.
        /// </summary>
        [DataMember]
        public Color Tint { get; set; }

        /// <summary>
        /// Returns a rectangle of the entire area of the console that will be rendered every frame. This is in absolute pixels.
        /// </summary>
        public Rectangle RenderBox { get { return _tintArea; } }

        /// <summary>
        /// Gets or sets the font used when rendering this surface.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is set to null.</exception>
        public FontBase Font
        {
            // TODO: If font is changed, all cells should double check that the character index is still valid.
            get { return _font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("The Font property cannot be set to null.");

                _font = value;
                CellSize = new Point(_font.CellWidth, _font.CellHeight);

                if (!_renderAreaCustomSet)
                    ResetViewArea();
                else
                    CalculateRenderArea();

                OnFontChanged();
            }
        }

        /// <summary>
        /// When true, positions the render area based on pixels rather than font cell size.
        /// </summary>
        [DataMember]
        public bool UseAbsolutePositioning { get; set; }

        /// <summary>
        /// Indicates whether or not this console is visible.
        /// </summary>
        [DataMember]
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; OnVisibleChanged(); } }

        /// <summary>
        /// A transform used when rendering the cell area. If this property is null, the <see cref="Position"/> property will be used for rendering.
        /// </summary>
        [DataMember]
        public Matrix? Transform { get; set; }

        /// <summary>
        /// Handler called after the console renders its cells.
        /// </summary>
        public Action<CellsRenderer> AfterRenderHandler { get; set; }

        /// <summary>
        /// Handler called before the console renders its cells.
        /// </summary>
        public Action<CellsRenderer> BeforeRenderHandler { get; set; }

        /// <summary>
        /// The SpriteBatch used when rendering cell data.
        /// </summary>
        public SpriteBatch Batch { get; protected set; }


        /// <summary>
        /// Updates only the effects of the visible cells. Speeds up processing on big consoles. Defaults to true.
        /// </summary>
        [DataMember]
        public bool UpdateOnlyViewCells { get; set; }

        /// <summary>
        /// The rendering size of each cell.
        /// </summary>
        [DataMember]
        public Point CellSize
        {
            get
            {
                return _cellSize;
            }
            set
            {
                _cellSize = value;
                CalculateRenderArea();
            }
        }

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        [DataMember]
        public Point Position
        {
            get { return _position; }
            set { Point previousPosition = _position; _position = value; OnPositionChanged(previousPosition); }
        }

        /// <summary>
        /// The cell data used when rendering.
        /// </summary>
        [DataMember]
        public CellSurface CellData
        {
            get { return _cellData; }
            set
            {
                if (_cellData != value)
                {
                    var oldSurace = _cellData;

                    if (_cellData != null)
                        _cellData.Resized -= cellData_Resized;

                    _cellData = value;

                    if (_cellData != null)
                        _cellData.Resized += cellData_Resized;

                    if (!_renderAreaCustomSet || _renderArea == new Rectangle(0, 0, _cellData.Width, _cellData.Height))
                        ResetViewArea();
                    else
                        CalculateRenderArea();

                    OnCellDataChanged(oldSurace, _cellData);
                }
            }
        }

        /// <summary>
        /// The area of the cell data to render.
        /// </summary>
        [DataMember]
        public Rectangle ViewArea
        {
            get
            {
                return _renderArea;
            }
            set
            {
                if (_cellData == null || value != new Rectangle(0, 0, _cellData.Width, _cellData.Height))
                {
                    _renderAreaCustomSet = true;
                    _renderArea = value;
                    CalculateRenderArea();
                }
                else
                    ResetViewArea();
            }
        }

        /// <summary>
        /// When false, does not performs the code within the <see cref="Update()"/> method. Defaults to true.
        /// </summary>
        [DataMember]
        public bool DoUpdate { get; set; }

        /// <summary>
        /// Creates a new instance of the CellsRenderer class with an existing cell data and a SpriteBatch to render with.
        /// </summary>
        /// <param name="cellData">The cell data.</param>
        /// <param name="spriteBatch">The SpriteBatch used for rendering.</param>
        public CellsRenderer(CellSurface cellData, SpriteBatch spriteBatch)
        {
            Batch = spriteBatch;
            DoUpdate = true;
            _isVisible = true;
            Position = new Point(0, 0);
            Tint = Color.Transparent;
            Font = Engine.DefaultFont;
            UpdateOnlyViewCells = true;

            CellData = cellData;
            ResetViewArea();
        }

        /// <summary>
        /// Updates all cell effects on cell data and updates the virtual cursor's cell effects.
        /// </summary>
        public virtual void Update()
        {
            if (DoUpdate && _isReady)
            {
                //if (UpdateOnlyViewCells)
                //{
                //    int cellCount = _renderAreaCells.Length;

                //    for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
                //    {
                //        var cell = _renderAreaCells[cellIndex];

                //        if (cell.IsVisible && cell.Effect != null && !cell.Effect.Finished)
                //            cell.Effect.Process(Engine.GameTimeElapsedUpdate, cell);
                //    }
                //}
                //else
                //{
                //    int cellCount = CellData.CellCount;

                //    for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
                //    {
                //        var cell = CellData[cellIndex];

                //        if (cell.IsVisible && cell.Effect != null && !cell.Effect.Finished)
                //            cell.Effect.Process(Engine.GameTimeElapsedUpdate, cell);
                //    }
                //}


                // Support passing in a list of effects to update instead.
                // Then if the UpdateOnlyViewCells is true, process all cells in view
                // Collecting the effects. Send the list to the UpdateEffects method
                CellData.UpdateEffects(Engine.GameTimeElapsedUpdate);
            }
        }

        /// <summary>
        /// Renders the cells to the screen.
        /// </summary>
        public virtual void Render()
        {
            if (IsVisible && _isReady)
            {
                Matrix transform;

                if (Transform.HasValue)
                    transform = Transform.Value;
                else
                    transform = GetPositionTransform();

                if (!_skipBatchBeginEnd)
                    Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, transform);

                OnBeforeRender();

                if (Tint.A != 255)
                {
                    Cell cell;

                    if (CellData.DefaultBackground.A != 0)
                        Batch.Draw(Engine.BackgroundCell, _tintArea, null, CellData.DefaultBackground);

                    for (int i = 0; i < _renderAreaRects.Length; i++)
                    {
                        cell = _renderAreaCells[i];

                        if (cell.IsVisible)
                        {
                            if (cell.ActualBackground != Color.Transparent && cell.ActualBackground != CellData.DefaultBackground)
                                Batch.Draw(Engine.BackgroundCell, _renderAreaRects[i], null, cell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                        }
                    }

                    for (int i = 0; i < _renderAreaRects.Length; i++)
                    {
                        cell = _renderAreaCells[i];

                        if (cell.IsVisible)
                        {
                            // "Print" the foreground of the cell on top of the background
                            if (cell.ActualForeground != Color.Transparent)
                                Batch.Draw(this.Font.Image, _renderAreaRects[i], Font.CharacterIndexRects[cell.ActualCharacterIndex], cell.ActualForeground, 0f, Vector2.Zero, cell.ActualSpriteEffect, 0.1f);
                        }
                    }

                    OnAfterRender();

                    if (Tint.A != 0)
                        Batch.Draw(Engine.BackgroundCell, _tintArea, null, Tint);
                }
                else
                {
                    Batch.Draw(Engine.BackgroundCell, _tintArea, null, Tint);
                }

                if (!_skipBatchBeginEnd)
                    Batch.End();
            }
        }

        /// <summary>
        /// Caches the render rectangles from the cell surface based on the area rectangle.
        /// </summary>
        protected void CalculateRenderArea()
        {
            if (CellData == null)
            {
                _isReady = false;
                return;
            }

            if (_renderArea.Width > CellData.Width)
                _renderArea.Width = CellData.Width;
            if (_renderArea.Height > CellData.Height)
                _renderArea.Height = CellData.Height;

            if (_renderArea.X < 0)
                _renderArea.X = 0;
            if (_renderArea.Y < 0)
                _renderArea.Y = 0;

            if (_renderArea.X + _renderArea.Width > CellData.Width)
                _renderArea.X = CellData.Width - _renderArea.Width;
            if (_renderArea.Y + _renderArea.Height > CellData.Height)
                _renderArea.Y = CellData.Height - _renderArea.Height;

            // Cache all of the rendering rectangles
            _renderAreaRects = new Rectangle[_renderArea.Width * _renderArea.Height];
            _renderAreaCells = new Cell[_renderArea.Width * _renderArea.Height];

            int index = 0;

            for (int y = 0; y < _renderArea.Height; y++)
            {
                for (int x = 0; x < _renderArea.Width; x++)
                {
                    _renderAreaRects[index] = new Rectangle(x * CellSize.X, y * CellSize.Y, CellSize.X, CellSize.Y);
                    _renderAreaCells[index] = CellData[(y + _renderArea.Top) * CellData.Width + (x + _renderArea.Left)];
                    index++;
                }
            }

            _tintArea = new Rectangle(0, 0, _renderArea.Width * CellSize.X, _renderArea.Height * CellSize.Y);
            _isReady = true;
        }

        /// <summary>
        /// Called before all cells from the <see cref="CellData"/> have been drawn, after the <see cref="Batch"/> has started drawing. Calls the <see cref="BeforeRenderHandler"/> handler.
        /// </summary>
        protected virtual void OnBeforeRender() { if (BeforeRenderHandler != null) BeforeRenderHandler(this); }

        /// <summary>
        /// Called after all cells from the <see cref="CellData"/> have been drawn, before the <see cref="Batch"/> has ended drawing. Calls the <see cref="AfterRenderHandler"/> handler.
        /// </summary>
        protected virtual void OnAfterRender() { if (AfterRenderHandler != null) AfterRenderHandler(this); }

        /// <summary>
        /// Gets the Matrix transform that positions the console on the screen.
        /// </summary>
        /// <returns>The transform.</returns>
        public virtual Matrix GetPositionTransform()
        {
            Point worldLocation;

            if (UseAbsolutePositioning)
                worldLocation = Position;
            else
                worldLocation = Position.ConsoleLocationToWorld(CellSize.X, CellSize.Y);

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }

        /// <summary>
        /// Resets the <see cref="ViewArea"/> rectangle to match the size of the <see cref="CellData"/>.
        /// </summary>
        public void ResetViewArea()
        {
            if (CellData == null)
            {
                _isReady = false;
                return;
            }

            _renderAreaCustomSet = false;
            _renderArea = new Rectangle(0, 0, CellData.Width, CellData.Height);
            CalculateRenderArea();
        }

        /// <summary>
        /// Resizes the graphics device manager to <see cref="ViewArea"/> of the <see cref="CellData"/> based on the current <see cref="CellSize"/>.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="additionalWidth">Additional width to add to the resize.</param>
        /// <param name="additionalHeight">Additional height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (CellSize.X * ViewArea.Width) + additionalWidth;
            manager.PreferredBackBufferHeight = (CellSize.Y * ViewArea.Height) + additionalHeight;
            manager.ApplyChanges();

            Engine.WindowWidth = manager.PreferredBackBufferWidth;
            Engine.WindowHeight = manager.PreferredBackBufferHeight;
        }

        /// <summary>
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }
        
        /// <summary>
        /// Called when the Resized event of the <see cref="CellData"/> property is raised.
        /// </summary>
        protected virtual void OnResize() { }

        /// <summary>
        /// Called when the <see cref="CellData"/> property changes.
        /// </summary>
        /// <param name="oldCells">Previous cell surface.</param>
        /// <param name="newCells">New cell surface.</param>
        protected virtual void OnCellDataChanged(CellSurface oldCells, CellSurface newCells) { }

        /// <summary>
        /// Called when the visibility of the console changes.
        /// </summary>
        protected virtual void OnVisibleChanged() { }

        /// <summary>
        /// Called when the font changes.
        /// </summary>
        protected virtual void OnFontChanged() { }

        private void cellData_Resized(object sender, EventArgs e)
        {
            if (!_renderAreaCustomSet)
                ResetViewArea();
            else
                CalculateRenderArea();

            OnResize();
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            Batch = new SpriteBatch(Engine.Device);

            if (_cellData != null)
                _cellData.Resized += cellData_Resized;

            CalculateRenderArea();
        }
    }
}
