using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiWindowTempData
    {
        public Vector2 CursorPos;
        public Vector2 CursorPosPrevLine;
        public Vector2 CursorStartPos;
        public Vector2 CursorMaxPos;
        public Vector2 IdealMaxPos;
        public Vector2 CurrLineSize;
        public Vector2 PrevLineSize;
        public float CurrLineTextBaseOffset;
        public float PrevLineTextBaseOffset;
        public byte IsSameLine;
        public byte IsSetPos;
        public float Indent;
        public float ColumnsOffset;
        public float GroupOffset;
        public Vector2 CursorStartPosLossyness;
        public ImGuiNavLayer NavLayerCurrent;
        public short NavLayersActiveMask;
        public short NavLayersActiveMaskNext;
        public byte NavIsScrollPushableX;
        public byte NavHideHighlightOneFrame;
        public byte NavWindowHasScrollY;
        public byte MenuBarAppending;
        public Vector2 MenuBarOffset;
        public ImGuiMenuColumns MenuColumns;
        public int TreeDepth;
        public uint TreeJumpToParentOnPopMask;
        public ImVector ChildWindows;
        public ImGuiStorage* StateStorage;
        public ImGuiOldColumns* CurrentColumns;
        public int CurrentTableIdx;
        public ImGuiLayoutType LayoutType;
        public ImGuiLayoutType ParentLayoutType;
        public float ItemWidth;
        public float TextWrapPos;
        public ImVector ItemWidthStack;
        public ImVector TextWrapPosStack;
    }
    public unsafe partial struct ImGuiWindowTempDataPtr
    {
        public ImGuiWindowTempData* NativePtr { get; }
        public ImGuiWindowTempDataPtr(ImGuiWindowTempData* nativePtr) => NativePtr = nativePtr;
        public ImGuiWindowTempDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiWindowTempData*)nativePtr;
        public static implicit operator ImGuiWindowTempDataPtr(ImGuiWindowTempData* nativePtr) => new ImGuiWindowTempDataPtr(nativePtr);
        public static implicit operator ImGuiWindowTempData* (ImGuiWindowTempDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiWindowTempDataPtr(IntPtr nativePtr) => new ImGuiWindowTempDataPtr(nativePtr);
        public ref Vector2 CursorPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorPos);
        public ref Vector2 CursorPosPrevLine => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorPosPrevLine);
        public ref Vector2 CursorStartPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorStartPos);
        public ref Vector2 CursorMaxPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorMaxPos);
        public ref Vector2 IdealMaxPos => ref Unsafe.AsRef<Vector2>(&NativePtr->IdealMaxPos);
        public ref Vector2 CurrLineSize => ref Unsafe.AsRef<Vector2>(&NativePtr->CurrLineSize);
        public ref Vector2 PrevLineSize => ref Unsafe.AsRef<Vector2>(&NativePtr->PrevLineSize);
        public ref float CurrLineTextBaseOffset => ref Unsafe.AsRef<float>(&NativePtr->CurrLineTextBaseOffset);
        public ref float PrevLineTextBaseOffset => ref Unsafe.AsRef<float>(&NativePtr->PrevLineTextBaseOffset);
        public ref bool IsSameLine => ref Unsafe.AsRef<bool>(&NativePtr->IsSameLine);
        public ref bool IsSetPos => ref Unsafe.AsRef<bool>(&NativePtr->IsSetPos);
        public ref float Indent => ref Unsafe.AsRef<float>(&NativePtr->Indent);
        public ref float ColumnsOffset => ref Unsafe.AsRef<float>(&NativePtr->ColumnsOffset);
        public ref float GroupOffset => ref Unsafe.AsRef<float>(&NativePtr->GroupOffset);
        public ref Vector2 CursorStartPosLossyness => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorStartPosLossyness);
        public ref ImGuiNavLayer NavLayerCurrent => ref Unsafe.AsRef<ImGuiNavLayer>(&NativePtr->NavLayerCurrent);
        public ref short NavLayersActiveMask => ref Unsafe.AsRef<short>(&NativePtr->NavLayersActiveMask);
        public ref short NavLayersActiveMaskNext => ref Unsafe.AsRef<short>(&NativePtr->NavLayersActiveMaskNext);
        public ref bool NavIsScrollPushableX => ref Unsafe.AsRef<bool>(&NativePtr->NavIsScrollPushableX);
        public ref bool NavHideHighlightOneFrame => ref Unsafe.AsRef<bool>(&NativePtr->NavHideHighlightOneFrame);
        public ref bool NavWindowHasScrollY => ref Unsafe.AsRef<bool>(&NativePtr->NavWindowHasScrollY);
        public ref bool MenuBarAppending => ref Unsafe.AsRef<bool>(&NativePtr->MenuBarAppending);
        public ref Vector2 MenuBarOffset => ref Unsafe.AsRef<Vector2>(&NativePtr->MenuBarOffset);
        public ref ImGuiMenuColumns MenuColumns => ref Unsafe.AsRef<ImGuiMenuColumns>(&NativePtr->MenuColumns);
        public ref int TreeDepth => ref Unsafe.AsRef<int>(&NativePtr->TreeDepth);
        public ref uint TreeJumpToParentOnPopMask => ref Unsafe.AsRef<uint>(&NativePtr->TreeJumpToParentOnPopMask);
        public ImVector<ImGuiWindowPtr> ChildWindows => new ImVector<ImGuiWindowPtr>(NativePtr->ChildWindows);
        public ImGuiStoragePtr StateStorage => new ImGuiStoragePtr(NativePtr->StateStorage);
        public ImGuiOldColumnsPtr CurrentColumns => new ImGuiOldColumnsPtr(NativePtr->CurrentColumns);
        public ref int CurrentTableIdx => ref Unsafe.AsRef<int>(&NativePtr->CurrentTableIdx);
        public ref ImGuiLayoutType LayoutType => ref Unsafe.AsRef<ImGuiLayoutType>(&NativePtr->LayoutType);
        public ref ImGuiLayoutType ParentLayoutType => ref Unsafe.AsRef<ImGuiLayoutType>(&NativePtr->ParentLayoutType);
        public ref float ItemWidth => ref Unsafe.AsRef<float>(&NativePtr->ItemWidth);
        public ref float TextWrapPos => ref Unsafe.AsRef<float>(&NativePtr->TextWrapPos);
        public ImVector<float> ItemWidthStack => new ImVector<float>(NativePtr->ItemWidthStack);
        public ImVector<float> TextWrapPosStack => new ImVector<float>(NativePtr->TextWrapPosStack);
    }
}
