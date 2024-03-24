namespace ImGuiNET.Internal
{
    [System.Flags]
    public enum ImGuiInputFlags
    {
        None = 0,
        Repeat = 1,
        RepeatRateDefault = 2,
        RepeatRateNavMove = 4,
        RepeatRateNavTweak = 8,
        RepeatRateMask_ = 14,
        CondHovered = 16,
        CondActive = 32,
        CondDefault = 48,
        CondMask = 48,
        LockThisFrame = 64,
        LockUntilRelease = 128,
        RouteFocused = 256,
        RouteGlobalLow = 512,
        RouteGlobal = 1024,
        RouteGlobalHigh = 2048,
        RouteMask = 3840,
        RouteAlways = 4096,
        RouteUnlessBgFocused = 8192,
        RouteExtraMask = 12288,
        SupportedByIsKeyPressed = 15,
        SupportedByShortcut = 16143,
        SupportedBySetKeyOwner = 192,
        SupportedBySetItemKeyOwner = 240,
    }
}
