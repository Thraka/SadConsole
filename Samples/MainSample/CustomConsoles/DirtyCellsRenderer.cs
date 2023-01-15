using System;
using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles;

internal class DirtyCellsRenderer : ScreenSurface
{
    ScreenSurface childSurface;

    public DirtyCellsRenderer() : base(80, 23)
    {
        UseMouse = true;

        if (!Settings.UnlimitedFPS)
        {
            Surface.Print(1, 1, "Settings.UnlimitedFPS must be set to true to see a difference in this example");
        }
        else
        {
            SadConsole.UI.ControlHost host = new SadConsole.UI.ControlHost();

            // Two option buttons that change the renderstep
            SadConsole.UI.Controls.RadioButton button1 = new SadConsole.UI.Controls.RadioButton(17, 1) { Text = "Normal Renderer", Position = (0, 1), IsSelected = true };
            SadConsole.UI.Controls.RadioButton button2 = new SadConsole.UI.Controls.RadioButton(17, 1) { Text = "Cell Renderer", Position = (0, 2) };

            button1.Click += NormalRenderer_Click;
            button2.Click += CellRenderer_Click;

            host.Add(button1);
            host.Add(button2);

            SadComponents.Add(host);

            // child console that moves character around really fast
            childSurface = new ScreenSurface(78, 18);
            childSurface.Surface.DefaultBackground = Color.DarkGreen;
            childSurface.Surface.Clear();
            //childSurface.Surface.FillWithRandomGarbage(255);
            childSurface.Position = (1, 4);

            Children.Add(childSurface);
            button2.InvokeClick();
        }
    }

    private void CellRenderer_Click(object sender, System.EventArgs e)
    {
        var oldRenderer = childSurface.RenderSteps.Where((s) => s is SadConsole.Renderers.SurfaceRenderStep).FirstOrDefault();

        childSurface.RenderSteps.Remove(oldRenderer);

        var step = SadConsole.Game.Instance.GetRendererStep(SadConsole.Renderers.Constants.RenderStepNames.SurfaceDirtyCells);
        childSurface.RenderSteps.Add(step);
        childSurface.IsDirty = true;
    }

    private void NormalRenderer_Click(object sender, System.EventArgs e)
    {
        var oldRenderer = childSurface.RenderSteps.Where((s) => s is SadConsole.Renderers.SurfaceDirtyCellsRenderStep).FirstOrDefault();

        childSurface.RenderSteps.Remove(oldRenderer);

        var step = SadConsole.Game.Instance.GetRendererStep(SadConsole.Renderers.Constants.RenderStepNames.Surface);
        childSurface.RenderSteps.Add(step);
        childSurface.IsDirty = true;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        if (childSurface != null)
        {
            var point = new Point(SadConsole.Game.Instance.Random.Next(0, childSurface.Surface.Width), SadConsole.Game.Instance.Random.Next(0, childSurface.Surface.Height));

            childSurface.Surface[point].Background = Color.DarkOrange.GetRandomColor(SadConsole.Game.Instance.Random);
            childSurface.IsDirty = true;
        }
    }
}
