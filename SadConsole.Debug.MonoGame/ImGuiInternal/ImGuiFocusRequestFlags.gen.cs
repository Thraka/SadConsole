namespace ImGuiNET.Internal
{
    [System.Flags]
    public enum ImGuiFocusRequestFlags
    {
        None = 0,
        RestoreFocusedChild = 1,
        UnlessBelowModal = 2,
    }
}
