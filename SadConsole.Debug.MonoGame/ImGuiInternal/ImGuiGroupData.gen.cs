using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiGroupData
    {
        public uint WindowID;
        public Vector2 BackupCursorPos;
        public Vector2 BackupCursorMaxPos;
        public float BackupIndent;
        public float BackupGroupOffset;
        public Vector2 BackupCurrLineSize;
        public float BackupCurrLineTextBaseOffset;
        public uint BackupActiveIdIsAlive;
        public byte BackupActiveIdPreviousFrameIsAlive;
        public byte BackupHoveredIdIsAlive;
        public byte EmitItem;
    }
    public unsafe partial struct ImGuiGroupDataPtr
    {
        public ImGuiGroupData* NativePtr { get; }
        public ImGuiGroupDataPtr(ImGuiGroupData* nativePtr) => NativePtr = nativePtr;
        public ImGuiGroupDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiGroupData*)nativePtr;
        public static implicit operator ImGuiGroupDataPtr(ImGuiGroupData* nativePtr) => new ImGuiGroupDataPtr(nativePtr);
        public static implicit operator ImGuiGroupData* (ImGuiGroupDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiGroupDataPtr(IntPtr nativePtr) => new ImGuiGroupDataPtr(nativePtr);
        public ref uint WindowID => ref Unsafe.AsRef<uint>(&NativePtr->WindowID);
        public ref Vector2 BackupCursorPos => ref Unsafe.AsRef<Vector2>(&NativePtr->BackupCursorPos);
        public ref Vector2 BackupCursorMaxPos => ref Unsafe.AsRef<Vector2>(&NativePtr->BackupCursorMaxPos);
        public ref float BackupIndent => ref Unsafe.AsRef<float>(&NativePtr->BackupIndent);
        public ref float BackupGroupOffset => ref Unsafe.AsRef<float>(&NativePtr->BackupGroupOffset);
        public ref Vector2 BackupCurrLineSize => ref Unsafe.AsRef<Vector2>(&NativePtr->BackupCurrLineSize);
        public ref float BackupCurrLineTextBaseOffset => ref Unsafe.AsRef<float>(&NativePtr->BackupCurrLineTextBaseOffset);
        public ref uint BackupActiveIdIsAlive => ref Unsafe.AsRef<uint>(&NativePtr->BackupActiveIdIsAlive);
        public ref bool BackupActiveIdPreviousFrameIsAlive => ref Unsafe.AsRef<bool>(&NativePtr->BackupActiveIdPreviousFrameIsAlive);
        public ref bool BackupHoveredIdIsAlive => ref Unsafe.AsRef<bool>(&NativePtr->BackupHoveredIdIsAlive);
        public ref bool EmitItem => ref Unsafe.AsRef<bool>(&NativePtr->EmitItem);
    }
}
