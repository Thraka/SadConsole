using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public class Serialization
    {
        class Component1 : Components.LogicComponent
        {
            public string Name { get; set; }

            public override void Render(IScreenObject host, TimeSpan delta) => throw new NotImplementedException();
            public override void Update(IScreenObject host, TimeSpan delta) => throw new NotImplementedException();
        }

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

            Component1 comp1 = new Component1() { Name = "component 1" };
            Component1 comp2 = new Component1() { Name = "component 2" };

            obj.SadComponents.Add(comp1);
            obj2.SadComponents.Add(comp2);

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

            Assert.AreEqual(obj.SadComponents.Count, newObj.SadComponents.Count);
            Assert.AreEqual(obj2.SadComponents.Count, newObj.Children[0].SadComponents.Count);

            Assert.AreEqual(comp1.Name, ((Component1)newObj.SadComponents[0]).Name);
            Assert.AreEqual(comp2.Name, ((Component1)newObj.Children[0].SadComponents[0]).Name);

        }

        [TestMethod]
        public void ScreenSurface_SaveLoad()
        {
            new SadConsole.Tests.BasicGameHost();

            ScreenSurface obj = new ScreenSurface(10, 10);
            obj.Surface.FillWithRandomGarbage(255);
            obj.Position = (10, 10);
            obj.IsEnabled = false;
            obj.IsVisible = false;
            obj.UseKeyboard = true;
            obj.UseMouse = false;

            ScreenSurface obj2 = new ScreenSurface(20, 20);
            obj2.Surface.FillWithRandomGarbage(255);
            obj2.Position = (15, 2);
            obj2.IsEnabled = true;
            obj2.IsVisible = false;
            obj2.UseKeyboard = false;
            obj2.UseMouse = true;

            obj.Children.Add(obj2);

            Component1 comp1 = new Component1() { Name = "component 1" };
            Component1 comp2 = new Component1() { Name = "component 2" };

            obj.SadComponents.Add(comp1);
            obj2.SadComponents.Add(comp2);

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
                int index1 = SadConsole.GameHost.Instance.Random.Next(0, obj.Surface.Count);
                int index2 = SadConsole.GameHost.Instance.Random.Next(0, obj.Surface.Count);

                Assert.IsTrue(obj.Surface[index1].Matches(newObj.Surface[index1]));
                Assert.IsTrue(obj2.Surface[index2].Matches(((ScreenSurface)newObj.Children[0]).Surface[index2]));
            }

            Assert.IsInstanceOfType(newObj.Children[0], typeof(ScreenSurface));

            Assert.AreEqual(obj.SadComponents.Count, newObj.SadComponents.Count);
            Assert.AreEqual(obj2.SadComponents.Count, newObj.Children[0].SadComponents.Count);

            Assert.AreEqual(comp1.Name, ((Component1)newObj.SadComponents[0]).Name);
            Assert.AreEqual(comp2.Name, ((Component1)newObj.Children[0].SadComponents[0]).Name);
        }

        [TestMethod]
        public void AnimatedScreenSurface_SaveLoad()
        {
            new SadConsole.Tests.BasicGameHost();

            // CreateStatic uses
            //   AnimationDuration = 1;
            //   Repeat = true;
            //   Name = "default";

            AnimatedScreenSurface animation = AnimatedScreenSurface.CreateStatic(10, 10, 10, 0.5d);
            animation.Name = "Static Frames";
            animation.Center = (2, 2);
            animation.Save("test.file");
            AnimatedScreenSurface animation2 = AnimatedScreenSurface.Load("test.file");

            Assert.AreEqual(animation.Width, animation2.Width);
            Assert.AreEqual(animation.Height, animation2.Height);
            Assert.AreEqual(animation.Name, animation2.Name);
            Assert.AreEqual(animation.Frames.Count, animation2.Frames.Count);
            Assert.AreEqual(animation.Font?.Name, animation2.Font?.Name);
            Assert.AreEqual(animation.FontSize, animation2.FontSize);
            Assert.AreEqual(animation.Center, animation2.Center);
            Assert.AreEqual(animation.Repeat, animation2.Repeat);
            Assert.AreEqual(animation.AnimationDuration, animation2.AnimationDuration);
            Assert.AreEqual(animation.CurrentFrameIndex, animation2.CurrentFrameIndex);

            var surfaceTest = new CellSurface();
            var surface1Frames = animation.Frames.ToList();
            var surface2Frames = animation2.Frames.ToList();
            for (int i = 0; i < surface1Frames.Count; i++)
            {
                surfaceTest.Surface_Equals(surface1Frames[i], surface2Frames[i]);
            }
        }
    }
}
