using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiKeyRoutingTable
    {
        public fixed short Index[140];
        public ImVector Entries;
        public ImVector EntriesNext;
    }
    public unsafe partial struct ImGuiKeyRoutingTablePtr
    {
        public ImGuiKeyRoutingTable* NativePtr { get; }
        public ImGuiKeyRoutingTablePtr(ImGuiKeyRoutingTable* nativePtr) => NativePtr = nativePtr;
        public ImGuiKeyRoutingTablePtr(IntPtr nativePtr) => NativePtr = (ImGuiKeyRoutingTable*)nativePtr;
        public static implicit operator ImGuiKeyRoutingTablePtr(ImGuiKeyRoutingTable* nativePtr) => new ImGuiKeyRoutingTablePtr(nativePtr);
        public static implicit operator ImGuiKeyRoutingTable* (ImGuiKeyRoutingTablePtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiKeyRoutingTablePtr(IntPtr nativePtr) => new ImGuiKeyRoutingTablePtr(nativePtr);
        public RangeAccessor<short> Index => new RangeAccessor<short>(NativePtr->Index, 140);
        public ImPtrVector<ImGuiKeyRoutingDataPtr> Entries => new ImPtrVector<ImGuiKeyRoutingDataPtr>(NativePtr->Entries, Unsafe.SizeOf<ImGuiKeyRoutingData>());
        public ImPtrVector<ImGuiKeyRoutingDataPtr> EntriesNext => new ImPtrVector<ImGuiKeyRoutingDataPtr>(NativePtr->EntriesNext, Unsafe.SizeOf<ImGuiKeyRoutingData>());
        public void Clear()
        {
            ImGuiNative.ImGuiKeyRoutingTable_Clear((ImGuiKeyRoutingTable*)(NativePtr));
        }
        public void Destroy()
        {
            ImGuiNative.ImGuiKeyRoutingTable_destroy((ImGuiKeyRoutingTable*)(NativePtr));
        }
    }
}
