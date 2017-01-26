using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Effects
{
    /// <summary>
    /// Effects manager for a text surface.
    /// </summary>
    public class EffectsManager
    {
        protected Dictionary<ICellEffect, CellEffectData> _effects;
        protected Dictionary<Cell, CellEffectData> _effectCells;

        protected ISurface backingSurface;

        /// <summary>
        /// Creates a new effects manager associated with a text surface.
        /// </summary>
        /// <param name="surface">Text surface to manage.</param>
        public EffectsManager(ISurface surface)
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
                throw new Exception("Cell is not part of the surface used to create this effects manager.");

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
                        _effects.Add(workingEffect.Effect, workingEffect);
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
            }
            else
                ClearCellEffect(cell);
        }

        /// <summary>
        /// Changes the effect of the <paramref name="cells"/> provided.
        /// </summary>
        /// <param name="cells">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(IEnumerable<Cell> cells, ICellEffect effect)
        {
            var list = new List<Cell>(backingSurface.Cells);
            foreach (var cell in cells)
            {
                if (!list.Contains(cell))
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
                        _effects.Add(workingEffect.Effect, workingEffect);
                }

                foreach (var cell in cells)
                {
                    if (!workingEffect.Cells.Contains(cell))
                    {
                        // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                        ClearCellEffect(cell);

                        // Add the cell to the effects by cell key and to list of known cells for the effect
                        _effectCells.Add(cell, workingEffect);
                        workingEffect.Cells.Add(cell);
                    }
                }
            }
            else
            {
                foreach (var cell in cells)
                    ClearCellEffect(cell);
            }
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(Cell cell)
        {
            return _effectCells.ContainsKey(cell) ? _effectCells[cell].Effect : null;
        }

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
            CellEffectData data = _effects[effect];

            foreach (var cell in data.Cells)
                ClearCellEffect(cell);
        }

        /// <summary>
        /// Removes all effects and associated cells.
        /// </summary>
        public void RemoveAll()
        {
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
            CellEffectData oldEffectData;

            if (_effectCells.TryGetValue(cell, out oldEffectData))
            {
                oldEffectData.Effect.Clear(cell);
                oldEffectData.Cells.Remove(cell);
                _effectCells.Remove(cell);

                if (oldEffectData.Cells.Count == 0)
                    _effects.Remove(oldEffectData.Effect);

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

            foreach (var effectData in _effects.Values)
            {
                List<Cell> cellsToRemove = new List<Cell>();
                effectData.Effect.Update(timeElapsed);

                foreach (var cell in effectData.Cells)
                {
                    if (effectData.Effect.Apply(cell))
                        backingSurface.IsDirty = true;

                    if (effectData.Effect.IsFinished && effectData.Effect.RemoveOnFinished)
                        cellsToRemove.Add(cell);
                }

                foreach (var cell in cellsToRemove)
                {
                    effectData.Effect.Clear(cell);
                    effectData.Cells.Remove(cell);
                    _effectCells.Remove(cell);
                    backingSurface.IsDirty = true;
                }

                if (effectData.Cells.Count == 0)
                    effectsToRemove.Add(effectData.Effect);
            }

            foreach (var effect in effectsToRemove)
                _effects.Remove(effect);
        }
        #endregion

        /// <summary>
        /// Saves the effects and the associated cell indexes from the backing surface.
        /// </summary>
        /// <param name="file">The file to save the effects to.</param>
        public void Save(string file)
        {
            EffectsManagerSerialized.Save(this, backingSurface, file);
        }

        /// <summary>
        /// Loads effects from a file.
        /// </summary>
        /// <param name="file">The file to load from.</param>
        /// <param name="backingSurface">The surface the effects were originally (or will be) associated with.</param>
        /// <returns></returns>
        public static EffectsManager Load(string file, ISurface backingSurface)
        {
            return EffectsManagerSerialized.Load(file, backingSurface);
        }



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
            Dictionary<ICellEffect, int[]> Effects;


            public static void Save(EffectsManager effectsManager, ISurface surface, string file)
            {
                EffectsManagerSerialized data = new EffectsManagerSerialized();

                data.Effects = new Dictionary<ICellEffect, int[]>(effectsManager._effects.Count);
                List<Cell> currentCells = new List<Cell>(surface.Cells);

                foreach (var effectData in effectsManager._effects.Values)
                {
                    List<int> effectCellPositions = new List<int>(effectData.Cells.Count);

                    foreach (var cell in effectData.Cells)
                        effectCellPositions.Add(currentCells.IndexOf(cell));

                    data.Effects.Add(effectData.Effect, effectCellPositions.ToArray());
                }

                SadConsole.Serializer.Save(data, file);
            }

            public static EffectsManager Load(string file, ISurface surface)
            {
                EffectsManagerSerialized data = Serializer.Load<EffectsManagerSerialized>(file);
                EffectsManager manager = new EffectsManager(surface);

                foreach (var effect in data.Effects.Keys)
                {
                    int[] effectCellIndexes = data.Effects[effect];

                    List<Cell> cells = new List<Cell>(effectCellIndexes.Length);

                    foreach (var index in effectCellIndexes)
                        cells.Add(surface.Cells[index]);

                    var effectData = new CellEffectData(effect) { Cells = cells };

                    manager._effects.Add(effect, effectData);

                    foreach (var cell in cells)
                        manager._effectCells.Add(cell, effectData);
                }

                return manager;
            }
        }
    }
}
