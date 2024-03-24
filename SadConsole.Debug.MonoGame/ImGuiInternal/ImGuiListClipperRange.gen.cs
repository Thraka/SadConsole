using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public unsafe partial struct ImGuiListClipperRange
    {
        public int Min;
        public int Max;
        public byte PosToIndexConvert;
        public sbyte PosToIndexOffsetMin;
        public sbyte PosToIndexOffsetMax;
    }
    public unsafe partial struct ImGuiListClipperRangePtr
    {
        public ImGuiListClipperRange* NativePtr { get; }
        public ImGuiListClipperRangePtr(ImGuiListClipperRange* nativePtr) => NativePtr = nativePtr;
        public ImGuiListClipperRangePtr(IntPtr nativePtr) => NativePtr = (ImGuiListClipperRange*)nativePtr;
        public static implicit operator ImGuiListClipperRangePtr(ImGuiListClipperRange* nativePtr) => new ImGuiListClipperRangePtr(nativePtr);
        public static implicit operator ImGuiListClipperRange* (ImGuiListClipperRangePtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiListClipperRangePtr(IntPtr nativePtr) => new ImGuiListClipperRangePtr(nativePtr);
        public ref int Min => ref Unsafe.AsRef<int>(&NativePtr->Min);
        public ref int Max => ref Unsafe.AsRef<int>(&NativePtr->Max);
        public ref bool PosToIndexConvert => ref Unsafe.AsRef<bool>(&NativePtr->PosToIndexConvert);
        public ref sbyte PosToIndexOffsetMin => ref Unsafe.AsRef<sbyte>(&NativePtr->PosToIndexOffsetMin);
        public ref sbyte PosToIndexOffsetMax => ref Unsafe.AsRef<sbyte>(&NativePtr->PosToIndexOffsetMax);
        public ImGuiListClipperRange FromIndices(int min, int max)
        {
            ImGuiListClipperRange ret = ImGuiNative.ImGuiListClipperRange_FromIndices(min, max);
            return ret;
        }
        public ImGuiListClipperRange FromPositions(float y1, float y2, int off_min, int off_max)
        {
            ImGuiListClipperRange ret = ImGuiNative.ImGuiListClipperRange_FromPositions(y1, y2, off_min, off_max);
            return ret;
        }
    }
}
