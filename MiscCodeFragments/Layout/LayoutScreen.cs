using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gum;
using Gum.Wireframe;
using RenderingLibrary;
using RenderingLibrary.Graphics;

namespace SadConsole.Layout;

public class LayoutScreen: ScreenObject
{
    public LayoutScreen(GraphicalUiElement root)
    {

    }
}

public enum ObjectType
{
    Surface,
    AnimatedSurface,
    Console,
    ControlsConsole
}

public struct LayoutSettings
{
    public string Name { get; init; }
    public ObjectType ObjectType { get; init; }
}

public class LayoutElement :GraphicalUiElement
{

}

public class LayoutRenderable : IVisible, IRenderableIpso, IRenderable, IPositionedSizedObject, ISetClipsChildren
{
    private ObservableCollection<IRenderableIpso> children = new ObservableCollection<IRenderableIpso>();

    private float height;

    private IRenderableIpso mParent;

    public bool AbsoluteVisible
    {
        get
        {
            if (((IVisible)this).Parent == null)
            {
                return Visible;
            }

            return Visible && ((IVisible)this).Parent.AbsoluteVisible;
        }
    }

    public BlendState BlendState => Gum.BlendState.NonPremultiplied;

    public ObservableCollection<IRenderableIpso> Children => children;

    ColorOperation IRenderableIpso.ColorOperation => ColorOperation.Modulate;

    public bool ClipsChildren { get; set; }

    public float Height
    {
        get
        {
            return height;
        }
        set
        {
            if (float.IsPositiveInfinity(value))
            {
                throw new ArgumentException();
            }

            height = value;
        }
    }

    public string Name { get; set; }

    public IRenderableIpso Parent
    {
        get
        {
            return mParent;
        }
        set
        {
            if (mParent != value)
            {
                if (mParent != null)
                {
                    mParent.Children.Remove(this);
                }

                mParent = value;
                if (mParent != null)
                {
                    mParent.Children.Add(this);
                }
            }
        }
    }

    public float Rotation { get; set; }

    public object Tag { get; set; }

    public bool Visible { get; set; } = true;


    public float Width { get; set; }

    public bool Wrap => false;

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public bool FlipHorizontal { get; set; }

    IVisible IVisible.Parent => Parent;

    public void PreRender()
    {
    }

    public void Render(ISystemManagers managers)
    {
    }

    void IRenderableIpso.SetParentDirect(IRenderableIpso parent)
    {
        mParent = parent;
    }

    public override string ToString()
    {
        return Name;
    }
}
