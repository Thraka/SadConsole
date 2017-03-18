using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public struct RangeIntSerialized
    {
        [DataMember]
        public int Maximum;

        [DataMember]
        public int Minimum;

        public int Get()
        {
            return Maximum == Minimum ? Maximum : Global.Random.Next(Maximum, Minimum);
        }

        public static implicit operator GameHelpers.RangeInt(RangeIntSerialized i)
        {
            return new GameHelpers.RangeInt() { Maximum = i.Maximum, Minimum = i.Minimum };
        }
        public static implicit operator RangeIntSerialized(GameHelpers.RangeInt i)
        {
            return new RangeIntSerialized() { Maximum = i.Maximum, Minimum = i.Minimum };
        }
    }

    [DataContract]
    public struct RangeDoubleSerialized
    {
        [DataMember]
        public double Maximum;

        [DataMember]
        public double Minimum;

        public static implicit operator GameHelpers.RangeDouble(RangeDoubleSerialized i)
        {
            return new GameHelpers.RangeDouble() { Maximum = i.Maximum, Minimum = i.Minimum };
        }
        public static implicit operator RangeDoubleSerialized(GameHelpers.RangeDouble i)
        {
            return new RangeDoubleSerialized() { Maximum = i.Maximum, Minimum = i.Minimum };
        }
    }
}
