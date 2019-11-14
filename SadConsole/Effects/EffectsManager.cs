
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
        protected Dictionary<ICellEffect, ColoredGlyphEffectData> _effects;
        protected Dictionary<ColoredGlyph, ColoredGlyphEffectData> _effectCells;

        protected CellSurface _backingSurface;

        /// <summary>
        /// Gets the number of effects.
        /// </summary>
        public int Count => _effects.Count;

        /// <summary>
        /// Creates a new effects manager associated with a text surface.
        /// </summary>
        /// <param name="surface">Text surface to manage.</param>
        public EffectsManager(CellSurface surface)
        {
            _effects = new Dictionary<ICellEffect, ColoredGlyphEffectData>(20);
            _effectCells = new Dictionary<ColoredGlyph, ColoredGlyphEffectData>(50);
            _backingSurface = surface;
        }

        /// <summary>
        /// Changes the effect of a specific cell.
        /// </summary>
        /// <param name="cell">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(ColoredGlyph cell, ICellEffect effect)
        {
            var list = new List<ColoredGlyph>(_backingSurface.Cells);
            if (!list.Contains(cell))
            {
                throw new Exception("Cell is not part of the surface used to create this effects manager.");
            }

            if (effect != null)
            {
                ColoredGlyphEffectData workingEffect;

                if (effect.CloneOnApply)
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
                        if (workingEffect.ContainsCell(in cell))
                        {
                            // Make sure the effect is attached to the cell.
                            return;
                        }
                    }
                }

                // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                ClearCellEffect(cell);

                // Add the cell to the effects by cell key and to list of known cells for the effect
                _effectCells.Add(cell, workingEffect);
                workingEffect.CellsStates.Add(new ColoredGlyphWithState(cell));
            }
            else
            {
                ClearCellEffect(cell);
            }
        }

        /// <summary>
        /// Changes the effect of the <paramref name="cells"/> provided.
        /// </summary>
        /// <param name="cells">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(IEnumerable<ColoredGlyph> cells, ICellEffect effect)
        {
            var list = new List<ColoredGlyph>(_backingSurface.Cells);
            foreach (ColoredGlyph cell in cells)
            {
                if (!list.Contains(cell))
                {
                    throw new Exception("Cell is not part of the surface used to create this effects manager.");
                }
            }


            if (effect != null)
            {
                ColoredGlyphEffectData workingEffect;

                if (effect.CloneOnApply)
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

                foreach (ColoredGlyph cell in cells)
                {
                    if (!workingEffect.ContainsCell(cell))
                    {
                        // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                        ClearCellEffect(cell);

                        // Add the cell to the effects by cell key and to list of known cells for the effect
                        _effectCells.Add(cell, workingEffect);
                        workingEffect.CellsStates.Add(new ColoredGlyphWithState(cell));
                    }
                }
            }
            else
            {
                foreach (ColoredGlyph cell in cells)
                {
                    ClearCellEffect(cell);
                }
            }
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(ColoredGlyph cell) => _effectCells.ContainsKey(cell) ? _effectCells[cell].Effect : null;

        public IEnumerable<ICellEffect> GetEffects()
        {
            if (_effects.Keys.Count == 0)
            {
                return null;
            }

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
                    ClearCellEffect(state.Cell);
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

        protected void ClearCellEffect(ColoredGlyph cell)
        {
            if (_effectCells.TryGetValue(cell, out ColoredGlyphEffectData oldEffectData))
            {
                oldEffectData.RemoveCell(ref cell, oldEffectData.Effect.RestoreCellOnFinished);
                _effectCells.Remove(cell);

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
                List<ColoredGlyph> cellsToRemove = new List<ColoredGlyph>();
                effectData.Effect.Update(timeElapsed);

                foreach (ColoredGlyphWithState cellState in effectData.CellsStates)
                {
                    if (effectData.Effect.ApplyToCell(cellState.Cell, cellState.State))
                        _backingSurface.IsDirty = true;

                    if (effectData.Effect.IsFinished && effectData.Effect.RemoveOnFinished)
                        cellsToRemove.Add(cellState.Cell);
                }

                for (int i = 0; i < cellsToRemove.Count; i++)
                {
                    ColoredGlyph cell = cellsToRemove[i];
                    effectData.RemoveCell(ref cell, effectData.Effect.RestoreCellOnFinished);
                    _effectCells.Remove(cell);
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
            public ICellEffect Effect;
            public List<ColoredGlyphWithState> CellsStates;

            public ColoredGlyphEffectData(ICellEffect effect)
            {
                Effect = effect;
                CellsStates = new List<ColoredGlyphWithState>();
            }

            public void RemoveCell(ref ColoredGlyph cell, bool restoreState)
            {
                for (int i = 0; i < CellsStates.Count; i++)
                {
                    ColoredGlyphWithState cellState = CellsStates[i];
                    if (cellState.Cell == cell)
                    {
                        if (restoreState)
                            cellState.State.RestoreState(ref cell);

                        CellsStates.Remove(cellState);
                        return;
                    }
                }
            }

            public bool ContainsCell(in ColoredGlyph cell)
            {
                foreach (var state in CellsStates)
                {
                    if (state.Cell == cell)
                        return true;
                }

                return false;
            }
	                
        }

        protected class ColoredGlyphWithState
        {
            public ColoredGlyph Cell;
            public ColoredGlyphState State;

            public ColoredGlyphWithState(ColoredGlyph cell) =>
                (Cell, State) = (cell, new ColoredGlyphState(cell));
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
            /// Restores the state of this cell from the <see cref="State"/> property.
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

        //TODO: this doesn't really work because we don't save the cell positions for hydration.. 

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
