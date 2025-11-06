using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void DropDeadCells()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            var effect = new SadConsole.Effects.Blink() { BlinkSpeed = System.TimeSpan.FromSeconds(1) };
            surface1.SetEffect(5, 5, effect);
            Assert.AreEqual(1, surface1.Effects.Count);

            surface1.Resize(30, 30, false);
            Assert.AreEqual(1, surface1.Effects.Count);
            Assert.IsNotNull(surface1.GetEffect(5, 5));

            surface1.Resize(10, 10, false);
            Assert.AreEqual(1, surface1.Effects.Count);
            Assert.IsNotNull(surface1.GetEffect(5, 5));

            surface1.Resize(5, 5, false);
            Assert.AreEqual(0, surface1.Effects.Count);

            surface1.Resize(8, 8, false);
            Assert.IsNull(surface1.GetEffect(5, 5));
        }
    }
}
