using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiLocEntry
    {
        public ImGuiLocKey Key;
        public byte* Text;
    }
    public unsafe partial struct ImGuiLocEntryPtr
    {
        public ImGuiLocEntry* NativePtr { get; }
        public ImGuiLocEntryPtr(ImGuiLocEntry* nativePtr) => NativePtr = nativePtr;
        public ImGuiLocEntryPtr(IntPtr nativePtr) => NativePtr = (ImGuiLocEntry*)nativePtr;
        public static implicit operator ImGuiLocEntryPtr(ImGuiLocEntry* nativePtr) => new ImGuiLocEntryPtr(nativePtr);
        public static implicit operator ImGuiLocEntry* (ImGuiLocEntryPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiLocEntryPtr(IntPtr nativePtr) => new ImGuiLocEntryPtr(nativePtr);
        public ref ImGuiLocKey Key => ref Unsafe.AsRef<ImGuiLocKey>(&NativePtr->Key);
        public IntPtr Text { get => (IntPtr)NativePtr->Text; set => NativePtr->Text = (byte*)value; }
    }
}
