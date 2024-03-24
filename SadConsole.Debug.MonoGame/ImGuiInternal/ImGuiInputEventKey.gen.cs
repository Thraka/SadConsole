using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventKey
    {
        public ImGuiKey Key;
        public byte Down;
        public float AnalogValue;
    }
    public unsafe partial struct ImGuiInputEventKeyPtr
    {
        public ImGuiInputEventKey* NativePtr { get; }
        public ImGuiInputEventKeyPtr(ImGuiInputEventKey* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventKeyPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventKey*)nativePtr;
        public static implicit operator ImGuiInputEventKeyPtr(ImGuiInputEventKey* nativePtr) => new ImGuiInputEventKeyPtr(nativePtr);
        public static implicit operator ImGuiInputEventKey* (ImGuiInputEventKeyPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventKeyPtr(IntPtr nativePtr) => new ImGuiInputEventKeyPtr(nativePtr);
        public ref ImGuiKey Key => ref Unsafe.AsRef<ImGuiKey>(&NativePtr->Key);
        public ref bool Down => ref Unsafe.AsRef<bool>(&NativePtr->Down);
        public ref float AnalogValue => ref Unsafe.AsRef<float>(&NativePtr->AnalogValue);
    }
}
