using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventAppFocused
    {
        public byte Focused;
    }
    public unsafe partial struct ImGuiInputEventAppFocusedPtr
    {
        public ImGuiInputEventAppFocused* NativePtr { get; }
        public ImGuiInputEventAppFocusedPtr(ImGuiInputEventAppFocused* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventAppFocusedPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventAppFocused*)nativePtr;
        public static implicit operator ImGuiInputEventAppFocusedPtr(ImGuiInputEventAppFocused* nativePtr) => new ImGuiInputEventAppFocusedPtr(nativePtr);
        public static implicit operator ImGuiInputEventAppFocused* (ImGuiInputEventAppFocusedPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventAppFocusedPtr(IntPtr nativePtr) => new ImGuiInputEventAppFocusedPtr(nativePtr);
        public ref bool Focused => ref Unsafe.AsRef<bool>(&NativePtr->Focused);
    }
}
