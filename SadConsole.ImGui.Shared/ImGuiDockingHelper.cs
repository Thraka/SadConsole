using System;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public class DockBuilder
{
    private readonly DockBuilder? _parent;
    private uint _parentId;
    private DockBuilder? _left;
    private DockBuilder? _right;
    private DockBuilder? _top;
    private DockBuilder? _bottom;

    public DockBuilder(uint parentId, DockBuilder? parent = null)
    {
        _parentId = parentId;
        _parent = parent;
    }

    public DockBuilder Window(string name)
    {
        if (_parent == null)
        {
            ImGuiP.DockBuilderAddNode(_parentId, (ImGuiDockNodeFlags)ImGuiDockNodeFlagsPrivate.Space);
            ImGuiP.DockBuilderSetNodeSize(_parentId, ImGui.GetMainViewport().Size);
        }

        ImGuiP.DockBuilderDockWindow(name, _parentId);

        return this;
    }

    public unsafe DockBuilder DockLeft(float size)
    {
        uint dockId;
        fixed (uint* parentIdPtr = &_parentId)
        {
            ImGuiP.DockBuilderSplitNode(_parentId, ImGuiDir.Left, size, &dockId, parentIdPtr);
        }

        _left = new DockBuilder(dockId, this);
        return _left;
    }

    public unsafe DockBuilder DockRight(float size)
    {
        uint dockId;
        fixed (uint* parentIdPtr = &_parentId)
        {
            ImGuiP.DockBuilderSplitNode(_parentId, ImGuiDir.Right, size, &dockId, parentIdPtr);
        }

        _right = new DockBuilder(dockId, this);
        return _right;
    }

    public unsafe DockBuilder DockTop(float size)
    {
        uint dockId;
        fixed (uint* parentIdPtr = &_parentId)
        {
            ImGuiP.DockBuilderSplitNode(_parentId, ImGuiDir.Up, size, &dockId, parentIdPtr);
        }

        _top = new DockBuilder(dockId, this);
        return _top;
    }

    public unsafe DockBuilder DockBottom(float size)
    {
        uint dockId;
        fixed (uint* parentIdPtr = &_parentId)
        {
            ImGuiP.DockBuilderSplitNode(_parentId, ImGuiDir.Down, size, &dockId, parentIdPtr);
        }

        _bottom = new DockBuilder(dockId, this);
        return _bottom;
    }

    public DockBuilder Done()
    {
        if (_parent == null)
            throw new InvalidOperationException("DockBuilder has no parent.");

        return _parent;
    }

    private int NumOfNodes()
    {
        int count = 1; // count self

        if (_left != null)
            count++;
        if (_right != null)
            count++;
        if (_top != null)
            count++;
        if (_bottom != null)
            count++;

        return count;
    }
}
