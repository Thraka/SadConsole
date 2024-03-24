using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventMouseViewport
    {
        public uint HoveredViewportID;
    }
    public unsafe partial struct ImGuiInputEventMouseViewportPtr
    {
        public ImGuiInputEventMouseViewport* NativePtr { get; }
        public ImGuiInputEventMouseViewportPtr(ImGuiInputEventMouseViewport* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventMouseViewportPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventMouseViewport*)nativePtr;
        public static implicit operator ImGuiInputEventMouseViewportPtr(ImGuiInputEventMouseViewport* nativePtr) => new ImGuiInputEventMouseViewportPtr(nativePtr);
        public static implicit operator ImGuiInputEventMouseViewport* (ImGuiInputEventMouseViewportPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventMouseViewportPtr(IntPtr nativePtr) => new ImGuiInputEventMouseViewportPtr(nativePtr);
        public ref uint HoveredViewportID => ref Unsafe.AsRef<uint>(&NativePtr->HoveredViewportID);
    }
}
