using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputEventMouseButton
    {
        public int Button;
        public byte Down;
        public ImGuiMouseSource MouseSource;
    }
    public unsafe partial struct ImGuiInputEventMouseButtonPtr
    {
        public ImGuiInputEventMouseButton* NativePtr { get; }
        public ImGuiInputEventMouseButtonPtr(ImGuiInputEventMouseButton* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputEventMouseButtonPtr(IntPtr nativePtr) => NativePtr = (ImGuiInputEventMouseButton*)nativePtr;
        public static implicit operator ImGuiInputEventMouseButtonPtr(ImGuiInputEventMouseButton* nativePtr) => new ImGuiInputEventMouseButtonPtr(nativePtr);
        public static implicit operator ImGuiInputEventMouseButton* (ImGuiInputEventMouseButtonPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputEventMouseButtonPtr(IntPtr nativePtr) => new ImGuiInputEventMouseButtonPtr(nativePtr);
        public ref int Button => ref Unsafe.AsRef<int>(&NativePtr->Button);
        public ref bool Down => ref Unsafe.AsRef<bool>(&NativePtr->Down);
        public ref ImGuiMouseSource MouseSource => ref Unsafe.AsRef<ImGuiMouseSource>(&NativePtr->MouseSource);
    }
}
