using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventMouseWheel
    {
        public float WheelX;
        public float WheelY;
        public ImGuiMouseSource MouseSource;
    }
    public unsafe partial struct ImGuiInputEventMouseWheelPtr
    {
        public ImGuiInputEventMouseWheel* NativePtr { get; }
        public ImGuiInputEventMouseWheelPtr(ImGuiInputEventMouseWheel* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventMouseWheelPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventMouseWheel*)nativePtr;
        public static implicit operator ImGuiInputEventMouseWheelPtr(ImGuiInputEventMouseWheel* nativePtr) => new ImGuiInputEventMouseWheelPtr(nativePtr);
        public static implicit operator ImGuiInputEventMouseWheel* (ImGuiInputEventMouseWheelPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventMouseWheelPtr(IntPtr nativePtr) => new ImGuiInputEventMouseWheelPtr(nativePtr);
        public ref float WheelX => ref Unsafe.AsRef<float>(&NativePtr->WheelX);
        public ref float WheelY => ref Unsafe.AsRef<float>(&NativePtr->WheelY);
        public ref ImGuiMouseSource MouseSource => ref Unsafe.AsRef<ImGuiMouseSource>(&NativePtr->MouseSource);
    }
}
