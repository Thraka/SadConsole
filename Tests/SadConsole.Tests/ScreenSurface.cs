using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public partial class ScreenSurface
    {
        [TestMethod]
        public void Create_SmallSize()
        {
            new SadConsole.Tests.BasicGameHost();
            var screenSurface = new SadConsole.ScreenSurface(20, 20);
        }
    }
}
