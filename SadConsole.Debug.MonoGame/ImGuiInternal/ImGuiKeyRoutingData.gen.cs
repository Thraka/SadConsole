using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiKeyRoutingData
    {
        public short NextEntryIndex;
        public ushort Mods;
        public byte RoutingNextScore;
        public uint RoutingCurr;
        public uint RoutingNext;
    }
    public unsafe partial struct ImGuiKeyRoutingDataPtr
    {
        public ImGuiKeyRoutingData* NativePtr { get; }
        public ImGuiKeyRoutingDataPtr(ImGuiKeyRoutingData* nativePtr) => NativePtr = nativePtr;
        public ImGuiKeyRoutingDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiKeyRoutingData*)nativePtr;
        public static implicit operator ImGuiKeyRoutingDataPtr(ImGuiKeyRoutingData* nativePtr) => new ImGuiKeyRoutingDataPtr(nativePtr);
        public static implicit operator ImGuiKeyRoutingData* (ImGuiKeyRoutingDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiKeyRoutingDataPtr(IntPtr nativePtr) => new ImGuiKeyRoutingDataPtr(nativePtr);
        public ref short NextEntryIndex => ref Unsafe.AsRef<short>(&NativePtr->NextEntryIndex);
        public ref ushort Mods => ref Unsafe.AsRef<ushort>(&NativePtr->Mods);
        public ref byte RoutingNextScore => ref Unsafe.AsRef<byte>(&NativePtr->RoutingNextScore);
        public ref uint RoutingCurr => ref Unsafe.AsRef<uint>(&NativePtr->RoutingCurr);
        public ref uint RoutingNext => ref Unsafe.AsRef<uint>(&NativePtr->RoutingNext);
        public void Destroy()
        {
            ImGuiNative.ImGuiKeyRoutingData_destroy((ImGuiKeyRoutingData*)(NativePtr));
        }
    }
}
