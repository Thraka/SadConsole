namespace ImGuiNET.Internal
{
    [System.Flags]
    public enum ImGuiNavMoveFlags
    {
        None = 0,
        LoopX = 1,
        LoopY = 2,
        WrapX = 4,
        WrapY = 8,
        WrapMask = 15,
        AllowCurrentNavId = 16,
        AlsoScoreVisibleSet = 32,
        ScrollToEdgeY = 64,
        Forwarded = 128,
        DebugNoResult = 256,
        FocusApi = 512,
        Tabbing = 1024,
        Activate = 2048,
        NoSelect = 4096,
        NoSetNavHighlight = 8192,
    }
}
