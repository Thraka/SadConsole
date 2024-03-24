using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiKeyOwnerData
    {
        public uint OwnerCurr;
        public uint OwnerNext;
        public byte LockThisFrame;
        public byte LockUntilRelease;
    }
    public unsafe partial struct ImGuiKeyOwnerDataPtr
    {
        public ImGuiKeyOwnerData* NativePtr { get; }
        public ImGuiKeyOwnerDataPtr(ImGuiKeyOwnerData* nativePtr) => NativePtr = nativePtr;
        public ImGuiKeyOwnerDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiKeyOwnerData*)nativePtr;
        public static implicit operator ImGuiKeyOwnerDataPtr(ImGuiKeyOwnerData* nativePtr) => new ImGuiKeyOwnerDataPtr(nativePtr);
        public static implicit operator ImGuiKeyOwnerData* (ImGuiKeyOwnerDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiKeyOwnerDataPtr(IntPtr nativePtr) => new ImGuiKeyOwnerDataPtr(nativePtr);
        public ref uint OwnerCurr => ref Unsafe.AsRef<uint>(&NativePtr->OwnerCurr);
        public ref uint OwnerNext => ref Unsafe.AsRef<uint>(&NativePtr->OwnerNext);
        public ref bool LockThisFrame => ref Unsafe.AsRef<bool>(&NativePtr->LockThisFrame);
        public ref bool LockUntilRelease => ref Unsafe.AsRef<bool>(&NativePtr->LockUntilRelease);
        public void Destroy()
        {
            ImGuiNative.ImGuiKeyOwnerData_destroy((ImGuiKeyOwnerData*)(NativePtr));
        }
    }
}
