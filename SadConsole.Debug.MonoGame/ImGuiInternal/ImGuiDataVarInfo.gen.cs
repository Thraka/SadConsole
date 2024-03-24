using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiDataVarInfo
    {
        public ImGuiDataType Type;
        public uint Count;
        public uint Offset;
    }
    public unsafe partial struct ImGuiDataVarInfoPtr
    {
        public ImGuiDataVarInfo* NativePtr { get; }
        public ImGuiDataVarInfoPtr(ImGuiDataVarInfo* nativePtr) => NativePtr = nativePtr;
        public ImGuiDataVarInfoPtr(IntPtr nativePtr) => NativePtr = (ImGuiDataVarInfo*)nativePtr;
        public static implicit operator ImGuiDataVarInfoPtr(ImGuiDataVarInfo* nativePtr) => new ImGuiDataVarInfoPtr(nativePtr);
        public static implicit operator ImGuiDataVarInfo* (ImGuiDataVarInfoPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiDataVarInfoPtr(IntPtr nativePtr) => new ImGuiDataVarInfoPtr(nativePtr);
        public ref ImGuiDataType Type => ref Unsafe.AsRef<ImGuiDataType>(&NativePtr->Type);
        public ref uint Count => ref Unsafe.AsRef<uint>(&NativePtr->Count);
        public ref uint Offset => ref Unsafe.AsRef<uint>(&NativePtr->Offset);
        public IntPtr GetVarPtr(IntPtr parent)
        {
            void* native_parent = (void*)parent.ToPointer();
            void* ret = ImGuiNative.ImGuiDataVarInfo_GetVarPtr((ImGuiDataVarInfo*)(NativePtr), native_parent);
            return (IntPtr)ret;
        }
    }
}
