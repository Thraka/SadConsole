using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiStackLevelInfo
    {
        public uint ID;
        public sbyte QueryFrameCount;
        public byte QuerySuccess;
        public ImGuiDataType DataType;
        public fixed byte Desc[57];
    }
    public unsafe partial struct ImGuiStackLevelInfoPtr
    {
        public ImGuiStackLevelInfo* NativePtr { get; }
        public ImGuiStackLevelInfoPtr(ImGuiStackLevelInfo* nativePtr) => NativePtr = nativePtr;
        public ImGuiStackLevelInfoPtr(IntPtr nativePtr) => NativePtr = (ImGuiStackLevelInfo*)nativePtr;
        public static implicit operator ImGuiStackLevelInfoPtr(ImGuiStackLevelInfo* nativePtr) => new ImGuiStackLevelInfoPtr(nativePtr);
        public static implicit operator ImGuiStackLevelInfo* (ImGuiStackLevelInfoPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiStackLevelInfoPtr(IntPtr nativePtr) => new ImGuiStackLevelInfoPtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref sbyte QueryFrameCount => ref Unsafe.AsRef<sbyte>(&NativePtr->QueryFrameCount);
        public ref bool QuerySuccess => ref Unsafe.AsRef<bool>(&NativePtr->QuerySuccess);
        public ref ImGuiDataType DataType => ref Unsafe.AsRef<ImGuiDataType>(&NativePtr->DataType);
        public RangeAccessor<byte> Desc => new RangeAccessor<byte>(NativePtr->Desc, 57);
        public void Destroy()
        {
            ImGuiNative.ImGuiStackLevelInfo_destroy((ImGuiStackLevelInfo*)(NativePtr));
        }
    }
}
