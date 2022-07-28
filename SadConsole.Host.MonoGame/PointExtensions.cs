using System;
using SadRogue.Primitives;
using MonoPoint = Microsoft.Xna.Framework.Point;
using SadRoguePoint = SadRogue.Primitives.Point;

namespace SadRogue.Primitives
{
    public static class SadRoguePointExtensions
    {

        public static MonoPoint ToMonoPoint(this SadRoguePoint self) => new MonoPoint(self.X, self.Y);
        public static SadRoguePoint Add(this SadRoguePoint self, MonoPoint other) => new SadRoguePoint(self.X + other.X, self.Y + other.Y);
        public static SadRoguePoint Subtract(this SadRoguePoint self, MonoPoint other) => new SadRoguePoint(self.X - other.X, self.Y - other.Y);

        public static SadRoguePoint Multiply(this SadRoguePoint self, MonoPoint other) => new SadRoguePoint(self.X * other.X, self.Y * other.Y);
        public static SadRoguePoint Divide(this SadRoguePoint self, MonoPoint other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        public static bool Equals(this SadRoguePoint self, MonoPoint other) => self.X == other.X && self.Y == other.Y;
    }
}

namespace Microsoft.Xna.Framework
{
    public static class MonoPointExtensions
    {
        public static SadRoguePoint ToPoint(this MonoPoint self) => new SadRoguePoint(self.X, self.Y);
        public static MonoPoint Add(this MonoPoint self, SadRoguePoint other) => new MonoPoint(self.X + other.X, self.Y + other.Y);
        public static MonoPoint Add(this MonoPoint self, int i) => new MonoPoint(self.X + i, self.Y + i);
        public static MonoPoint Add(this MonoPoint self, Direction dir) => new MonoPoint(self.X + dir.DeltaX, self.Y + dir.DeltaY);
        public static MonoPoint Subtract(this MonoPoint self, SadRoguePoint other) => new MonoPoint(self.X - other.X, self.Y - other.Y);
        public static MonoPoint Subtract(this MonoPoint self, int i) => new MonoPoint(self.X - i, self.Y - i);

        public static MonoPoint Multiply(this MonoPoint self, SadRoguePoint other) => new MonoPoint(self.X * other.X, self.Y * other.Y);
        public static MonoPoint Multiply(this MonoPoint self, int i) => new MonoPoint(self.X * i, self.Y * i);
        public static MonoPoint Multiply(this MonoPoint self, double d)
            => new MonoPoint((int)Math.Round(self.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        public static MonoPoint Divide(this MonoPoint self, SadRoguePoint other)
            => new MonoPoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        public static MonoPoint Divide(this MonoPoint self, double d)
            => new MonoPoint((int)Math.Round(self.X / d, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        public static bool Equals(this MonoPoint self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }
}
