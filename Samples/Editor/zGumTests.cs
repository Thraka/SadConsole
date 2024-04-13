using RenderingLibrary.Graphics;
using RenderingLibrary;
using Gum.Wireframe;
using System.Collections.ObjectModel;
using SadConsole.UI.Controls;
using SadConsole.UI;

namespace SadConsole.Editor;

public class SadConsoleGumControl : GraphicalUiElement
{
    public Func<GraphicalUiElement, ControlBase>? CreationCallBack;

    public SadConsoleGumControl(Func<GraphicalUiElement, ControlBase> createControl): base(new InvisibleRenderable(), null!)
    {
        WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
        HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;

        CreationCallBack = createControl;
    }
}

public class SadConsoleGumHost : GraphicalUiElement
{
    ControlsConsole _host;

    public SadConsoleGumHost(ControlsConsole hostConsole) : base(new InvisibleRenderable(), null!)
    {
        WrapsChildren = true;
        CanvasWidth = hostConsole.Width;
        CanvasHeight = hostConsole.Height;
        X = 0;
        Y = 0;
        Width = hostConsole.Width;
        Height = hostConsole.Height;
        _host = hostConsole;
    }

    public void Generate()
    {
        UpdateLayout();

        foreach (SadConsoleGumControl child in Children)
        {
            ControlBase control = child.CreationCallBack!(child);
            control.Position = ((int)child.X, (int)child.Y);
            _host.Controls.Add(control);
            child.CreationCallBack = null;
        }
    }
}

public class ControlBaseGumWrapper : IVisible, IRenderableIpso, IRenderable, IPositionedSizedObject, ISetClipsChildren
{
    private ObservableCollection<IRenderableIpso> children = new ObservableCollection<IRenderableIpso>();

    private float height;

    private IRenderableIpso mParent;

    public ControlBase SadControl { get; set; }

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

    public Gum.BlendState BlendState => Gum.BlendState.NonPremultiplied;

    public ObservableCollection<IRenderableIpso> Children => children;

    ColorOperation IRenderableIpso.ColorOperation => ColorOperation.Modulate;

    public bool ClipsChildren { get; set; }

    public float Height { get => SadControl.Height; set => SadControl.Resize(SadControl.Width, value == 0 ? 1 : (int)value); }

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


    public float Width { get => SadControl.Width; set => SadControl.Resize(value == 0 ? 1 : (int)value, SadControl.Height); }

    public bool Wrap => false;

    public float X { get => SadControl.Position.X; set => SadControl.Position = SadControl.Position.WithX((int)value); }

    public float Y { get => SadControl.Position.Y; set => SadControl.Position = SadControl.Position.WithY((int)value); }

    public float Z { get; set; }

    public bool FlipHorizontal { get; set; }

    IVisible IVisible.Parent => Parent;

    public ControlBaseGumWrapper(ControlBase control)
    {
        SadControl = control;
    }

    public void PreRender()
    {
    }

    public void Render(ISystemManagers managers)
    {
    }

    void IRenderableIpso.SetParentDirect(IRenderableIpso parent)
    {
        mParent = parent;

        if (parent is null) return;

        if (((GraphicalUiElement)parent).Component is ControlBaseGumWrapper wrapper)
        {
            if (wrapper.SadControl is Panel panel)
                SadControl.Parent = panel;
        }
        
    }

    public override string ToString()
    {
        return Name;
    }
}
