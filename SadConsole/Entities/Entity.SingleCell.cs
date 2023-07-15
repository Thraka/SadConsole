using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole.Entities;

public partial class Entity
{
    /// <summary>
    /// An entity that is a single cell.
    /// </summary>
    [DataContract]
    public class SingleCell
    {
        private ColoredGlyph _glyph;

        [DataMember(Name = "Effect")]
        private ICellEffect? _effect;

        [DataMember(Name = "Appearance")]
        private ColoredGlyphState _effectState;

        /// <summary>
        /// When <see langword="true"/>, indicates that this cell is dirty and needs to be redrawn.
        /// </summary>
        public bool IsDirty { get => _glyph.IsDirty; set => _glyph.IsDirty = value; }

        /// <summary>
        /// Represents what the entity looks like.
        /// </summary>
        public ColoredGlyph Appearance
        {
            get => _glyph;

            [MemberNotNull(nameof(_glyph))]
            protected set
            {
                _glyph = value ?? throw new System.NullReferenceException("Appearance cannot be null.");
                _effectState = new ColoredGlyphState(_glyph);

                IsDirty = true;
            }
        }


        /// <summary>
        /// An effect that can be applied to the <see cref="Appearance"/>.
        /// </summary>
        public ICellEffect? Effect
        {
            get => _effect;
            set
            {
                // Same effect
                if (_effect == value && null != value) return;

                // Previously had an effect
                if (_effect != null)
                {
                    if (_effect.RestoreCellOnRemoved)
                        _effectState.RestoreState(ref _glyph);
                }

                // Removing the effect; reset the appearance
                if (value == null)
                {
                    _effectState = new ColoredGlyphState(_glyph);
                    _effect = null;
                }

                // Adding a new effect
                else
                {
                    _effectState = new ColoredGlyphState(_glyph);
                    _effect = value.CloneOnAdd ? value.Clone() : value;
                }
            }
        }

        /// <summary>
        /// Creates a new entity with the specified foreground, background, and glyph.
        /// </summary>
        /// <param name="foreground">The foreground color of the entity.</param>
        /// <param name="background">The background color of the entity.</param>
        /// <param name="glyph">The glyph color of the entity.</param>
        public SingleCell(Color foreground, Color background, int glyph)
        {
            Appearance = new ColoredGlyph(foreground, background, glyph);
        }

        [JsonConstructor]
        private SingleCell(ColoredGlyphState appearance, ICellEffect effect)
        {
            Appearance = new ColoredGlyph(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror, appearance.IsVisible, appearance.Decorators);
            Effect = effect;
        }

        /// <summary>
        /// Creates a new entity, references the provided glyph as the appearance.
        /// </summary>
        /// <param name="appearance">The appearance of the entity.</param>
        public SingleCell(ColoredGlyph appearance)
        {
            Appearance = appearance;
        }

        /// <summary>
        /// If an effect is applied to the cell, updates the effect.
        /// </summary>
        /// <param name="delta"></param>
        public void Update(TimeSpan delta)
        {
            if (_effect != null && !_effect.IsFinished)
            {
                _effect.Update(delta);
                _effect.ApplyToCell(Appearance, _effectState);

                if (_effect.IsFinished)
                {
                    if (_effect.RemoveOnFinished)
                    {
                        Effect = null;
                    }
                }
            }
        }
    }
}
