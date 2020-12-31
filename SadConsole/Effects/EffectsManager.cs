
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Effects
{
    /// <summary>
    /// Effects manager for a text surface.
    /// </summary>
    public class EffectsManager
    {
        /// <summary>
        /// A dictionary of effect data keyed by the effect.
        /// </summary>
        protected Dictionary<ICellEffect, ColoredGlyphEffectData> _effects;

        /// <summary>
        /// A dictionary of effect data keyed by the cell index.
        /// </summary>
        protected Dictionary<int, ColoredGlyphEffectData> _effectCells;

        /// <summary>
        /// The surface hosting this effects manager.
        /// </summary>
        protected ICellSurface _backingSurface;

        /// <summary>
        /// Gets the number of effects.
        /// </summary>
        public int Count => _effects.Count;

        /// <summary>
        /// Creates a new effects manager associated with a text surface.
        /// </summary>
        /// <param name="surface">Text surface to manage.</param>
        public EffectsManager(ICellSurface surface)
        {
            _effects = new Dictionary<ICellEffect, ColoredGlyphEffectData>(20);
            _effectCells = new Dictionary<int, ColoredGlyphEffectData>(50);
            _backingSurface = surface;
        }

        /// <summary>
        /// Changes the effect of a specific cell.
        /// </summary>
        /// <param name="cellIndex">Cell index to set the effect for.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(int cellIndex, ICellEffect effect)
        {
            ColoredGlyph cell = _backingSurface[cellIndex];

            if (effect != null)
            {
                ColoredGlyphEffectData workingEffect;

                if (effect.CloneOnAdd)
                {
                    effect = effect.Clone();
                    workingEffect = new ColoredGlyphEffectData(effect);
                    _effects.Add(workingEffect.Effect, workingEffect);
                }
                else
                {
                    // Is the effect unknown? Add it.
                    if (GetKnownEffect(effect, out workingEffect) == false)
                    {
                        _effects.Add(workingEffect.Effect, workingEffect);
                    }
                    else
                    {
                        if (workingEffect.ContainsCell(cellIndex))
                        {
                            // Make sure the effect is attached to the cell.
                            return;
                        }
                    }
                }

                // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                ClearCellEffect(cellIndex);

                // Add the cell to the effects by cell key and to list of known cells for the effect
                _effectCells.Add(cellIndex, workingEffect);
                workingEffect.CellsStates.Add(new ColoredGlyphWithState(cell, cellIndex));
            }
            else
            {
                ClearCellEffect(cellIndex);
            }
        }

        /// <summary>
        /// Changes the effect of the <paramref name="cellIndicies"/> provided.
        /// </summary>
        /// <param name="cellIndicies">A list of cell indicies to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(IEnumerable<int> cellIndicies, ICellEffect effect)
        {
            var cells = cellIndicies.Select(i => (_backingSurface[i], i));

            if (effect != null)
            {
                ColoredGlyphEffectData workingEffect;

                if (effect.CloneOnAdd)
                {
                    effect = effect.Clone();
                    workingEffect = new ColoredGlyphEffectData(effect);
                    _effects.Add(workingEffect.Effect, workingEffect);
                }
                else
                {
                    // Is the effect unknown? Add it.
                    if (GetKnownEffect(effect, out workingEffect) == false)
                    {
                        _effects.Add(workingEffect.Effect, workingEffect);
                    }
                }

                foreach (var cell in cells)
                {
                    if (!workingEffect.ContainsCell(cell.i))
                    {
                        // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                        ClearCellEffect(cell.i);

                        // Add the cell to the effects by cell key and to list of known cells for the effect
                        _effectCells.Add(cell.i, workingEffect);
                        workingEffect.CellsStates.Add(new ColoredGlyphWithState(cell.Item1, cell.i));
                    }
                }
            }
            else
            {
                foreach (var cell in cells)
                {
                    ClearCellEffect(cell.i);
                }
            }
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(int cellIndex) => _effectCells.ContainsKey(cellIndex) ? _effectCells[cellIndex].Effect : null;

        /// <summary>
        /// Gets a collection of effects associated with the manager.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICellEffect> GetEffects()
        {
            if (_effects.Keys.Count == 0)
                return null;

            return _effects.Keys;
        }

        /// <summary>
        /// Removes an effect and associated cells from the manager.
        /// </summary>
        /// <param name="effect">Effect to remove.</param>
        public void Remove(ICellEffect effect)
        {
            if (_effects.ContainsKey(effect))
            {
                ColoredGlyphWithState[] states = _effects[effect].CellsStates.ToArray();

                foreach (var state in states)
                    ClearCellEffect(state.CellIndex);
            }
        }

        /// <summary>
        /// Removes all effects and associated cells.
        /// </summary>
        public void RemoveAll()
        {
            ICellEffect[] effects = _effects.Keys.ToArray();

            foreach (ICellEffect effect in effects)
            {
                Remove(effect);
            }

            _effectCells.Clear();
            _effects.Clear();
        }

        #region Effect Helpers
        /// <summary>
        /// Gets effect data from the dicronary if it exists.
        /// </summary>
        /// <param name="effect">The effect to get.</param>
        /// <param name="effectData">The effect data ssociated with the effect.</param>
        /// <returns><see langword="true"/> when the effect exists; otherwise <see langword="false"/>.</returns>
        protected bool GetKnownEffect(ICellEffect effect, out ColoredGlyphEffectData effectData)
        {
            if (_effects.ContainsKey(effect))
            {
                effectData = _effects[effect];
                return true;
            }
            else
            {
                effectData = new ColoredGlyphEffectData(effect);
                return false;
            }
        }

        /// <summary>
        /// Clears the effect for the cell specified by index.
        /// </summary>
        /// <param name="cellIndex">The cell index.</param>
        protected void ClearCellEffect(int cellIndex)
        {
            if (_effectCells.TryGetValue(cellIndex, out ColoredGlyphEffectData oldEffectData))
            {
                oldEffectData.RemoveCell(cellIndex, oldEffectData.Effect.RestoreCellOnRemoved & oldEffectData.Effect.IsFinished);
                _effectCells.Remove(cellIndex);

                if (oldEffectData.CellsStates.Count == 0)
                {
                    _effects.Remove(oldEffectData.Effect);
                }

                _backingSurface.IsDirty = true;
            }
        }

        /// <summary>
        /// Updates all known effects and applies them to their associated cells.
        /// </summary>
        /// <param name="timeElapsed">The time elapased since the last update.</param>
        public void UpdateEffects(double timeElapsed)
        {
            List<ICellEffect> effectsToRemove = new List<ICellEffect>();

            foreach (ColoredGlyphEffectData effectData in _effects.Values)
            {
                List<ColoredGlyphWithState> cellsToRemove = new List<ColoredGlyphWithState>();
                effectData.Effect.Update(timeElapsed);

                foreach (ColoredGlyphWithState cellState in effectData.CellsStates)
                {
                    if (effectData.Effect.ApplyToCell(cellState.Cell, cellState.State))
                        _backingSurface.IsDirty = true;

                    if (effectData.Effect.IsFinished && effectData.Effect.RemoveOnFinished)
                        cellsToRemove.Add(cellState);
                }

                for (int i = 0; i < cellsToRemove.Count; i++)
                {
                    effectData.RemoveCell(cellsToRemove[i].CellIndex, effectData.Effect.RestoreCellOnRemoved & effectData.Effect.IsFinished);
                    _effectCells.Remove(cellsToRemove[i].CellIndex);
                    _backingSurface.IsDirty = true;
                }

                if (effectData.CellsStates.Count == 0)
                {
                    effectsToRemove.Add(effectData.Effect);
                }
            }

            foreach (ICellEffect effect in effectsToRemove)
            {
                _effects.Remove(effect);
            }
        }
        #endregion

        ///// <summary>
        ///// Saves the effects and the associated cell indexes from the backing surface.
        ///// </summary>
        ///// <param name="file">The file to save the effects to.</param>
        //public void Save(string file) => EffectsManagerSerialized.Save(this, _backingSurface, file);

        ///// <summary>
        ///// Loads effects from a file.
        ///// </summary>
        ///// <param name="file">The file to load from.</param>
        ///// <param name="backingSurface">The surface the effects were originally (or will be) associated with.</param>
        ///// <returns></returns>
        //public static EffectsManager Load(string file, CellSurface backingSurface) => EffectsManagerSerialized.Load(file, backingSurface);



        /// <summary>
        /// Represents a mapping of a single effect and associated cells.
        /// </summary>
        protected class ColoredGlyphEffectData
        {
            /// <summary>
            /// The effect.
            /// </summary>
            public ICellEffect Effect;

            /// <summary>
            /// The cells affected by the effect.
            /// </summary>
            public List<ColoredGlyphWithState> CellsStates;

            /// <summary>
            /// Creates a new instance of the cell-effect mapping.
            /// </summary>
            /// <param name="effect">The effect.</param>
            public ColoredGlyphEffectData(ICellEffect effect)
            {
                Effect = effect;
                CellsStates = new List<ColoredGlyphWithState>();
            }

            /// <summary>
            /// Removes a cell by index from the effect data.
            /// </summary>
            /// <param name="cellIndex">The cell index.</param>
            /// <param name="restoreState">If <see langword="true"/> the cell will have its original state restored; otherwise <see langword="false"/>.</param>
            public void RemoveCell(int cellIndex, bool restoreState)
            {
                for (int i = 0; i < CellsStates.Count; i++)
                {
                    ColoredGlyphWithState cellState = CellsStates[i];
                    if (cellState.CellIndex == cellIndex)
                    {
                        ColoredGlyph cell = cellState.Cell;

                        if (restoreState)
                            cellState.State.RestoreState(ref cell);

                        CellsStates.Remove(cellState);
                        return;
                    }
                }
            }

            /// <summary>
            /// Returns <see langword="true"/> when the cell index is already associated with the effect; otherwise <see langword="false"/>.
            /// </summary>
            /// <param name="cellIndex">The cell to check.</param>
            /// <returns><see langword="true"/> to indicate the cell is associated with the effect; otherwise <see langword="false"/>.</returns>
            public bool ContainsCell(int cellIndex)
            {
                foreach (var state in CellsStates)
                {
                    if (state.CellIndex == cellIndex)
                        return true;
                }

                return false;
            }
	                
        }

        /// <summary>
        /// A glyph with its original state.
        /// </summary>
        protected class ColoredGlyphWithState
        {
            /// <summary>
            /// The cell.
            /// </summary>
            public ColoredGlyph Cell;

            /// <summary>
            /// The original state of the cell.
            /// </summary>
            public ColoredGlyphState State;

            /// <summary>
            /// The cells index.
            /// </summary>
            public int CellIndex;

            /// <summary>
            /// Creates a new instance of this class with the specified cell and index.
            /// </summary>
            /// <param name="cell">The cell to generate a state from.</param>
            /// <param name="cellIndex">The index of the cell in the parent surface.</param>
            public ColoredGlyphWithState(ColoredGlyph cell, int cellIndex) =>
                (Cell, State, CellIndex) = (cell, new ColoredGlyphState(cell), cellIndex);
        }

        /// <summary>
        /// A <see cref="ColoredGlyph"/> with state information.
        /// </summary>
        public readonly struct ColoredGlyphState
        {
            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.Decorators"/> property.
            /// </summary>
            public CellDecorator[] Decorators { get; }

            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.Foreground"/> property.
            /// </summary>
            public Color Foreground { get; }

            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.Background"/> property.
            /// </summary>
            public Color Background { get; }

            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.Glyph"/> property.
            /// </summary>
            public int Glyph { get; }

            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.Mirror"/> property.
            /// </summary>
            public Mirror Mirror { get; }

            /// <summary>
            /// A copy of the <see cref="ColoredGlyph.IsVisible"/> property.
            /// </summary>
            public bool IsVisible { get; }

            /// <summary>
            /// Creates a new state from a cell.
            /// </summary>
            /// <param name="cell">The colored glyph this state is a copy of.</param>
            public ColoredGlyphState(ColoredGlyph cell)
            {
                Foreground = cell.Foreground;
                Background = cell.Background;
                Mirror = cell.Mirror;
                Glyph = cell.Glyph;
                IsVisible = cell.IsVisible;
                Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : Array.Empty<CellDecorator>();
            }

            /// <summary>
            /// Restores this state to the specified cell.
            /// </summary>
            public void RestoreState(ref ColoredGlyph cell)
            {
                cell.Foreground = Foreground;
                cell.Background = Background;
                cell.Mirror = Mirror;
                cell.Glyph = Glyph;
                cell.IsVisible = IsVisible;
                cell.Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>();
            }
        }

        //TODO: load/save for effects manager now that it saves the cell index.

        //[DataContract]
        //internal class EffectsManagerSerialized
        //{
        //    [DataMember]
        //    private Dictionary<ICellEffect, int[]> Effects;

        //    public static void Save(EffectsManager effectsManager, CellSurface surface, string file)
        //    {
        //        EffectsManagerSerialized data = new EffectsManagerSerialized
        //        {
        //            Effects = new Dictionary<ICellEffect, int[]>(effectsManager._effects.Count)
        //        };
        //        List<ColoredGlyph> currentCells = new List<ColoredGlyph>(surface.Cells);

        //        foreach (ColoredGlyphEffectData effectData in effectsManager._effects.Values)
        //        {
        //            List<int> effectCellPositions = new List<int>(effectData.Cells.Count);

        //            foreach (ColoredGlyph cell in effectData.Cells)
        //            {
        //                effectCellPositions.Add(currentCells.IndexOf(cell));
        //            }

        //            data.Effects.Add(effectData.Effect, effectCellPositions.ToArray());
        //        }

        //        SadConsole.Serializer.Save(data, file, Settings.SerializationIsCompressed);
        //    }

        //    public static EffectsManager Load(string file, CellSurface surface)
        //    {
        //        EffectsManagerSerialized data = Serializer.Load<EffectsManagerSerialized>(file, Settings.SerializationIsCompressed);
        //        EffectsManager manager = new EffectsManager(surface);

        //        foreach (ICellEffect effect in data.Effects.Keys)
        //        {
        //            int[] effectCellIndexes = data.Effects[effect];

        //            List<ColoredGlyph> cells = new List<ColoredGlyph>(effectCellIndexes.Length);

        //            foreach (int index in effectCellIndexes)
        //            {
        //                cells.Add(surface.Cells[index]);
        //            }

        //            var effectData = new ColoredGlyphEffectData(effect) { Cells = cells };

        //            manager._effects.Add(effect, effectData);

        //            foreach (ColoredGlyph cell in cells)
        //            {
        //                manager._effectCells.Add(cell, effectData);
        //            }
        //        }

        //        return manager;
        //    }
        //}
    }
}
