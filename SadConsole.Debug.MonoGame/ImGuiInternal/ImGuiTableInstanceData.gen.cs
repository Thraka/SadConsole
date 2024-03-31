using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiTableInstanceData
    {
        public uint TableInstanceID;
        public float LastOuterHeight;
        public float LastFirstRowHeight;
        public float LastFrozenHeight;
    }
    public unsafe partial struct ImGuiTableInstanceDataPtr
    {
        public ImGuiTableInstanceData* NativePtr { get; }
        public ImGuiTableInstanceDataPtr(ImGuiTableInstanceData* nativePtr) => NativePtr = nativePtr;
        public ImGuiTableInstanceDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiTableInstanceData*)nativePtr;
        public static implicit operator ImGuiTableInstanceDataPtr(ImGuiTableInstanceData* nativePtr) => new ImGuiTableInstanceDataPtr(nativePtr);
        public static implicit operator ImGuiTableInstanceData* (ImGuiTableInstanceDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiTableInstanceDataPtr(IntPtr nativePtr) => new ImGuiTableInstanceDataPtr(nativePtr);
        public ref uint TableInstanceID => ref Unsafe.AsRef<uint>(&NativePtr->TableInstanceID);
        public ref float LastOuterHeight => ref Unsafe.AsRef<float>(&NativePtr->LastOuterHeight);
        public ref float LastFirstRowHeight => ref Unsafe.AsRef<float>(&NativePtr->LastFirstRowHeight);
        public ref float LastFrozenHeight => ref Unsafe.AsRef<float>(&NativePtr->LastFrozenHeight);
        public void Destroy()
        {
            ImGuiNative.ImGuiTableInstanceData_destroy((ImGuiTableInstanceData*)(NativePtr));
        }
    }
}
