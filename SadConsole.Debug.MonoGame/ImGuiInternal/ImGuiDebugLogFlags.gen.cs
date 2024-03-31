namespace ImGuiNET.Internal
{
    [System.Flags]
    public enum ImGuiDebugLogFlags
    {
        None = 0,
        EventActiveId = 1,
        EventFocus = 2,
        EventPopup = 4,
        EventNav = 8,
        EventClipper = 16,
        EventSelection = 32,
        EventIO = 64,
        EventDocking = 128,
        EventViewport = 256,
        EventMask = 511,
        OutputToTTY = 1024,
    }
}
