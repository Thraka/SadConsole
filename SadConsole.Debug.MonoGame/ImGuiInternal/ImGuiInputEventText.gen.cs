using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventText
    {
        public uint Char;
    }
    public unsafe partial struct ImGuiInputEventTextPtr
    {
        public ImGuiInputEventText* NativePtr { get; }
        public ImGuiInputEventTextPtr(ImGuiInputEventText* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventTextPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventText*)nativePtr;
        public static implicit operator ImGuiInputEventTextPtr(ImGuiInputEventText* nativePtr) => new ImGuiInputEventTextPtr(nativePtr);
        public static implicit operator ImGuiInputEventText* (ImGuiInputEventTextPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventTextPtr(IntPtr nativePtr) => new ImGuiInputEventTextPtr(nativePtr);
        public ref uint Char => ref Unsafe.AsRef<uint>(&NativePtr->Char);
    }
}
