using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    public partial class ScreenObjectTests
    {
        [TestClass]
        public class Children
        {
            [TestMethod]
            public void Children_Clear()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(3);

                foreach (var item in objects)
                    Assert.AreEqual(parent, item.Parent);

                // Method to test
                parent.Children.Clear();

                foreach (var item in objects)
                {
                    Assert.IsNull(item.Parent);
                    Assert.IsFalse(parent.Children.Contains(item));
                }
            }

            [TestMethod]
            public void Children_Add()
            {
                ScreenObject parent = new();
                ScreenObject[] objects = new ScreenObject[3];

                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i] = new();

                    // Method to test
                    parent.Children.Add(objects[i]);
                }

                foreach (var child in objects)
                {
                    Assert.AreEqual(parent, child.Parent);
                    Assert.IsTrue(parent.Children.Contains(child));
                }
            }

            [TestMethod]
            public void Children_Remove()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(3);

                foreach (var child in objects)
                {
                    // Method to test
                    parent.Children.Remove(child);
                    Assert.IsFalse(parent.Children.Contains(child));
                    Assert.IsNull(child.Parent);
                }
            }

            [TestMethod]
            public void Children_Insert()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(5);
                ScreenObject insertedObject = new();

                // Method to test
                parent.Children.Insert(3, insertedObject);

                Assert.AreEqual(0, parent.Children.IndexOf(objects[0]));
                Assert.AreEqual(1, parent.Children.IndexOf(objects[1]));
                Assert.AreEqual(2, parent.Children.IndexOf(objects[2]));
                Assert.AreEqual(3, parent.Children.IndexOf(insertedObject));
                Assert.AreEqual(4, parent.Children.IndexOf(objects[3]));
                Assert.AreEqual(5, parent.Children.IndexOf(objects[4]));
            }

            [TestMethod]
            public void Children_MoveToTop()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(3);

                foreach (var item in objects)
                    Assert.AreEqual(parent, item.Parent);

                Assert.IsFalse(parent.Children.IsTop(objects[0]));

                // Method to test
                parent.Children.MoveToTop(objects[0]);

                Assert.AreEqual(0, parent.Children.IndexOf(objects[1]));
                Assert.AreEqual(1, parent.Children.IndexOf(objects[2]));
                Assert.AreEqual(2, parent.Children.IndexOf(objects[0]));

                Assert.IsTrue(parent.Children.IsTop(objects[0]));
            }

            [TestMethod]
            public void Children_MoveToBottom()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(3);

                foreach (var item in objects)
                    Assert.AreEqual(parent, item.Parent);

                Assert.IsFalse(parent.Children.IsBottom(objects[2]));

                // Method to test
                parent.Children.MoveToBottom(objects[2]);

                Assert.AreEqual(0, parent.Children.IndexOf(objects[2]));
                Assert.AreEqual(1, parent.Children.IndexOf(objects[0]));
                Assert.AreEqual(2, parent.Children.IndexOf(objects[1]));

                Assert.IsTrue(parent.Children.IsBottom(objects[2]));
            }

            [TestMethod]
            public void Children_Sort()
            {
                (ScreenObject[] objects, ScreenObject parent) = CreateObjectsAndParent(5);

                objects[0].SortOrder = 3;
                objects[1].SortOrder = 1;
                objects[2].SortOrder = 4;
                objects[3].SortOrder = 2;
                objects[4].SortOrder = 5;

                parent.Children.Add(objects[0]);
                parent.Children.Add(objects[1]);
                parent.Children.Add(objects[2]);
                parent.Children.Add(objects[3]);
                parent.Children.Add(objects[4]);

                Assert.AreEqual(objects[0], parent.Children[0]);
                Assert.AreEqual(objects[1], parent.Children[1]);
                Assert.AreEqual(objects[2], parent.Children[2]);
                Assert.AreEqual(objects[3], parent.Children[3]);
                Assert.AreEqual(objects[4], parent.Children[4]);

                // Method to test
                parent.Children.Sort(ScreenObjectComparer.Instance);

                Assert.AreEqual(objects[1], parent.Children[0]);
                Assert.AreEqual(objects[3], parent.Children[1]);
                Assert.AreEqual(objects[0], parent.Children[2]);
                Assert.AreEqual(objects[2], parent.Children[3]);
                Assert.AreEqual(objects[4], parent.Children[4]);
            }
        }
    }
}
