using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiPopupData
    {
        public uint PopupId;
        public ImGuiWindow* Window;
        public ImGuiWindow* BackupNavWindow;
        public int ParentNavLayer;
        public int OpenFrameCount;
        public uint OpenParentId;
        public Vector2 OpenPopupPos;
        public Vector2 OpenMousePos;
    }
    public unsafe partial struct ImGuiPopupDataPtr
    {
        public ImGuiPopupData* NativePtr { get; }
        public ImGuiPopupDataPtr(ImGuiPopupData* nativePtr) => NativePtr = nativePtr;
        public ImGuiPopupDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiPopupData*)nativePtr;
        public static implicit operator ImGuiPopupDataPtr(ImGuiPopupData* nativePtr) => new ImGuiPopupDataPtr(nativePtr);
        public static implicit operator ImGuiPopupData* (ImGuiPopupDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiPopupDataPtr(IntPtr nativePtr) => new ImGuiPopupDataPtr(nativePtr);
        public ref uint PopupId => ref Unsafe.AsRef<uint>(&NativePtr->PopupId);
        public ImGuiWindowPtr Window => new ImGuiWindowPtr(NativePtr->Window);
        public ImGuiWindowPtr BackupNavWindow => new ImGuiWindowPtr(NativePtr->BackupNavWindow);
        public ref int ParentNavLayer => ref Unsafe.AsRef<int>(&NativePtr->ParentNavLayer);
        public ref int OpenFrameCount => ref Unsafe.AsRef<int>(&NativePtr->OpenFrameCount);
        public ref uint OpenParentId => ref Unsafe.AsRef<uint>(&NativePtr->OpenParentId);
        public ref Vector2 OpenPopupPos => ref Unsafe.AsRef<Vector2>(&NativePtr->OpenPopupPos);
        public ref Vector2 OpenMousePos => ref Unsafe.AsRef<Vector2>(&NativePtr->OpenMousePos);
        public void Destroy()
        {
            ImGuiNative.ImGuiPopupData_destroy((ImGuiPopupData*)(NativePtr));
        }
    }
}
