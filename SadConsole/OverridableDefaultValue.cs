using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class OverridableDefaultValue<TType>
    {
        private TType _originalValue;

        public bool Overridden { get; private set; }

        public TType Value
        {
            get => DefaultValue;
            set { DefaultValue = value; Overridden = true; }
        }

        public TType DefaultValue { get; set; }

        public OverridableDefaultValue(TType value) =>
            _originalValue = DefaultValue = value;

        public void Reset()
        {
            DefaultValue = _originalValue;
            Overridden = false;
        }

        public void ChangeOriginalValue(TType value)
        {
            _originalValue = value;
            if (!Overridden)
                DefaultValue = _originalValue;
        }

        public static implicit operator TType(OverridableDefaultValue<TType> instance) =>
            instance.Value;
    }
}
