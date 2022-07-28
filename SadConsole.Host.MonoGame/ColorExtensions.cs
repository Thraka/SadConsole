using MonoColor = Microsoft.Xna.Framework.Color;
using SadRogueColor = SadRogue.Primitives.Color;

namespace SadRogue.Primitives
{
    public static class SadRogueColorExtensions
    {
        public static MonoColor ToMonoColor(this SadRogueColor self) => new MonoColor(self.R, self.G, self.B, self.A);
        public static bool Equals(this SadRogueColor self, MonoColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}

namespace Microsoft.Xna.Framework
{
    public static class MonoColorExtensions
    {
        public static SadRogueColor ToSadRogueColor(this MonoColor self) => new SadRogueColor(self.R, self.G, self.B, self.A);
        public static bool Equals(this MonoColor self, SadRogueColor other)
            => self.R == other.R && self.G == other.G && self.B == other.B && self.A == other.A;
    }
}
