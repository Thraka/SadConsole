
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SadConsole.Effects
{
    /// <summary>
    /// Effects manager for a text surface.
    /// </summary>
    public class EffectsManager
    {
        protected Dictionary<ICellEffect, CellEffectData> _effects;
        protected Dictionary<Cell, CellEffectData> _effectCells;

        protected CellSurface backingSurface;

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
            _effects = new Dictionary<ICellEffect, CellEffectData>(20);
            _effectCells = new Dictionary<Cell, CellEffectData>(50);
            backingSurface = surface;
        }

        /// <summary>
        /// Changes the effect of a specific cell.
        /// </summary>
        /// <param name="cell">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(Cell cell, ICellEffect effect)
        {
            var list = new List<Cell>(backingSurface.Cells);
            if (!list.Contains(cell))
            {
                throw new Exception("Cell is not part of the surface used to create this effects manager.");
            }

            if (effect != null)
            {
                CellEffectData workingEffect;

                if (effect.CloneOnApply)
                {
                    effect = effect.Clone();
                    workingEffect = new CellEffectData(effect);
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
                        if (workingEffect.Cells.Contains(cell))
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
                workingEffect.Cells.Add(cell);
                workingEffect.Effect.AddCell(cell);
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
        public void SetEffect(IEnumerable<Cell> cells, ICellEffect effect)
        {
            var list = new List<Cell>(backingSurface.Cells);
            foreach (Cell cell in cells)
            {
                if (!list.Contains(cell))
                {
                    throw new Exception("Cell is not part of the surface used to create this effects manager.");
                }
            }


            if (effect != null)
            {
                CellEffectData workingEffect;

                if (effect.CloneOnApply)
                {
                    effect = effect.Clone();
                    workingEffect = new CellEffectData(effect);
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

                foreach (Cell cell in cells)
                {
                    if (!workingEffect.Cells.Contains(cell))
                    {
                        // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                        ClearCellEffect(cell);

                        // Add the cell to the effects by cell key and to list of known cells for the effect
                        _effectCells.Add(cell, workingEffect);
                        workingEffect.Cells.Add(cell);
                        workingEffect.Effect.AddCell(cell);
                    }
                }
            }
            else
            {
                foreach (Cell cell in cells)
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
        public Effects.ICellEffect GetEffect(Cell cell) => _effectCells.ContainsKey(cell) ? _effectCells[cell].Effect : null;

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
                Cell[] cells = _effects[effect].Cells.ToArray();

                foreach (Cell cell in cells)
                {
                    ClearCellEffect(cell);
                }
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
        protected bool GetKnownEffect(ICellEffect effect, out CellEffectData effectData)
        {
            if (_effects.ContainsKey(effect))
            {
                effectData = _effects[effect];
                return true;
            }
            else
            {
                effectData = new CellEffectData(effect);
                return false;
            }
        }

        protected void ClearCellEffect(Cell cell)
        {

            if (_effectCells.TryGetValue(cell, out CellEffectData oldEffectData))
            {
                oldEffectData.Effect.ClearCell(cell);
                oldEffectData.Cells.Remove(cell);
                _effectCells.Remove(cell);

                if (oldEffectData.Cells.Count == 0)
                {
                    _effects.Remove(oldEffectData.Effect);
                }

                backingSurface.IsDirty = true;
            }

        }

        /// <summary>
        /// Updates all known effects and applies them to their associated cells.
        /// </summary>
        /// <param name="timeElapsed">The time elapased since the last update.</param>
        public void UpdateEffects(double timeElapsed)
        {
            List<ICellEffect> effectsToRemove = new List<ICellEffect>();

            foreach (CellEffectData effectData in _effects.Values)
            {
                List<Cell> cellsToRemove = new List<Cell>();
                effectData.Effect.Update(timeElapsed);

                foreach (Cell cell in effectData.Cells)
                {
                    if (effectData.Effect.UpdateCell(cell))
                    {
                        backingSurface.IsDirty = true;
                    }

                    if (effectData.Effect.IsFinished && effectData.Effect.RemoveOnFinished)
                    {
                        cellsToRemove.Add(cell);
                    }
                }

                foreach (Cell cell in cellsToRemove)
                {
                    effectData.Effect.ClearCell(cell);
                    effectData.Cells.Remove(cell);
                    _effectCells.Remove(cell);
                    backingSurface.IsDirty = true;
                }

                if (effectData.Cells.Count == 0)
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

        /// <summary>
        /// Saves the effects and the associated cell indexes from the backing surface.
        /// </summary>
        /// <param name="file">The file to save the effects to.</param>
        public void Save(string file) => EffectsManagerSerialized.Save(this, backingSurface, file);

        /// <summary>
        /// Loads effects from a file.
        /// </summary>
        /// <param name="file">The file to load from.</param>
        /// <param name="backingSurface">The surface the effects were originally (or will be) associated with.</param>
        /// <returns></returns>
        public static EffectsManager Load(string file, CellSurface backingSurface) => EffectsManagerSerialized.Load(file, backingSurface);



        /// <summary>
        /// Represents a mapping of a single effect and associated cells.
        /// </summary>
        public class CellEffectData
        {
            public ICellEffect Effect;
            public List<Cell> Cells;

            public CellEffectData(ICellEffect effect)
            {
                Effect = effect;
                Cells = new List<Cell>();
            }
        }


        [DataContract]
        internal class EffectsManagerSerialized
        {
            [DataMember]
            private Dictionary<ICellEffect, int[]> Effects;


            public static void Save(EffectsManager effectsManager, CellSurface surface, string file)
            {
                EffectsManagerSerialized data = new EffectsManagerSerialized
                {
                    Effects = new Dictionary<ICellEffect, int[]>(effectsManager._effects.Count)
                };
                List<Cell> currentCells = new List<Cell>(surface.Cells);

                foreach (CellEffectData effectData in effectsManager._effects.Values)
                {
                    List<int> effectCellPositions = new List<int>(effectData.Cells.Count);

                    foreach (Cell cell in effectData.Cells)
                    {
                        effectCellPositions.Add(currentCells.IndexOf(cell));
                    }

                    data.Effects.Add(effectData.Effect, effectCellPositions.ToArray());
                }

                SadConsole.Serializer.Save(data, file, Settings.SerializationIsCompressed);
            }

            public static EffectsManager Load(string file, CellSurface surface)
            {
                EffectsManagerSerialized data = Serializer.Load<EffectsManagerSerialized>(file, Settings.SerializationIsCompressed);
                EffectsManager manager = new EffectsManager(surface);

                foreach (ICellEffect effect in data.Effects.Keys)
                {
                    int[] effectCellIndexes = data.Effects[effect];

                    List<Cell> cells = new List<Cell>(effectCellIndexes.Length);

                    foreach (int index in effectCellIndexes)
                    {
                        cells.Add(surface.Cells[index]);
                    }

                    var effectData = new CellEffectData(effect) { Cells = cells };

                    manager._effects.Add(effect, effectData);

                    foreach (Cell cell in cells)
                    {
                        manager._effectCells.Add(cell, effectData);
                    }
                }

                return manager;
            }
        }
    }
}
