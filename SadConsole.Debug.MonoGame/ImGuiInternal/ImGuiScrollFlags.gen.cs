namespace ImGuiNET.Internal
{
    [System.Flags]
    public enum ImGuiScrollFlags
    {
        None = 0,
        KeepVisibleEdgeX = 1,
        KeepVisibleEdgeY = 2,
        KeepVisibleCenterX = 4,
        KeepVisibleCenterY = 8,
        AlwaysCenterX = 16,
        AlwaysCenterY = 32,
        NoScrollParent = 64,
        MaskX = 21,
        MaskY = 42,
    }
}
