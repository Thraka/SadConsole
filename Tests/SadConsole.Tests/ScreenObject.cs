using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public partial class ScreenObjectTests
    {
        private static (ScreenObject[] ChildObjects, ScreenObject Parent) CreateObjectsAndParent(int count)
        {
            ScreenObject parent = new();
            ScreenObject[] objects = new ScreenObject[count];

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i] = new();
                parent.Children.Add(objects[i]);
            }

            return (objects, parent);
        }

        private static ScreenObject[] CreateObjects(int count) =>
            Enumerable.Range(0, count).Select(_ => new ScreenObject()).ToArray();

        [TestMethod]
        public void Parent_Set_Unset()
        {
            ScreenObject parent = new();
            ScreenObject child = new();

            child.Parent = parent;

            Assert.AreEqual(parent, child.Parent);
            Assert.IsTrue(parent.Children.Contains(child));

            child.Parent = null;

            Assert.AreNotEqual(parent, child.Parent);
            Assert.IsFalse(parent.Children.Contains(child));
        }

        [TestMethod]
        public void Positioning()
        {
            ScreenObject parent = new();
            ScreenObject child = new();
            ScreenObject child2 = new();

            child.Parent = parent;
            child2.Parent = child;

            parent.Position = (10, 10);
            child.Position = (10, 10);
            child2.Position = (10, 10);

            Assert.IsTrue(parent.AbsolutePosition == (10, 10));
            Assert.IsTrue(child.AbsolutePosition == (20, 20));
            Assert.IsTrue(child2.AbsolutePosition == (30, 30));

            child.Position = (20, 20);

            Assert.IsTrue(parent.AbsolutePosition == (10, 10));
            Assert.IsTrue(child.AbsolutePosition == (30, 30));
            Assert.IsTrue(child2.AbsolutePosition == (40, 40));
        }

        [TestMethod]
        public void Focusing()
        {
            new BasicGameHost();

            ScreenObject obj1 = new();
            ScreenObject obj2 = new();
            ScreenObject obj3 = new();

            Assert.IsNull(GameHost.Instance.FocusedScreenObjects.ScreenObject);
            obj1.IsFocused = true;
            Assert.AreEqual(obj1, GameHost.Instance.FocusedScreenObjects.ScreenObject);

            obj2.FocusedMode = FocusBehavior.Push;
            obj2.IsFocused = true;
            Assert.AreEqual(obj2, GameHost.Instance.FocusedScreenObjects.ScreenObject);

            obj3.FocusedMode = FocusBehavior.None;
            obj3.IsFocused = true;
            Assert.AreEqual(obj2, GameHost.Instance.FocusedScreenObjects.ScreenObject);

            obj2.IsFocused = false;
            Assert.AreEqual(obj1, GameHost.Instance.FocusedScreenObjects.ScreenObject);

            obj1.IsFocused = false;
            Assert.IsNull(GameHost.Instance.FocusedScreenObjects.ScreenObject);
        }
    }
}
