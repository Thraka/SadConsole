using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        public void ScreenObject_SaveLoad()
        {
            new SadConsole.Tests.BasicGameHost();
            ScreenObject obj = new ScreenObject();
            obj.Position = (10, 10);
            obj.IsEnabled = false;
            obj.IsVisible = false;
            obj.UseKeyboard = true;
            obj.UseMouse = false;

            ScreenObject obj2 = new ScreenObject();
            obj2.Position = (15, 2);
            obj2.IsEnabled = true;
            obj2.IsVisible = false;
            obj2.UseKeyboard = false;
            obj2.UseMouse = true;

            obj.Children.Add(obj2);

            SadConsole.Serializer.Save(obj, "test.file", false);
            var newObj = SadConsole.Serializer.Load<ScreenObject>("test.file", false);

            Assert.AreEqual(obj.Position, newObj.Position);
            Assert.AreEqual(obj.IsEnabled, newObj.IsEnabled);
            Assert.AreEqual(obj.IsVisible, newObj.IsVisible);
            Assert.AreEqual(obj.UseKeyboard, newObj.UseKeyboard);
            Assert.AreEqual(obj.UseMouse, newObj.UseMouse);
            Assert.AreEqual(obj.AbsolutePosition, newObj.AbsolutePosition);

            Assert.AreEqual(obj2.Position, newObj.Children[0].Position);
            Assert.AreEqual(obj2.IsEnabled, newObj.Children[0].IsEnabled);
            Assert.AreEqual(obj2.IsVisible, newObj.Children[0].IsVisible);
            Assert.AreEqual(obj2.UseKeyboard, newObj.Children[0].UseKeyboard);
            Assert.AreEqual(obj2.UseMouse, newObj.Children[0].UseMouse);
            Assert.AreEqual(obj2.AbsolutePosition, newObj.Children[0].AbsolutePosition);

            Assert.IsInstanceOfType(newObj.Children[0], typeof(ScreenObject));
        }

        [TestMethod]
        public void ScreenSurface_SaveLoad()
        {
            new SadConsole.Tests.BasicGameHost();

            ScreenSurface obj = new ScreenSurface(10, 10);
            obj.Surface.FillWithRandomGarbage();
            obj.Position = (10, 10);
            obj.IsEnabled = false;
            obj.IsVisible = false;
            obj.UseKeyboard = true;
            obj.UseMouse = false;

            ScreenSurface obj2 = new ScreenSurface(20, 20);
            obj2.Surface.FillWithRandomGarbage();
            obj2.Position = (15, 2);
            obj2.IsEnabled = true;
            obj2.IsVisible = false;
            obj2.UseKeyboard = false;
            obj2.UseMouse = true;

            obj.Children.Add(obj2);

            SadConsole.Serializer.Save(obj, "test.file", false);
            var newObj = SadConsole.Serializer.Load<ScreenSurface>("test.file", false);

            Assert.AreEqual(obj.Position, newObj.Position);
            Assert.AreEqual(obj.IsEnabled, newObj.IsEnabled);
            Assert.AreEqual(obj.IsVisible, newObj.IsVisible);
            Assert.AreEqual(obj.UseKeyboard, newObj.UseKeyboard);
            Assert.AreEqual(obj.UseMouse, newObj.UseMouse);
            Assert.AreEqual(obj.AbsolutePosition, newObj.AbsolutePosition);

            Assert.AreEqual(obj2.Position, newObj.Children[0].Position);
            Assert.AreEqual(obj2.IsEnabled, newObj.Children[0].IsEnabled);
            Assert.AreEqual(obj2.IsVisible, newObj.Children[0].IsVisible);
            Assert.AreEqual(obj2.UseKeyboard, newObj.Children[0].UseKeyboard);
            Assert.AreEqual(obj2.UseMouse, newObj.Children[0].UseMouse);
            Assert.AreEqual(obj2.AbsolutePosition, newObj.Children[0].AbsolutePosition);

            for (int i = 0; i < 10; i++)
            {
                int index1 = SadConsole.GameHost.Instance.Random.Next(0, obj.Surface.Cells.Length);
                int index2 = SadConsole.GameHost.Instance.Random.Next(0, obj.Surface.Cells.Length);

                Assert.IsTrue(obj.Surface[index1].Equals(newObj.Surface[index1]));
                Assert.IsTrue(obj2.Surface[index2].Equals(((ScreenSurface)newObj.Children[0]).Surface[index2]));
            }

            Assert.IsInstanceOfType(newObj.Children[0], typeof(ScreenSurface));
        }

        [TestMethod]
        public void AnimatedScreenSurface_SaveLoad()
        {
            new SadConsole.Tests.BasicGameHost();

            AnimatedScreenSurface animation = AnimatedScreenSurface.CreateStatic(10, 10, 10, 0.5d);
            animation.Save("test.file");
            AnimatedScreenSurface animation2 = AnimatedScreenSurface.Load("test.file");

            Assert.AreEqual(animation.Frames.Count, animation2.Frames.Count);
            Assert.AreEqual(animation.Font?.Name, animation2.Font?.Name);
            Assert.AreEqual(animation.Repeat, animation2.Repeat);
            Assert.AreEqual(animation.AnimationDuration, animation2.AnimationDuration);
            Assert.AreEqual(animation.CurrentFrameIndex, animation2.CurrentFrameIndex);
        }
    }
}
