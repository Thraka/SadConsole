using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiStackTool
    {
        public int LastActiveFrame;
        public int StackLevel;
        public uint QueryId;
        public ImVector Results;
        public byte CopyToClipboardOnCtrlC;
        public float CopyToClipboardLastTime;
    }
    public unsafe partial struct ImGuiStackToolPtr
    {
        public ImGuiStackTool* NativePtr { get; }
        public ImGuiStackToolPtr(ImGuiStackTool* nativePtr) => NativePtr = nativePtr;
        public ImGuiStackToolPtr(IntPtr nativePtr) => NativePtr = (ImGuiStackTool*)nativePtr;
        public static implicit operator ImGuiStackToolPtr(ImGuiStackTool* nativePtr) => new ImGuiStackToolPtr(nativePtr);
        public static implicit operator ImGuiStackTool* (ImGuiStackToolPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiStackToolPtr(IntPtr nativePtr) => new ImGuiStackToolPtr(nativePtr);
        public ref int LastActiveFrame => ref Unsafe.AsRef<int>(&NativePtr->LastActiveFrame);
        public ref int StackLevel => ref Unsafe.AsRef<int>(&NativePtr->StackLevel);
        public ref uint QueryId => ref Unsafe.AsRef<uint>(&NativePtr->QueryId);
        public ImPtrVector<ImGuiStackLevelInfoPtr> Results => new ImPtrVector<ImGuiStackLevelInfoPtr>(NativePtr->Results, Unsafe.SizeOf<ImGuiStackLevelInfo>());
        public ref bool CopyToClipboardOnCtrlC => ref Unsafe.AsRef<bool>(&NativePtr->CopyToClipboardOnCtrlC);
        public ref float CopyToClipboardLastTime => ref Unsafe.AsRef<float>(&NativePtr->CopyToClipboardLastTime);
        public void Destroy()
        {
            ImGuiNative.ImGuiStackTool_destroy((ImGuiStackTool*)(NativePtr));
        }
    }
}
