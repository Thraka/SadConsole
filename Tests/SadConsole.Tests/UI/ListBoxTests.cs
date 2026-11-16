using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Tests.UI;

[TestClass]
public class ListBoxTests
{
    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
    }

    /// <summary>
    /// Regression test for GitHub issue #383.
    /// Removing items while scrolled to the bottom must not leave the
    /// scroll bar value above its new maximum, which made the grip draw
    /// outside of the scroll bar and throw IndexOutOfRangeException.
    /// </summary>
    [TestMethod]
    public void RemoveItems_ScrolledToBottom_DoesNotThrow()
    {
        var console = new ControlsConsole(60, 15);
        var list = new ListBox(20, 10) { Position = (1, 1) };

        for (int i = 0; i < 15; i++)
            list.Items.Add($"item {i}");

        console.Controls.Add(list);
        list.UpdateAndRedraw(System.TimeSpan.Zero);

        list.ScrollBar.Value = list.ScrollBar.MaximumValue;

        list.Items.RemoveAt(list.Items.Count - 1);
        list.Items.RemoveAt(list.Items.Count - 1);
        list.Items.RemoveAt(list.Items.Count - 1);

        Assert.IsTrue(list.ScrollBar.Value <= list.ScrollBar.MaximumValue);
    }

    [TestMethod]
    public void ScrollBar_LowerMaximum_ClampsValue()
    {
        var scrollBar = new ScrollBar(Orientation.Vertical, 8)
        {
            MaximumValue = 7,
        };
        scrollBar.Value = 7;

        int changed = 0;
        scrollBar.ValueChanged += (s, e) => changed++;

        scrollBar.MaximumValue = 5;

        Assert.AreEqual(5, scrollBar.Value);
        Assert.AreEqual(1, changed);
    }
}
