using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiInputTextDeactivatedState
    {
        public uint ID;
        public ImVector TextA;
    }
    public unsafe partial struct ImGuiInputTextDeactivatedStatePtr
    {
        public ImGuiInputTextDeactivatedState* NativePtr { get; }
        public ImGuiInputTextDeactivatedStatePtr(ImGuiInputTextDeactivatedState* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputTextDeactivatedStatePtr(IntPtr nativePtr) => NativePtr = (ImGuiInputTextDeactivatedState*)nativePtr;
        public static implicit operator ImGuiInputTextDeactivatedStatePtr(ImGuiInputTextDeactivatedState* nativePtr) => new ImGuiInputTextDeactivatedStatePtr(nativePtr);
        public static implicit operator ImGuiInputTextDeactivatedState* (ImGuiInputTextDeactivatedStatePtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputTextDeactivatedStatePtr(IntPtr nativePtr) => new ImGuiInputTextDeactivatedStatePtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ImVector<byte> TextA => new ImVector<byte>(NativePtr->TextA);
        public void ClearFreeMemory()
        {
            ImGuiNative.ImGuiInputTextDeactivatedState_ClearFreeMemory((ImGuiInputTextDeactivatedState*)(NativePtr));
        }
        public void Destroy()
        {
            ImGuiNative.ImGuiInputTextDeactivatedState_destroy((ImGuiInputTextDeactivatedState*)(NativePtr));
        }
    }
}
