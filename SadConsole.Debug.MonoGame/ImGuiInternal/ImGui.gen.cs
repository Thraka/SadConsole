using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace ImGuiNET.Internal
{
    public static unsafe partial class ImGui
    {
        public static void ActivateItemByID(uint id)
        {
            ImGuiNative.igActivateItemByID(id);
        }
        public static uint AddContextHook(IntPtr context, ImGuiContextHookPtr hook)
        {
            ImGuiContextHook* native_hook = hook.NativePtr;
            uint ret = ImGuiNative.igAddContextHook(context, native_hook);
            return ret;
        }
        public static void AddSettingsHandler(ImGuiSettingsHandlerPtr handler)
        {
            ImGuiSettingsHandler* native_handler = handler.NativePtr;
            ImGuiNative.igAddSettingsHandler(native_handler);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool ArrowButtonEx(ReadOnlySpan<char> str_id, ImGuiDir dir, Vector2 size_arg)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igArrowButtonEx(native_str_id, dir, size_arg, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret != 0;
        }
#endif
        public static bool ArrowButtonEx(string str_id, ImGuiDir dir, Vector2 size_arg)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igArrowButtonEx(native_str_id, dir, size_arg, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool ArrowButtonEx(ReadOnlySpan<char> str_id, ImGuiDir dir, Vector2 size_arg, ImGuiButtonFlags flags)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            byte ret = ImGuiNative.igArrowButtonEx(native_str_id, dir, size_arg, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret != 0;
        }
#endif
        public static bool ArrowButtonEx(string str_id, ImGuiDir dir, Vector2 size_arg, ImGuiButtonFlags flags)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            byte ret = ImGuiNative.igArrowButtonEx(native_str_id, dir, size_arg, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginChildEx(ReadOnlySpan<char> name, uint id, Vector2 size_arg, bool border, ImGuiWindowFlags flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            byte native_border = border ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igBeginChildEx(native_name, id, size_arg, native_border, flags);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginChildEx(string name, uint id, Vector2 size_arg, bool border, ImGuiWindowFlags flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            byte native_border = border ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igBeginChildEx(native_name, id, size_arg, native_border, flags);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void BeginColumns(ReadOnlySpan<char> str_id, int count)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiOldColumnFlags flags = (ImGuiOldColumnFlags)0;
            ImGuiNative.igBeginColumns(native_str_id, count, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
        }
#endif
        public static void BeginColumns(string str_id, int count)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiOldColumnFlags flags = (ImGuiOldColumnFlags)0;
            ImGuiNative.igBeginColumns(native_str_id, count, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void BeginColumns(ReadOnlySpan<char> str_id, int count, ImGuiOldColumnFlags flags)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiNative.igBeginColumns(native_str_id, count, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
        }
#endif
        public static void BeginColumns(string str_id, int count, ImGuiOldColumnFlags flags)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            ImGuiNative.igBeginColumns(native_str_id, count, flags);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
        }
        public static bool BeginComboPopup(uint popup_id, ImRect bb, ImGuiComboFlags flags)
        {
            byte ret = ImGuiNative.igBeginComboPopup(popup_id, bb, flags);
            return ret != 0;
        }
        public static bool BeginComboPreview()
        {
            byte ret = ImGuiNative.igBeginComboPreview();
            return ret != 0;
        }
        public static void BeginDockableDragDropSource(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igBeginDockableDragDropSource(native_window);
        }
        public static void BeginDockableDragDropTarget(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igBeginDockableDragDropTarget(native_window);
        }
        public static void BeginDocked(ImGuiWindowPtr window, ref bool p_open)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte native_p_open_val = p_open ? (byte)1 : (byte)0;
            byte* native_p_open = &native_p_open_val;
            ImGuiNative.igBeginDocked(native_window, native_p_open);
            p_open = native_p_open_val != 0;
        }
        public static bool BeginDragDropTargetCustom(ImRect bb, uint id)
        {
            byte ret = ImGuiNative.igBeginDragDropTargetCustom(bb, id);
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginMenuEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte enabled = 1;
            byte ret = ImGuiNative.igBeginMenuEx(native_label, native_icon, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
#endif
        public static bool BeginMenuEx(string label, string icon)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte enabled = 1;
            byte ret = ImGuiNative.igBeginMenuEx(native_label, native_icon, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginMenuEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon, bool enabled)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte native_enabled = enabled ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igBeginMenuEx(native_label, native_icon, native_enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
#endif
        public static bool BeginMenuEx(string label, string icon, bool enabled)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte native_enabled = enabled ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igBeginMenuEx(native_label, native_icon, native_enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
        public static bool BeginPopupEx(uint id, ImGuiWindowFlags extra_flags)
        {
            byte ret = ImGuiNative.igBeginPopupEx(id, extra_flags);
            return ret != 0;
        }
        public static bool BeginTabBarEx(ImGuiTabBarPtr tab_bar, ImRect bb, ImGuiTabBarFlags flags, ImGuiDockNodePtr dock_node)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiDockNode* native_dock_node = dock_node.NativePtr;
            byte ret = ImGuiNative.igBeginTabBarEx(native_tab_bar, bb, flags, native_dock_node);
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginTableEx(ReadOnlySpan<char> name, uint id, int columns_count)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiTableFlags flags = (ImGuiTableFlags)0;
            Vector2 outer_size = new Vector2();
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginTableEx(string name, uint id, int columns_count)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiTableFlags flags = (ImGuiTableFlags)0;
            Vector2 outer_size = new Vector2();
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginTableEx(ReadOnlySpan<char> name, uint id, int columns_count, ImGuiTableFlags flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            Vector2 outer_size = new Vector2();
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginTableEx(string name, uint id, int columns_count, ImGuiTableFlags flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            Vector2 outer_size = new Vector2();
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginTableEx(ReadOnlySpan<char> name, uint id, int columns_count, ImGuiTableFlags flags, Vector2 outer_size)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginTableEx(string name, uint id, int columns_count, ImGuiTableFlags flags, Vector2 outer_size)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            float inner_width = 0.0f;
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginTableEx(ReadOnlySpan<char> name, uint id, int columns_count, ImGuiTableFlags flags, Vector2 outer_size, float inner_width)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginTableEx(string name, uint id, int columns_count, ImGuiTableFlags flags, Vector2 outer_size, float inner_width)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            byte ret = ImGuiNative.igBeginTableEx(native_name, id, columns_count, flags, outer_size, inner_width);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
        public static bool BeginTooltipEx(ImGuiTooltipFlags tooltip_flags, ImGuiWindowFlags extra_window_flags)
        {
            byte ret = ImGuiNative.igBeginTooltipEx(tooltip_flags, extra_window_flags);
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool BeginViewportSideBar(ReadOnlySpan<char> name, ImGuiViewportPtr viewport, ImGuiDir dir, float size, ImGuiWindowFlags window_flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiViewport* native_viewport = viewport.NativePtr;
            byte ret = ImGuiNative.igBeginViewportSideBar(native_name, native_viewport, dir, size, window_flags);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
#endif
        public static bool BeginViewportSideBar(string name, ImGuiViewportPtr viewport, ImGuiDir dir, float size, ImGuiWindowFlags window_flags)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiViewport* native_viewport = viewport.NativePtr;
            byte ret = ImGuiNative.igBeginViewportSideBar(native_name, native_viewport, dir, size, window_flags);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return ret != 0;
        }
        public static void BringWindowToDisplayBack(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igBringWindowToDisplayBack(native_window);
        }
        public static void BringWindowToDisplayBehind(ImGuiWindowPtr window, ImGuiWindowPtr above_window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* native_above_window = above_window.NativePtr;
            ImGuiNative.igBringWindowToDisplayBehind(native_window, native_above_window);
        }
        public static void BringWindowToDisplayFront(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igBringWindowToDisplayFront(native_window);
        }
        public static void BringWindowToFocusFront(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igBringWindowToFocusFront(native_window);
        }
        public static bool ButtonBehavior(ImRect bb, uint id, ref bool out_hovered, ref bool out_held)
        {
            byte native_out_hovered_val = out_hovered ? (byte)1 : (byte)0;
            byte* native_out_hovered = &native_out_hovered_val;
            byte native_out_held_val = out_held ? (byte)1 : (byte)0;
            byte* native_out_held = &native_out_held_val;
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igButtonBehavior(bb, id, native_out_hovered, native_out_held, flags);
            out_hovered = native_out_hovered_val != 0;
            out_held = native_out_held_val != 0;
            return ret != 0;
        }
        public static bool ButtonBehavior(ImRect bb, uint id, ref bool out_hovered, ref bool out_held, ImGuiButtonFlags flags)
        {
            byte native_out_hovered_val = out_hovered ? (byte)1 : (byte)0;
            byte* native_out_hovered = &native_out_hovered_val;
            byte native_out_held_val = out_held ? (byte)1 : (byte)0;
            byte* native_out_held = &native_out_held_val;
            byte ret = ImGuiNative.igButtonBehavior(bb, id, native_out_hovered, native_out_held, flags);
            out_hovered = native_out_hovered_val != 0;
            out_held = native_out_held_val != 0;
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool ButtonEx(ReadOnlySpan<char> label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            Vector2 size_arg = new Vector2();
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#endif
        public static bool ButtonEx(string label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            Vector2 size_arg = new Vector2();
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool ButtonEx(ReadOnlySpan<char> label, Vector2 size_arg)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#endif
        public static bool ButtonEx(string label, Vector2 size_arg)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool ButtonEx(ReadOnlySpan<char> label, Vector2 size_arg, ImGuiButtonFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#endif
        public static bool ButtonEx(string label, Vector2 size_arg, ImGuiButtonFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte ret = ImGuiNative.igButtonEx(native_label, size_arg, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
        public static Vector2 CalcItemSize(Vector2 size, float default_w, float default_h)
        {
            Vector2 __retval;
            ImGuiNative.igCalcItemSize(&__retval, size, default_w, default_h);
            return __retval;
        }
        public static ImDrawFlags CalcRoundingFlagsForRectInRect(ImRect r_in, ImRect r_outer, float threshold)
        {
            ImDrawFlags ret = ImGuiNative.igCalcRoundingFlagsForRectInRect(r_in, r_outer, threshold);
            return ret;
        }
        public static int CalcTypematicRepeatAmount(float t0, float t1, float repeat_delay, float repeat_rate)
        {
            int ret = ImGuiNative.igCalcTypematicRepeatAmount(t0, t1, repeat_delay, repeat_rate);
            return ret;
        }
        public static Vector2 CalcWindowNextAutoFitSize(ImGuiWindowPtr window)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igCalcWindowNextAutoFitSize(&__retval, native_window);
            return __retval;
        }
        public static float CalcWrapWidthForPos(Vector2 pos, float wrap_pos_x)
        {
            float ret = ImGuiNative.igCalcWrapWidthForPos(pos, wrap_pos_x);
            return ret;
        }
        public static void CallContextHooks(IntPtr context, ImGuiContextHookType type)
        {
            ImGuiNative.igCallContextHooks(context, type);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool CheckboxFlags(ReadOnlySpan<char> label, ref long flags, long flags_value)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (long* native_flags = &flags)
            {
                byte ret = ImGuiNative.igCheckboxFlags_S64Ptr(native_label, native_flags, flags_value);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
                return ret != 0;
            }
        }
#endif
        public static bool CheckboxFlags(string label, ref long flags, long flags_value)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (long* native_flags = &flags)
            {
                byte ret = ImGuiNative.igCheckboxFlags_S64Ptr(native_label, native_flags, flags_value);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
                return ret != 0;
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool CheckboxFlags(ReadOnlySpan<char> label, ref ulong flags, ulong flags_value)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (ulong* native_flags = &flags)
            {
                byte ret = ImGuiNative.igCheckboxFlags_U64Ptr(native_label, native_flags, flags_value);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
                return ret != 0;
            }
        }
#endif
        public static bool CheckboxFlags(string label, ref ulong flags, ulong flags_value)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (ulong* native_flags = &flags)
            {
                byte ret = ImGuiNative.igCheckboxFlags_U64Ptr(native_label, native_flags, flags_value);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
                return ret != 0;
            }
        }
        public static void ClearActiveID()
        {
            ImGuiNative.igClearActiveID();
        }
        public static void ClearDragDrop()
        {
            ImGuiNative.igClearDragDrop();
        }
        public static void ClearIniSettings()
        {
            ImGuiNative.igClearIniSettings();
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ClearWindowSettings(ReadOnlySpan<char> name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiNative.igClearWindowSettings(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
        }
#endif
        public static void ClearWindowSettings(string name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiNative.igClearWindowSettings(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
        }
        public static bool CloseButton(uint id, Vector2 pos)
        {
            byte ret = ImGuiNative.igCloseButton(id, pos);
            return ret != 0;
        }
        public static void ClosePopupsExceptModals()
        {
            ImGuiNative.igClosePopupsExceptModals();
        }
        public static void ClosePopupsOverWindow(ImGuiWindowPtr ref_window, bool restore_focus_to_window_under_popup)
        {
            ImGuiWindow* native_ref_window = ref_window.NativePtr;
            byte native_restore_focus_to_window_under_popup = restore_focus_to_window_under_popup ? (byte)1 : (byte)0;
            ImGuiNative.igClosePopupsOverWindow(native_ref_window, native_restore_focus_to_window_under_popup);
        }
        public static void ClosePopupToLevel(int remaining, bool restore_focus_to_window_under_popup)
        {
            byte native_restore_focus_to_window_under_popup = restore_focus_to_window_under_popup ? (byte)1 : (byte)0;
            ImGuiNative.igClosePopupToLevel(remaining, native_restore_focus_to_window_under_popup);
        }
        public static bool CollapseButton(uint id, Vector2 pos, ImGuiDockNodePtr dock_node)
        {
            ImGuiDockNode* native_dock_node = dock_node.NativePtr;
            byte ret = ImGuiNative.igCollapseButton(id, pos, native_dock_node);
            return ret != 0;
        }
        public static void ColorEditOptionsPopup(ref float col, ImGuiColorEditFlags flags)
        {
            fixed (float* native_col = &col)
            {
                ImGuiNative.igColorEditOptionsPopup(native_col, flags);
            }
        }
        public static void ColorPickerOptionsPopup(ref float ref_col, ImGuiColorEditFlags flags)
        {
            fixed (float* native_ref_col = &ref_col)
            {
                ImGuiNative.igColorPickerOptionsPopup(native_ref_col, flags);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ColorTooltip(ReadOnlySpan<char> text, ref float col, ImGuiColorEditFlags flags)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (float* native_col = &col)
            {
                ImGuiNative.igColorTooltip(native_text, native_col, flags);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void ColorTooltip(string text, ref float col, ImGuiColorEditFlags flags)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (float* native_col = &col)
            {
                ImGuiNative.igColorTooltip(native_text, native_col, flags);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
        public static ImGuiKey ConvertShortcutMod(ImGuiKey key_chord)
        {
            ImGuiKey ret = ImGuiNative.igConvertShortcutMod(key_chord);
            return ret;
        }
        public static ImGuiKey ConvertSingleModFlagToKey(IntPtr ctx, ImGuiKey key)
        {
            ImGuiKey ret = ImGuiNative.igConvertSingleModFlagToKey(ctx, key);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static ImGuiWindowSettingsPtr CreateNewWindowSettings(ReadOnlySpan<char> name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiWindowSettings* ret = ImGuiNative.igCreateNewWindowSettings(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return new ImGuiWindowSettingsPtr(ret);
        }
#endif
        public static ImGuiWindowSettingsPtr CreateNewWindowSettings(string name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiWindowSettings* ret = ImGuiNative.igCreateNewWindowSettings(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return new ImGuiWindowSettingsPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool DataTypeApplyFromText(ReadOnlySpan<char> buf, ImGuiDataType data_type, IntPtr p_data, ReadOnlySpan<char> format)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte ret = ImGuiNative.igDataTypeApplyFromText(native_buf, data_type, native_p_data, native_format);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool DataTypeApplyFromText(string buf, ImGuiDataType data_type, IntPtr p_data, string format)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte ret = ImGuiNative.igDataTypeApplyFromText(native_buf, data_type, native_p_data, native_format);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
        public static void DataTypeApplyOp(ImGuiDataType data_type, int op, IntPtr output, IntPtr arg_1, IntPtr arg_2)
        {
            void* native_output = (void*)output.ToPointer();
            void* native_arg_1 = (void*)arg_1.ToPointer();
            void* native_arg_2 = (void*)arg_2.ToPointer();
            ImGuiNative.igDataTypeApplyOp(data_type, op, native_output, native_arg_1, native_arg_2);
        }
        public static bool DataTypeClamp(ImGuiDataType data_type, IntPtr p_data, IntPtr p_min, IntPtr p_max)
        {
            void* native_p_data = (void*)p_data.ToPointer();
            void* native_p_min = (void*)p_min.ToPointer();
            void* native_p_max = (void*)p_max.ToPointer();
            byte ret = ImGuiNative.igDataTypeClamp(data_type, native_p_data, native_p_min, native_p_max);
            return ret != 0;
        }
        public static int DataTypeCompare(ImGuiDataType data_type, IntPtr arg_1, IntPtr arg_2)
        {
            void* native_arg_1 = (void*)arg_1.ToPointer();
            void* native_arg_2 = (void*)arg_2.ToPointer();
            int ret = ImGuiNative.igDataTypeCompare(data_type, native_arg_1, native_arg_2);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int DataTypeFormatString(ReadOnlySpan<char> buf, int buf_size, ImGuiDataType data_type, IntPtr p_data, ReadOnlySpan<char> format)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            int ret = ImGuiNative.igDataTypeFormatString(native_buf, buf_size, data_type, native_p_data, native_format);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret;
        }
#endif
        public static int DataTypeFormatString(string buf, int buf_size, ImGuiDataType data_type, IntPtr p_data, string format)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            int ret = ImGuiNative.igDataTypeFormatString(native_buf, buf_size, data_type, native_p_data, native_format);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret;
        }
        public static ImGuiDataTypeInfoPtr DataTypeGetInfo(ImGuiDataType data_type)
        {
            ImGuiDataTypeInfo* ret = ImGuiNative.igDataTypeGetInfo(data_type);
            return new ImGuiDataTypeInfoPtr(ret);
        }
        public static void DebugDrawItemRect()
        {
            uint col = 4278190335;
            ImGuiNative.igDebugDrawItemRect(col);
        }
        public static void DebugDrawItemRect(uint col)
        {
            ImGuiNative.igDebugDrawItemRect(col);
        }
        public static void DebugLocateItem(uint target_id)
        {
            ImGuiNative.igDebugLocateItem(target_id);
        }
        public static void DebugLocateItemOnHover(uint target_id)
        {
            ImGuiNative.igDebugLocateItemOnHover(target_id);
        }
        public static void DebugLocateItemResolveWithLastItem()
        {
            ImGuiNative.igDebugLocateItemResolveWithLastItem();
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugLog(ReadOnlySpan<char> fmt)
        {
            byte* native_fmt;
            int fmt_byteCount = 0;
            if (fmt != null)
            {
                fmt_byteCount = Encoding.UTF8.GetByteCount(fmt);
                if (fmt_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt = Util.Allocate(fmt_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_stackBytes = stackalloc byte[fmt_byteCount + 1];
                    native_fmt = native_fmt_stackBytes;
                }
                int native_fmt_offset = Util.GetUtf8(fmt, native_fmt, fmt_byteCount);
                native_fmt[native_fmt_offset] = 0;
            }
            else { native_fmt = null; }
            ImGuiNative.igDebugLog(native_fmt);
            if (fmt_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt);
            }
        }
#endif
        public static void DebugLog(string fmt)
        {
            byte* native_fmt;
            int fmt_byteCount = 0;
            if (fmt != null)
            {
                fmt_byteCount = Encoding.UTF8.GetByteCount(fmt);
                if (fmt_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt = Util.Allocate(fmt_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_stackBytes = stackalloc byte[fmt_byteCount + 1];
                    native_fmt = native_fmt_stackBytes;
                }
                int native_fmt_offset = Util.GetUtf8(fmt, native_fmt, fmt_byteCount);
                native_fmt[native_fmt_offset] = 0;
            }
            else { native_fmt = null; }
            ImGuiNative.igDebugLog(native_fmt);
            if (fmt_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt);
            }
        }
        public static void DebugNodeColumns(ImGuiOldColumnsPtr columns)
        {
            ImGuiOldColumns* native_columns = columns.NativePtr;
            ImGuiNative.igDebugNodeColumns(native_columns);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeDockNode(ImGuiDockNodePtr node, ReadOnlySpan<char> label)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeDockNode(native_node, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void DebugNodeDockNode(ImGuiDockNodePtr node, string label)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeDockNode(native_node, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
        public static void DebugNodeDrawCmdShowMeshAndBoundingBox(ImDrawListPtr out_draw_list, ImDrawListPtr draw_list, ImDrawCmdPtr draw_cmd, bool show_mesh, bool show_aabb)
        {
            ImDrawList* native_out_draw_list = out_draw_list.NativePtr;
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImDrawCmd* native_draw_cmd = draw_cmd.NativePtr;
            byte native_show_mesh = show_mesh ? (byte)1 : (byte)0;
            byte native_show_aabb = show_aabb ? (byte)1 : (byte)0;
            ImGuiNative.igDebugNodeDrawCmdShowMeshAndBoundingBox(native_out_draw_list, native_draw_list, native_draw_cmd, native_show_mesh, native_show_aabb);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeDrawList(ImGuiWindowPtr window, ImGuiViewportPPtr viewport, ImDrawListPtr draw_list, ReadOnlySpan<char> label)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeDrawList(native_window, native_viewport, native_draw_list, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void DebugNodeDrawList(ImGuiWindowPtr window, ImGuiViewportPPtr viewport, ImDrawListPtr draw_list, string label)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeDrawList(native_window, native_viewport, native_draw_list, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
        public static void DebugNodeFont(ImFontPtr font)
        {
            ImFont* native_font = font.NativePtr;
            ImGuiNative.igDebugNodeFont(native_font);
        }
        public static void DebugNodeFontGlyph(ImFontPtr font, ImFontGlyphPtr glyph)
        {
            ImFont* native_font = font.NativePtr;
            ImFontGlyph* native_glyph = glyph.NativePtr;
            ImGuiNative.igDebugNodeFontGlyph(native_font, native_glyph);
        }
        public static void DebugNodeInputTextState(ImGuiInputTextStatePtr state)
        {
            ImGuiInputTextState* native_state = state.NativePtr;
            ImGuiNative.igDebugNodeInputTextState(native_state);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeStorage(ImGuiStoragePtr storage, ReadOnlySpan<char> label)
        {
            ImGuiStorage* native_storage = storage.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeStorage(native_storage, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void DebugNodeStorage(ImGuiStoragePtr storage, string label)
        {
            ImGuiStorage* native_storage = storage.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeStorage(native_storage, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeTabBar(ImGuiTabBarPtr tab_bar, ReadOnlySpan<char> label)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeTabBar(native_tab_bar, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void DebugNodeTabBar(ImGuiTabBarPtr tab_bar, string label)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeTabBar(native_tab_bar, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
        public static void DebugNodeTableSettings(ImGuiTableSettingsPtr settings)
        {
            ImGuiTableSettings* native_settings = settings.NativePtr;
            ImGuiNative.igDebugNodeTableSettings(native_settings);
        }
        public static void DebugNodeViewport(ImGuiViewportPPtr viewport)
        {
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igDebugNodeViewport(native_viewport);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeWindow(ImGuiWindowPtr window, ReadOnlySpan<char> label)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeWindow(native_window, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void DebugNodeWindow(ImGuiWindowPtr window, string label)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igDebugNodeWindow(native_window, native_label);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
        public static void DebugNodeWindowSettings(ImGuiWindowSettingsPtr settings)
        {
            ImGuiWindowSettings* native_settings = settings.NativePtr;
            ImGuiNative.igDebugNodeWindowSettings(native_settings);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DebugNodeWindowsList(ref ImVector windows, ReadOnlySpan<char> label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (ImVector* native_windows = &windows)
            {
                ImGuiNative.igDebugNodeWindowsList(native_windows, native_label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
            }
        }
#endif
        public static void DebugNodeWindowsList(ref ImVector windows, string label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            fixed (ImVector* native_windows = &windows)
            {
                ImGuiNative.igDebugNodeWindowsList(native_windows, native_label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_label);
                }
            }
        }
        public static void DebugNodeWindowsListByBeginStackParent(ref ImGuiWindow* windows, int windows_size, ImGuiWindowPtr parent_in_begin_stack)
        {
            ImGuiWindow* native_parent_in_begin_stack = parent_in_begin_stack.NativePtr;
            fixed (ImGuiWindow** native_windows = &windows)
            {
                ImGuiNative.igDebugNodeWindowsListByBeginStackParent(native_windows, windows_size, native_parent_in_begin_stack);
            }
        }
        public static void DebugRenderKeyboardPreview(ImDrawListPtr draw_list)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igDebugRenderKeyboardPreview(native_draw_list);
        }
        public static void DebugRenderViewportThumbnail(ImDrawListPtr draw_list, ImGuiViewportPPtr viewport, ImRect bb)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igDebugRenderViewportThumbnail(native_draw_list, native_viewport, bb);
        }
        public static void DebugStartItemPicker()
        {
            ImGuiNative.igDebugStartItemPicker();
        }
        public static void DestroyPlatformWindow(ImGuiViewportPPtr viewport)
        {
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igDestroyPlatformWindow(native_viewport);
        }
        public static uint DockBuilderAddNode()
        {
            uint node_id = 0;
            ImGuiDockNodeFlags flags = (ImGuiDockNodeFlags)0;
            uint ret = ImGuiNative.igDockBuilderAddNode(node_id, flags);
            return ret;
        }
        public static uint DockBuilderAddNode(uint node_id)
        {
            ImGuiDockNodeFlags flags = (ImGuiDockNodeFlags)0;
            uint ret = ImGuiNative.igDockBuilderAddNode(node_id, flags);
            return ret;
        }
        public static uint DockBuilderAddNode(uint node_id, ImGuiDockNodeFlags flags)
        {
            uint ret = ImGuiNative.igDockBuilderAddNode(node_id, flags);
            return ret;
        }
        public static void DockBuilderCopyDockSpace(uint src_dockspace_id, uint dst_dockspace_id, ref ImVector in_window_remap_pairs)
        {
            fixed (ImVector* native_in_window_remap_pairs = &in_window_remap_pairs)
            {
                ImGuiNative.igDockBuilderCopyDockSpace(src_dockspace_id, dst_dockspace_id, native_in_window_remap_pairs);
            }
        }
        public static void DockBuilderCopyNode(uint src_node_id, uint dst_node_id, out ImVector out_node_remap_pairs)
        {
            fixed (ImVector* native_out_node_remap_pairs = &out_node_remap_pairs)
            {
                ImGuiNative.igDockBuilderCopyNode(src_node_id, dst_node_id, native_out_node_remap_pairs);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DockBuilderCopyWindowSettings(ReadOnlySpan<char> src_name, ReadOnlySpan<char> dst_name)
        {
            byte* native_src_name;
            int src_name_byteCount = 0;
            if (src_name != null)
            {
                src_name_byteCount = Encoding.UTF8.GetByteCount(src_name);
                if (src_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_src_name = Util.Allocate(src_name_byteCount + 1);
                }
                else
                {
                    byte* native_src_name_stackBytes = stackalloc byte[src_name_byteCount + 1];
                    native_src_name = native_src_name_stackBytes;
                }
                int native_src_name_offset = Util.GetUtf8(src_name, native_src_name, src_name_byteCount);
                native_src_name[native_src_name_offset] = 0;
            }
            else { native_src_name = null; }
            byte* native_dst_name;
            int dst_name_byteCount = 0;
            if (dst_name != null)
            {
                dst_name_byteCount = Encoding.UTF8.GetByteCount(dst_name);
                if (dst_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst_name = Util.Allocate(dst_name_byteCount + 1);
                }
                else
                {
                    byte* native_dst_name_stackBytes = stackalloc byte[dst_name_byteCount + 1];
                    native_dst_name = native_dst_name_stackBytes;
                }
                int native_dst_name_offset = Util.GetUtf8(dst_name, native_dst_name, dst_name_byteCount);
                native_dst_name[native_dst_name_offset] = 0;
            }
            else { native_dst_name = null; }
            ImGuiNative.igDockBuilderCopyWindowSettings(native_src_name, native_dst_name);
            if (src_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_src_name);
            }
            if (dst_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_dst_name);
            }
        }
#endif
        public static void DockBuilderCopyWindowSettings(string src_name, string dst_name)
        {
            byte* native_src_name;
            int src_name_byteCount = 0;
            if (src_name != null)
            {
                src_name_byteCount = Encoding.UTF8.GetByteCount(src_name);
                if (src_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_src_name = Util.Allocate(src_name_byteCount + 1);
                }
                else
                {
                    byte* native_src_name_stackBytes = stackalloc byte[src_name_byteCount + 1];
                    native_src_name = native_src_name_stackBytes;
                }
                int native_src_name_offset = Util.GetUtf8(src_name, native_src_name, src_name_byteCount);
                native_src_name[native_src_name_offset] = 0;
            }
            else { native_src_name = null; }
            byte* native_dst_name;
            int dst_name_byteCount = 0;
            if (dst_name != null)
            {
                dst_name_byteCount = Encoding.UTF8.GetByteCount(dst_name);
                if (dst_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst_name = Util.Allocate(dst_name_byteCount + 1);
                }
                else
                {
                    byte* native_dst_name_stackBytes = stackalloc byte[dst_name_byteCount + 1];
                    native_dst_name = native_dst_name_stackBytes;
                }
                int native_dst_name_offset = Util.GetUtf8(dst_name, native_dst_name, dst_name_byteCount);
                native_dst_name[native_dst_name_offset] = 0;
            }
            else { native_dst_name = null; }
            ImGuiNative.igDockBuilderCopyWindowSettings(native_src_name, native_dst_name);
            if (src_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_src_name);
            }
            if (dst_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_dst_name);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void DockBuilderDockWindow(ReadOnlySpan<char> window_name, uint node_id)
        {
            byte* native_window_name;
            int window_name_byteCount = 0;
            if (window_name != null)
            {
                window_name_byteCount = Encoding.UTF8.GetByteCount(window_name);
                if (window_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_window_name = Util.Allocate(window_name_byteCount + 1);
                }
                else
                {
                    byte* native_window_name_stackBytes = stackalloc byte[window_name_byteCount + 1];
                    native_window_name = native_window_name_stackBytes;
                }
                int native_window_name_offset = Util.GetUtf8(window_name, native_window_name, window_name_byteCount);
                native_window_name[native_window_name_offset] = 0;
            }
            else { native_window_name = null; }
            ImGuiNative.igDockBuilderDockWindow(native_window_name, node_id);
            if (window_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_window_name);
            }
        }
#endif
        public static void DockBuilderDockWindow(string window_name, uint node_id)
        {
            byte* native_window_name;
            int window_name_byteCount = 0;
            if (window_name != null)
            {
                window_name_byteCount = Encoding.UTF8.GetByteCount(window_name);
                if (window_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_window_name = Util.Allocate(window_name_byteCount + 1);
                }
                else
                {
                    byte* native_window_name_stackBytes = stackalloc byte[window_name_byteCount + 1];
                    native_window_name = native_window_name_stackBytes;
                }
                int native_window_name_offset = Util.GetUtf8(window_name, native_window_name, window_name_byteCount);
                native_window_name[native_window_name_offset] = 0;
            }
            else { native_window_name = null; }
            ImGuiNative.igDockBuilderDockWindow(native_window_name, node_id);
            if (window_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_window_name);
            }
        }
        public static void DockBuilderFinish(uint node_id)
        {
            ImGuiNative.igDockBuilderFinish(node_id);
        }
        public static ImGuiDockNodePtr DockBuilderGetCentralNode(uint node_id)
        {
            ImGuiDockNode* ret = ImGuiNative.igDockBuilderGetCentralNode(node_id);
            return new ImGuiDockNodePtr(ret);
        }
        public static ImGuiDockNodePtr DockBuilderGetNode(uint node_id)
        {
            ImGuiDockNode* ret = ImGuiNative.igDockBuilderGetNode(node_id);
            return new ImGuiDockNodePtr(ret);
        }
        public static void DockBuilderRemoveNode(uint node_id)
        {
            ImGuiNative.igDockBuilderRemoveNode(node_id);
        }
        public static void DockBuilderRemoveNodeChildNodes(uint node_id)
        {
            ImGuiNative.igDockBuilderRemoveNodeChildNodes(node_id);
        }
        public static void DockBuilderRemoveNodeDockedWindows(uint node_id)
        {
            byte clear_settings_refs = 1;
            ImGuiNative.igDockBuilderRemoveNodeDockedWindows(node_id, clear_settings_refs);
        }
        public static void DockBuilderRemoveNodeDockedWindows(uint node_id, bool clear_settings_refs)
        {
            byte native_clear_settings_refs = clear_settings_refs ? (byte)1 : (byte)0;
            ImGuiNative.igDockBuilderRemoveNodeDockedWindows(node_id, native_clear_settings_refs);
        }
        public static void DockBuilderSetNodePos(uint node_id, Vector2 pos)
        {
            ImGuiNative.igDockBuilderSetNodePos(node_id, pos);
        }
        public static void DockBuilderSetNodeSize(uint node_id, Vector2 size)
        {
            ImGuiNative.igDockBuilderSetNodeSize(node_id, size);
        }
        public static uint DockBuilderSplitNode(uint node_id, ImGuiDir split_dir, float size_ratio_for_node_at_dir, out uint out_id_at_dir, out uint out_id_at_opposite_dir)
        {
            fixed (uint* native_out_id_at_dir = &out_id_at_dir)
            {
                fixed (uint* native_out_id_at_opposite_dir = &out_id_at_opposite_dir)
                {
                    uint ret = ImGuiNative.igDockBuilderSplitNode(node_id, split_dir, size_ratio_for_node_at_dir, native_out_id_at_dir, native_out_id_at_opposite_dir);
                    return ret;
                }
            }
        }
        public static bool DockContextCalcDropPosForDocking(ImGuiWindowPtr target, ImGuiDockNodePtr target_node, ImGuiWindowPtr payload_window, ImGuiDockNodePtr payload_node, ImGuiDir split_dir, bool split_outer, out Vector2 out_pos)
        {
            ImGuiWindow* native_target = target.NativePtr;
            ImGuiDockNode* native_target_node = target_node.NativePtr;
            ImGuiWindow* native_payload_window = payload_window.NativePtr;
            ImGuiDockNode* native_payload_node = payload_node.NativePtr;
            byte native_split_outer = split_outer ? (byte)1 : (byte)0;
            fixed (Vector2* native_out_pos = &out_pos)
            {
                byte ret = ImGuiNative.igDockContextCalcDropPosForDocking(native_target, native_target_node, native_payload_window, native_payload_node, split_dir, native_split_outer, native_out_pos);
                return ret != 0;
            }
        }
        public static void DockContextClearNodes(IntPtr ctx, uint root_id, bool clear_settings_refs)
        {
            byte native_clear_settings_refs = clear_settings_refs ? (byte)1 : (byte)0;
            ImGuiNative.igDockContextClearNodes(ctx, root_id, native_clear_settings_refs);
        }
        public static void DockContextEndFrame(IntPtr ctx)
        {
            ImGuiNative.igDockContextEndFrame(ctx);
        }
        public static ImGuiDockNodePtr DockContextFindNodeByID(IntPtr ctx, uint id)
        {
            ImGuiDockNode* ret = ImGuiNative.igDockContextFindNodeByID(ctx, id);
            return new ImGuiDockNodePtr(ret);
        }
        public static uint DockContextGenNodeID(IntPtr ctx)
        {
            uint ret = ImGuiNative.igDockContextGenNodeID(ctx);
            return ret;
        }
        public static void DockContextInitialize(IntPtr ctx)
        {
            ImGuiNative.igDockContextInitialize(ctx);
        }
        public static void DockContextNewFrameUpdateDocking(IntPtr ctx)
        {
            ImGuiNative.igDockContextNewFrameUpdateDocking(ctx);
        }
        public static void DockContextNewFrameUpdateUndocking(IntPtr ctx)
        {
            ImGuiNative.igDockContextNewFrameUpdateUndocking(ctx);
        }
        public static void DockContextProcessUndockNode(IntPtr ctx, ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            ImGuiNative.igDockContextProcessUndockNode(ctx, native_node);
        }
        public static void DockContextProcessUndockWindow(IntPtr ctx, ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte clear_persistent_docking_ref = 1;
            ImGuiNative.igDockContextProcessUndockWindow(ctx, native_window, clear_persistent_docking_ref);
        }
        public static void DockContextProcessUndockWindow(IntPtr ctx, ImGuiWindowPtr window, bool clear_persistent_docking_ref)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte native_clear_persistent_docking_ref = clear_persistent_docking_ref ? (byte)1 : (byte)0;
            ImGuiNative.igDockContextProcessUndockWindow(ctx, native_window, native_clear_persistent_docking_ref);
        }
        public static void DockContextQueueDock(IntPtr ctx, ImGuiWindowPtr target, ImGuiDockNodePtr target_node, ImGuiWindowPtr payload, ImGuiDir split_dir, float split_ratio, bool split_outer)
        {
            ImGuiWindow* native_target = target.NativePtr;
            ImGuiDockNode* native_target_node = target_node.NativePtr;
            ImGuiWindow* native_payload = payload.NativePtr;
            byte native_split_outer = split_outer ? (byte)1 : (byte)0;
            ImGuiNative.igDockContextQueueDock(ctx, native_target, native_target_node, native_payload, split_dir, split_ratio, native_split_outer);
        }
        public static void DockContextQueueUndockNode(IntPtr ctx, ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            ImGuiNative.igDockContextQueueUndockNode(ctx, native_node);
        }
        public static void DockContextQueueUndockWindow(IntPtr ctx, ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igDockContextQueueUndockWindow(ctx, native_window);
        }
        public static void DockContextRebuildNodes(IntPtr ctx)
        {
            ImGuiNative.igDockContextRebuildNodes(ctx);
        }
        public static void DockContextShutdown(IntPtr ctx)
        {
            ImGuiNative.igDockContextShutdown(ctx);
        }
        public static bool DockNodeBeginAmendTabBar(ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            byte ret = ImGuiNative.igDockNodeBeginAmendTabBar(native_node);
            return ret != 0;
        }
        public static void DockNodeEndAmendTabBar()
        {
            ImGuiNative.igDockNodeEndAmendTabBar();
        }
        public static int DockNodeGetDepth(ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            int ret = ImGuiNative.igDockNodeGetDepth(native_node);
            return ret;
        }
        public static ImGuiDockNodePtr DockNodeGetRootNode(ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            ImGuiDockNode* ret = ImGuiNative.igDockNodeGetRootNode(native_node);
            return new ImGuiDockNodePtr(ret);
        }
        public static uint DockNodeGetWindowMenuButtonId(ImGuiDockNodePtr node)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            uint ret = ImGuiNative.igDockNodeGetWindowMenuButtonId(native_node);
            return ret;
        }
        public static bool DockNodeIsInHierarchyOf(ImGuiDockNodePtr node, ImGuiDockNodePtr parent)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            ImGuiDockNode* native_parent = parent.NativePtr;
            byte ret = ImGuiNative.igDockNodeIsInHierarchyOf(native_node, native_parent);
            return ret != 0;
        }
        public static void DockNodeWindowMenuHandler_Default(IntPtr ctx, ImGuiDockNodePtr node, ImGuiTabBarPtr tab_bar)
        {
            ImGuiDockNode* native_node = node.NativePtr;
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiNative.igDockNodeWindowMenuHandler_Default(ctx, native_node, native_tab_bar);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool DragBehavior(uint id, ImGuiDataType data_type, IntPtr p_v, float v_speed, IntPtr p_min, IntPtr p_max, ReadOnlySpan<char> format, ImGuiSliderFlags flags)
        {
            void* native_p_v = (void*)p_v.ToPointer();
            void* native_p_min = (void*)p_min.ToPointer();
            void* native_p_max = (void*)p_max.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte ret = ImGuiNative.igDragBehavior(id, data_type, native_p_v, v_speed, native_p_min, native_p_max, native_format, flags);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool DragBehavior(uint id, ImGuiDataType data_type, IntPtr p_v, float v_speed, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags)
        {
            void* native_p_v = (void*)p_v.ToPointer();
            void* native_p_min = (void*)p_min.ToPointer();
            void* native_p_max = (void*)p_max.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte ret = ImGuiNative.igDragBehavior(id, data_type, native_p_v, v_speed, native_p_min, native_p_max, native_format, flags);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
        public static void EndColumns()
        {
            ImGuiNative.igEndColumns();
        }
        public static void EndComboPreview()
        {
            ImGuiNative.igEndComboPreview();
        }
        public static void ErrorCheckEndFrameRecover(IntPtr log_callback)
        {
            void* user_data = null;
            ImGuiNative.igErrorCheckEndFrameRecover(log_callback, user_data);
        }
        public static void ErrorCheckEndFrameRecover(IntPtr log_callback, IntPtr user_data)
        {
            void* native_user_data = (void*)user_data.ToPointer();
            ImGuiNative.igErrorCheckEndFrameRecover(log_callback, native_user_data);
        }
        public static void ErrorCheckEndWindowRecover(IntPtr log_callback)
        {
            void* user_data = null;
            ImGuiNative.igErrorCheckEndWindowRecover(log_callback, user_data);
        }
        public static void ErrorCheckEndWindowRecover(IntPtr log_callback, IntPtr user_data)
        {
            void* native_user_data = (void*)user_data.ToPointer();
            ImGuiNative.igErrorCheckEndWindowRecover(log_callback, native_user_data);
        }
        public static void ErrorCheckUsingSetCursorPosToExtendParentBoundaries()
        {
            ImGuiNative.igErrorCheckUsingSetCursorPosToExtendParentBoundaries();
        }
        public static Vector2 FindBestWindowPosForPopup(ImGuiWindowPtr window)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igFindBestWindowPosForPopup(&__retval, native_window);
            return __retval;
        }
        //public static Vector2 FindBestWindowPosForPopupEx(Vector2 ref_pos, Vector2 size, ref ImGuiDir last_dir, ImRect r_outer, ImRect r_avoid, ImGuiPopupPositionPolicy policy)
        //{
        //    Vector2 __retval;
        //    fixed (IntPtr native_last_dir = &last_dir)
        //    {
        //        ImGuiNative.igFindBestWindowPosForPopupEx(&__retval, ref_pos, size, native_last_dir, r_outer, r_avoid, policy);
        //        return __retval;
        //    }
        //}
        public static ImGuiWindowPtr FindBlockingModal(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* ret = ImGuiNative.igFindBlockingModal(native_window);
            return new ImGuiWindowPtr(ret);
        }
        public static ImGuiWindowPtr FindBottomMostVisibleWindowWithinBeginStack(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* ret = ImGuiNative.igFindBottomMostVisibleWindowWithinBeginStack(native_window);
            return new ImGuiWindowPtr(ret);
        }
        public static ImGuiViewportPPtr FindHoveredViewportFromPlatformWindowStack(Vector2 mouse_platform_pos)
        {
            ImGuiViewportP* ret = ImGuiNative.igFindHoveredViewportFromPlatformWindowStack(mouse_platform_pos);
            return new ImGuiViewportPPtr(ret);
        }
        public static ImGuiOldColumnsPtr FindOrCreateColumns(ImGuiWindowPtr window, uint id)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiOldColumns* ret = ImGuiNative.igFindOrCreateColumns(native_window, id);
            return new ImGuiOldColumnsPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string FindRenderedTextEnd(ReadOnlySpan<char> text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte* ret = ImGuiNative.igFindRenderedTextEnd(native_text, native_text+text_byteCount);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string FindRenderedTextEnd(string text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte* ret = ImGuiNative.igFindRenderedTextEnd(native_text, native_text+text_byteCount);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static ImGuiSettingsHandlerPtr FindSettingsHandler(ReadOnlySpan<char> type_name)
        {
            byte* native_type_name;
            int type_name_byteCount = 0;
            if (type_name != null)
            {
                type_name_byteCount = Encoding.UTF8.GetByteCount(type_name);
                if (type_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_type_name = Util.Allocate(type_name_byteCount + 1);
                }
                else
                {
                    byte* native_type_name_stackBytes = stackalloc byte[type_name_byteCount + 1];
                    native_type_name = native_type_name_stackBytes;
                }
                int native_type_name_offset = Util.GetUtf8(type_name, native_type_name, type_name_byteCount);
                native_type_name[native_type_name_offset] = 0;
            }
            else { native_type_name = null; }
            ImGuiSettingsHandler* ret = ImGuiNative.igFindSettingsHandler(native_type_name);
            if (type_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_type_name);
            }
            return new ImGuiSettingsHandlerPtr(ret);
        }
#endif
        public static ImGuiSettingsHandlerPtr FindSettingsHandler(string type_name)
        {
            byte* native_type_name;
            int type_name_byteCount = 0;
            if (type_name != null)
            {
                type_name_byteCount = Encoding.UTF8.GetByteCount(type_name);
                if (type_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_type_name = Util.Allocate(type_name_byteCount + 1);
                }
                else
                {
                    byte* native_type_name_stackBytes = stackalloc byte[type_name_byteCount + 1];
                    native_type_name = native_type_name_stackBytes;
                }
                int native_type_name_offset = Util.GetUtf8(type_name, native_type_name, type_name_byteCount);
                native_type_name[native_type_name_offset] = 0;
            }
            else { native_type_name = null; }
            ImGuiSettingsHandler* ret = ImGuiNative.igFindSettingsHandler(native_type_name);
            if (type_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_type_name);
            }
            return new ImGuiSettingsHandlerPtr(ret);
        }
        public static ImGuiWindowPtr FindWindowByID(uint id)
        {
            ImGuiWindow* ret = ImGuiNative.igFindWindowByID(id);
            return new ImGuiWindowPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static ImGuiWindowPtr FindWindowByName(ReadOnlySpan<char> name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiWindow* ret = ImGuiNative.igFindWindowByName(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return new ImGuiWindowPtr(ret);
        }
#endif
        public static ImGuiWindowPtr FindWindowByName(string name)
        {
            byte* native_name;
            int name_byteCount = 0;
            if (name != null)
            {
                name_byteCount = Encoding.UTF8.GetByteCount(name);
                if (name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_name = Util.Allocate(name_byteCount + 1);
                }
                else
                {
                    byte* native_name_stackBytes = stackalloc byte[name_byteCount + 1];
                    native_name = native_name_stackBytes;
                }
                int native_name_offset = Util.GetUtf8(name, native_name, name_byteCount);
                native_name[native_name_offset] = 0;
            }
            else { native_name = null; }
            ImGuiWindow* ret = ImGuiNative.igFindWindowByName(native_name);
            if (name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_name);
            }
            return new ImGuiWindowPtr(ret);
        }
        public static int FindWindowDisplayIndex(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            int ret = ImGuiNative.igFindWindowDisplayIndex(native_window);
            return ret;
        }
        public static ImGuiWindowSettingsPtr FindWindowSettingsByID(uint id)
        {
            ImGuiWindowSettings* ret = ImGuiNative.igFindWindowSettingsByID(id);
            return new ImGuiWindowSettingsPtr(ret);
        }
        public static ImGuiWindowSettingsPtr FindWindowSettingsByWindow(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindowSettings* ret = ImGuiNative.igFindWindowSettingsByWindow(native_window);
            return new ImGuiWindowSettingsPtr(ret);
        }
        public static void FocusItem()
        {
            ImGuiNative.igFocusItem();
        }
        public static void FocusTopMostWindowUnderOne(ImGuiWindowPtr under_this_window, ImGuiWindowPtr ignore_window, ImGuiViewportPtr filter_viewport, ImGuiFocusRequestFlags flags)
        {
            ImGuiWindow* native_under_this_window = under_this_window.NativePtr;
            ImGuiWindow* native_ignore_window = ignore_window.NativePtr;
            ImGuiViewport* native_filter_viewport = filter_viewport.NativePtr;
            ImGuiNative.igFocusTopMostWindowUnderOne(native_under_this_window, native_ignore_window, native_filter_viewport, flags);
        }
        public static void FocusWindow(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiFocusRequestFlags flags = (ImGuiFocusRequestFlags)0;
            ImGuiNative.igFocusWindow(native_window, flags);
        }
        public static void FocusWindow(ImGuiWindowPtr window, ImGuiFocusRequestFlags flags)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igFocusWindow(native_window, flags);
        }
        public static void GcAwakeTransientWindowBuffers(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igGcAwakeTransientWindowBuffers(native_window);
        }
        public static void GcCompactTransientMiscBuffers()
        {
            ImGuiNative.igGcCompactTransientMiscBuffers();
        }
        public static void GcCompactTransientWindowBuffers(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igGcCompactTransientWindowBuffers(native_window);
        }
        public static uint GetActiveID()
        {
            uint ret = ImGuiNative.igGetActiveID();
            return ret;
        }
        public static float GetColumnNormFromOffset(ImGuiOldColumnsPtr columns, float offset)
        {
            ImGuiOldColumns* native_columns = columns.NativePtr;
            float ret = ImGuiNative.igGetColumnNormFromOffset(native_columns, offset);
            return ret;
        }
        public static float GetColumnOffsetFromNorm(ImGuiOldColumnsPtr columns, float offset_norm)
        {
            ImGuiOldColumns* native_columns = columns.NativePtr;
            float ret = ImGuiNative.igGetColumnOffsetFromNorm(native_columns, offset_norm);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static uint GetColumnsID(ReadOnlySpan<char> str_id, int count)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            uint ret = ImGuiNative.igGetColumnsID(native_str_id, count);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret;
        }
#endif
        public static uint GetColumnsID(string str_id, int count)
        {
            byte* native_str_id;
            int str_id_byteCount = 0;
            if (str_id != null)
            {
                str_id_byteCount = Encoding.UTF8.GetByteCount(str_id);
                if (str_id_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id = Util.Allocate(str_id_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_stackBytes = stackalloc byte[str_id_byteCount + 1];
                    native_str_id = native_str_id_stackBytes;
                }
                int native_str_id_offset = Util.GetUtf8(str_id, native_str_id, str_id_byteCount);
                native_str_id[native_str_id_offset] = 0;
            }
            else { native_str_id = null; }
            uint ret = ImGuiNative.igGetColumnsID(native_str_id, count);
            if (str_id_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id);
            }
            return ret;
        }
        public static Vector2 GetContentRegionMaxAbs()
        {
            Vector2 __retval;
            ImGuiNative.igGetContentRegionMaxAbs(&__retval);
            return __retval;
        }
        public static uint GetCurrentFocusScope()
        {
            uint ret = ImGuiNative.igGetCurrentFocusScope();
            return ret;
        }
        public static ImGuiTabBarPtr GetCurrentTabBar()
        {
            ImGuiTabBar* ret = ImGuiNative.igGetCurrentTabBar();
            return new ImGuiTabBarPtr(ret);
        }
        public static ImGuiWindowPtr GetCurrentWindow()
        {
            ImGuiWindow* ret = ImGuiNative.igGetCurrentWindow();
            return new ImGuiWindowPtr(ret);
        }
        public static ImGuiWindowPtr GetCurrentWindowRead()
        {
            ImGuiWindow* ret = ImGuiNative.igGetCurrentWindowRead();
            return new ImGuiWindowPtr(ret);
        }
        public static ImFontPtr GetDefaultFont()
        {
            ImFont* ret = ImGuiNative.igGetDefaultFont();
            return new ImFontPtr(ret);
        }
        public static uint GetFocusID()
        {
            uint ret = ImGuiNative.igGetFocusID();
            return ret;
        }
        public static ImDrawListPtr GetForegroundDrawList(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImDrawList* ret = ImGuiNative.igGetForegroundDrawList_WindowPtr(native_window);
            return new ImDrawListPtr(ret);
        }
        public static uint GetHoveredID()
        {
            uint ret = ImGuiNative.igGetHoveredID();
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static uint GetIDWithSeed(ReadOnlySpan<char> str_id_begin, uint seed)
        {
            byte* native_str_id_begin;
            int str_id_begin_byteCount = 0;
                str_id_begin_byteCount = Encoding.UTF8.GetByteCount(str_id_begin);
                if (str_id_begin_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id_begin = Util.Allocate(str_id_begin_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_begin_stackBytes = stackalloc byte[str_id_begin_byteCount + 1];
                    native_str_id_begin = native_str_id_begin_stackBytes;
                }
                int native_str_id_begin_offset = Util.GetUtf8(str_id_begin, native_str_id_begin, str_id_begin_byteCount);
                native_str_id_begin[native_str_id_begin_offset] = 0;
            uint ret = ImGuiNative.igGetIDWithSeed_Str(native_str_id_begin, native_str_id_begin+str_id_begin_byteCount, seed);
            if (str_id_begin_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id_begin);
            }
            return ret;
        }
#endif
        public static uint GetIDWithSeed(string str_id_begin, uint seed)
        {
            byte* native_str_id_begin;
            int str_id_begin_byteCount = 0;
                str_id_begin_byteCount = Encoding.UTF8.GetByteCount(str_id_begin);
                if (str_id_begin_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_id_begin = Util.Allocate(str_id_begin_byteCount + 1);
                }
                else
                {
                    byte* native_str_id_begin_stackBytes = stackalloc byte[str_id_begin_byteCount + 1];
                    native_str_id_begin = native_str_id_begin_stackBytes;
                }
                int native_str_id_begin_offset = Util.GetUtf8(str_id_begin, native_str_id_begin, str_id_begin_byteCount);
                native_str_id_begin[native_str_id_begin_offset] = 0;
            uint ret = ImGuiNative.igGetIDWithSeed_Str(native_str_id_begin, native_str_id_begin+str_id_begin_byteCount, seed);
            if (str_id_begin_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_id_begin);
            }
            return ret;
        }
        public static uint GetIDWithSeed(int n, uint seed)
        {
            uint ret = ImGuiNative.igGetIDWithSeed_Int(n, seed);
            return ret;
        }
        public static ImGuiInputTextStatePtr GetInputTextState(uint id)
        {
            ImGuiInputTextState* ret = ImGuiNative.igGetInputTextState(id);
            return new ImGuiInputTextStatePtr(ret);
        }
        public static ImGuiItemFlags GetItemFlags()
        {
            ImGuiItemFlags ret = ImGuiNative.igGetItemFlags();
            return ret;
        }
        public static ImGuiItemStatusFlags GetItemStatusFlags()
        {
            ImGuiItemStatusFlags ret = ImGuiNative.igGetItemStatusFlags();
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void GetKeyChordName(ImGuiKey key_chord, ReadOnlySpan<char> out_buf, int out_buf_size)
        {
            byte* native_out_buf;
            int out_buf_byteCount = 0;
            if (out_buf != null)
            {
                out_buf_byteCount = Encoding.UTF8.GetByteCount(out_buf);
                if (out_buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_out_buf = Util.Allocate(out_buf_byteCount + 1);
                }
                else
                {
                    byte* native_out_buf_stackBytes = stackalloc byte[out_buf_byteCount + 1];
                    native_out_buf = native_out_buf_stackBytes;
                }
                int native_out_buf_offset = Util.GetUtf8(out_buf, native_out_buf, out_buf_byteCount);
                native_out_buf[native_out_buf_offset] = 0;
            }
            else { native_out_buf = null; }
            ImGuiNative.igGetKeyChordName(key_chord, native_out_buf, out_buf_size);
            if (out_buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_out_buf);
            }
        }
#endif
        public static void GetKeyChordName(ImGuiKey key_chord, string out_buf, int out_buf_size)
        {
            byte* native_out_buf;
            int out_buf_byteCount = 0;
            if (out_buf != null)
            {
                out_buf_byteCount = Encoding.UTF8.GetByteCount(out_buf);
                if (out_buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_out_buf = Util.Allocate(out_buf_byteCount + 1);
                }
                else
                {
                    byte* native_out_buf_stackBytes = stackalloc byte[out_buf_byteCount + 1];
                    native_out_buf = native_out_buf_stackBytes;
                }
                int native_out_buf_offset = Util.GetUtf8(out_buf, native_out_buf, out_buf_byteCount);
                native_out_buf[native_out_buf_offset] = 0;
            }
            else { native_out_buf = null; }
            ImGuiNative.igGetKeyChordName(key_chord, native_out_buf, out_buf_size);
            if (out_buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_out_buf);
            }
        }
        public static ImGuiKeyDataPtr GetKeyData(IntPtr ctx, ImGuiKey key)
        {
            ImGuiKeyData* ret = ImGuiNative.igGetKeyData_ContextPtr(ctx, key);
            return new ImGuiKeyDataPtr(ret);
        }
        public static ImGuiKeyDataPtr GetKeyData(ImGuiKey key)
        {
            ImGuiKeyData* ret = ImGuiNative.igGetKeyData_Key(key);
            return new ImGuiKeyDataPtr(ret);
        }
        public static Vector2 GetKeyMagnitude2d(ImGuiKey key_left, ImGuiKey key_right, ImGuiKey key_up, ImGuiKey key_down)
        {
            Vector2 __retval;
            ImGuiNative.igGetKeyMagnitude2d(&__retval, key_left, key_right, key_up, key_down);
            return __retval;
        }
        public static uint GetKeyOwner(ImGuiKey key)
        {
            uint ret = ImGuiNative.igGetKeyOwner(key);
            return ret;
        }
        public static ImGuiKeyOwnerDataPtr GetKeyOwnerData(IntPtr ctx, ImGuiKey key)
        {
            ImGuiKeyOwnerData* ret = ImGuiNative.igGetKeyOwnerData(ctx, key);
            return new ImGuiKeyOwnerDataPtr(ret);
        }
        public static float GetNavTweakPressedAmount(ImGuiAxis axis)
        {
            float ret = ImGuiNative.igGetNavTweakPressedAmount(axis);
            return ret;
        }
        public static ImRect GetPopupAllowedExtentRect(ImGuiWindowPtr window)
        {
            ImRect __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igGetPopupAllowedExtentRect(&__retval, native_window);
            return __retval;
        }
        public static ImGuiKeyRoutingDataPtr GetShortcutRoutingData(ImGuiKey key_chord)
        {
            ImGuiKeyRoutingData* ret = ImGuiNative.igGetShortcutRoutingData(key_chord);
            return new ImGuiKeyRoutingDataPtr(ret);
        }
        public static ImGuiDataVarInfoPtr GetStyleVarInfo(ImGuiStyleVar idx)
        {
            ImGuiDataVarInfo* ret = ImGuiNative.igGetStyleVarInfo(idx);
            return new ImGuiDataVarInfoPtr(ret);
        }
        public static ImGuiWindowPtr GetTopMostAndVisiblePopupModal()
        {
            ImGuiWindow* ret = ImGuiNative.igGetTopMostAndVisiblePopupModal();
            return new ImGuiWindowPtr(ret);
        }
        public static ImGuiWindowPtr GetTopMostPopupModal()
        {
            ImGuiWindow* ret = ImGuiNative.igGetTopMostPopupModal();
            return new ImGuiWindowPtr(ret);
        }
        public static void GetTypematicRepeatRate(ImGuiInputFlags flags, ref float repeat_delay, ref float repeat_rate)
        {
            fixed (float* native_repeat_delay = &repeat_delay)
            {
                fixed (float* native_repeat_rate = &repeat_rate)
                {
                    ImGuiNative.igGetTypematicRepeatRate(flags, native_repeat_delay, native_repeat_rate);
                }
            }
        }
        public static ImGuiPlatformMonitorPtr GetViewportPlatformMonitor(ImGuiViewportPtr viewport)
        {
            ImGuiViewport* native_viewport = viewport.NativePtr;
            ImGuiPlatformMonitor* ret = ImGuiNative.igGetViewportPlatformMonitor(native_viewport);
            return new ImGuiPlatformMonitorPtr(ret);
        }
        public static bool GetWindowAlwaysWantOwnTabBar(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte ret = ImGuiNative.igGetWindowAlwaysWantOwnTabBar(native_window);
            return ret != 0;
        }
        public static ImGuiDockNodePtr GetWindowDockNode()
        {
            ImGuiDockNode* ret = ImGuiNative.igGetWindowDockNode();
            return new ImGuiDockNodePtr(ret);
        }
        public static uint GetWindowResizeBorderID(ImGuiWindowPtr window, ImGuiDir dir)
        {
            ImGuiWindow* native_window = window.NativePtr;
            uint ret = ImGuiNative.igGetWindowResizeBorderID(native_window, dir);
            return ret;
        }
        public static uint GetWindowResizeCornerID(ImGuiWindowPtr window, int n)
        {
            ImGuiWindow* native_window = window.NativePtr;
            uint ret = ImGuiNative.igGetWindowResizeCornerID(native_window, n);
            return ret;
        }
        public static uint GetWindowScrollbarID(ImGuiWindowPtr window, ImGuiAxis axis)
        {
            ImGuiWindow* native_window = window.NativePtr;
            uint ret = ImGuiNative.igGetWindowScrollbarID(native_window, axis);
            return ret;
        }
        public static ImRect GetWindowScrollbarRect(ImGuiWindowPtr window, ImGuiAxis axis)
        {
            ImRect __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igGetWindowScrollbarRect(&__retval, native_window, axis);
            return __retval;
        }
        public static int ImAbs(int x)
        {
            int ret = ImGuiNative.igImAbs_Int(x);
            return ret;
        }
        public static float ImAbs(float x)
        {
            float ret = ImGuiNative.igImAbs_Float(x);
            return ret;
        }
        public static double ImAbs(double x)
        {
            double ret = ImGuiNative.igImAbs_double(x);
            return ret;
        }
        public static bool ImageButtonEx(uint id, IntPtr texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 bg_col, Vector4 tint_col)
        {
            ImGuiButtonFlags flags = (ImGuiButtonFlags)0;
            byte ret = ImGuiNative.igImageButtonEx(id, texture_id, size, uv0, uv1, bg_col, tint_col, flags);
            return ret != 0;
        }
        public static bool ImageButtonEx(uint id, IntPtr texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 bg_col, Vector4 tint_col, ImGuiButtonFlags flags)
        {
            byte ret = ImGuiNative.igImageButtonEx(id, texture_id, size, uv0, uv1, bg_col, tint_col, flags);
            return ret != 0;
        }
        public static uint ImAlphaBlendColors(uint col_a, uint col_b)
        {
            uint ret = ImGuiNative.igImAlphaBlendColors(col_a, col_b);
            return ret;
        }
        public static Vector2 ImBezierCubicCalc(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t)
        {
            Vector2 __retval;
            ImGuiNative.igImBezierCubicCalc(&__retval, p1, p2, p3, p4, t);
            return __retval;
        }
        public static Vector2 ImBezierCubicClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p, int num_segments)
        {
            Vector2 __retval;
            ImGuiNative.igImBezierCubicClosestPoint(&__retval, p1, p2, p3, p4, p, num_segments);
            return __retval;
        }
        public static Vector2 ImBezierCubicClosestPointCasteljau(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p, float tess_tol)
        {
            Vector2 __retval;
            ImGuiNative.igImBezierCubicClosestPointCasteljau(&__retval, p1, p2, p3, p4, p, tess_tol);
            return __retval;
        }
        public static Vector2 ImBezierQuadraticCalc(Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 __retval;
            ImGuiNative.igImBezierQuadraticCalc(&__retval, p1, p2, p3, t);
            return __retval;
        }
        public static void ImBitArrayClearAllBits(ref uint arr, int bitcount)
        {
            fixed (uint* native_arr = &arr)
            {
                ImGuiNative.igImBitArrayClearAllBits(native_arr, bitcount);
            }
        }
        public static void ImBitArrayClearBit(ref uint arr, int n)
        {
            fixed (uint* native_arr = &arr)
            {
                ImGuiNative.igImBitArrayClearBit(native_arr, n);
            }
        }
        public static uint ImBitArrayGetStorageSizeInBytes(int bitcount)
        {
            uint ret = ImGuiNative.igImBitArrayGetStorageSizeInBytes(bitcount);
            return ret;
        }
        public static void ImBitArraySetBit(ref uint arr, int n)
        {
            fixed (uint* native_arr = &arr)
            {
                ImGuiNative.igImBitArraySetBit(native_arr, n);
            }
        }
        public static void ImBitArraySetBitRange(ref uint arr, int n, int n2)
        {
            fixed (uint* native_arr = &arr)
            {
                ImGuiNative.igImBitArraySetBitRange(native_arr, n, n2);
            }
        }
        public static bool ImBitArrayTestBit(ref uint arr, int n)
        {
            fixed (uint* native_arr = &arr)
            {
                byte ret = ImGuiNative.igImBitArrayTestBit(native_arr, n);
                return ret != 0;
            }
        }
        public static bool ImCharIsBlankA(byte c)
        {
            byte ret = ImGuiNative.igImCharIsBlankA(c);
            return ret != 0;
        }
        public static bool ImCharIsBlankW(uint c)
        {
            byte ret = ImGuiNative.igImCharIsBlankW(c);
            return ret != 0;
        }
        public static Vector2 ImClamp(Vector2 v, Vector2 mn, Vector2 mx)
        {
            Vector2 __retval;
            ImGuiNative.igImClamp(&__retval, v, mn, mx);
            return __retval;
        }
        public static float ImDot(Vector2 a, Vector2 b)
        {
            float ret = ImGuiNative.igImDot(a, b);
            return ret;
        }
        public static float ImExponentialMovingAverage(float avg, float sample, int n)
        {
            float ret = ImGuiNative.igImExponentialMovingAverage(avg, sample, n);
            return ret;
        }
        public static bool ImFileClose(IntPtr file)
        {
            byte ret = ImGuiNative.igImFileClose(file);
            return ret != 0;
        }
        public static ulong ImFileGetSize(IntPtr file)
        {
            ulong ret = ImGuiNative.igImFileGetSize(file);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static IntPtr ImFileLoadToMemory(ReadOnlySpan<char> filename, ReadOnlySpan<char> mode)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            uint* out_file_size = null;
            int padding_bytes = 0;
            void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, out_file_size, padding_bytes);
            if (filename_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_filename);
            }
            if (mode_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_mode);
            }
            return (IntPtr)ret;
        }
#endif
        public static IntPtr ImFileLoadToMemory(string filename, string mode)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            uint* out_file_size = null;
            int padding_bytes = 0;
            void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, out_file_size, padding_bytes);
            if (filename_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_filename);
            }
            if (mode_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_mode);
            }
            return (IntPtr)ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static IntPtr ImFileLoadToMemory(ReadOnlySpan<char> filename, ReadOnlySpan<char> mode, out uint out_file_size)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            int padding_bytes = 0;
            fixed (uint* native_out_file_size = &out_file_size)
            {
                void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, native_out_file_size, padding_bytes);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_filename);
                }
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_mode);
                }
                return (IntPtr)ret;
            }
        }
#endif
        public static IntPtr ImFileLoadToMemory(string filename, string mode, out uint out_file_size)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            int padding_bytes = 0;
            fixed (uint* native_out_file_size = &out_file_size)
            {
                void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, native_out_file_size, padding_bytes);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_filename);
                }
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_mode);
                }
                return (IntPtr)ret;
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static IntPtr ImFileLoadToMemory(ReadOnlySpan<char> filename, ReadOnlySpan<char> mode, out uint out_file_size, int padding_bytes)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            fixed (uint* native_out_file_size = &out_file_size)
            {
                void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, native_out_file_size, padding_bytes);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_filename);
                }
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_mode);
                }
                return (IntPtr)ret;
            }
        }
#endif
        public static IntPtr ImFileLoadToMemory(string filename, string mode, out uint out_file_size, int padding_bytes)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            fixed (uint* native_out_file_size = &out_file_size)
            {
                void* ret = ImGuiNative.igImFileLoadToMemory(native_filename, native_mode, native_out_file_size, padding_bytes);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_filename);
                }
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_mode);
                }
                return (IntPtr)ret;
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static IntPtr ImFileOpen(ReadOnlySpan<char> filename, ReadOnlySpan<char> mode)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            IntPtr ret = ImGuiNative.igImFileOpen(native_filename, native_mode);
            if (filename_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_filename);
            }
            if (mode_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_mode);
            }
            return ret;
        }
#endif
        public static IntPtr ImFileOpen(string filename, string mode)
        {
            byte* native_filename;
            int filename_byteCount = 0;
            if (filename != null)
            {
                filename_byteCount = Encoding.UTF8.GetByteCount(filename);
                if (filename_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_filename = Util.Allocate(filename_byteCount + 1);
                }
                else
                {
                    byte* native_filename_stackBytes = stackalloc byte[filename_byteCount + 1];
                    native_filename = native_filename_stackBytes;
                }
                int native_filename_offset = Util.GetUtf8(filename, native_filename, filename_byteCount);
                native_filename[native_filename_offset] = 0;
            }
            else { native_filename = null; }
            byte* native_mode;
            int mode_byteCount = 0;
            if (mode != null)
            {
                mode_byteCount = Encoding.UTF8.GetByteCount(mode);
                if (mode_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_mode = Util.Allocate(mode_byteCount + 1);
                }
                else
                {
                    byte* native_mode_stackBytes = stackalloc byte[mode_byteCount + 1];
                    native_mode = native_mode_stackBytes;
                }
                int native_mode_offset = Util.GetUtf8(mode, native_mode, mode_byteCount);
                native_mode[native_mode_offset] = 0;
            }
            else { native_mode = null; }
            IntPtr ret = ImGuiNative.igImFileOpen(native_filename, native_mode);
            if (filename_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_filename);
            }
            if (mode_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_mode);
            }
            return ret;
        }
        public static ulong ImFileRead(IntPtr data, ulong size, ulong count, IntPtr file)
        {
            void* native_data = (void*)data.ToPointer();
            ulong ret = ImGuiNative.igImFileRead(native_data, size, count, file);
            return ret;
        }
        public static ulong ImFileWrite(IntPtr data, ulong size, ulong count, IntPtr file)
        {
            void* native_data = (void*)data.ToPointer();
            ulong ret = ImGuiNative.igImFileWrite(native_data, size, count, file);
            return ret;
        }
        public static float ImFloor(float f)
        {
            float ret = ImGuiNative.igImFloor_Float(f);
            return ret;
        }
        public static Vector2 ImFloor(Vector2 v)
        {
            Vector2 __retval;
            ImGuiNative.igImFloor_Vec2(&__retval, v);
            return __retval;
        }
        public static float ImFloorSigned(float f)
        {
            float ret = ImGuiNative.igImFloorSigned_Float(f);
            return ret;
        }
        public static Vector2 ImFloorSigned(Vector2 v)
        {
            Vector2 __retval;
            ImGuiNative.igImFloorSigned_Vec2(&__retval, v);
            return __retval;
        }
        public static void ImFontAtlasBuildFinish(ImFontAtlasPtr atlas)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            ImGuiNative.igImFontAtlasBuildFinish(native_atlas);
        }
        public static void ImFontAtlasBuildInit(ImFontAtlasPtr atlas)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            ImGuiNative.igImFontAtlasBuildInit(native_atlas);
        }
        public static void ImFontAtlasBuildMultiplyCalcLookupTable(out byte out_table, float in_multiply_factor)
        {
            fixed (byte* native_out_table = &out_table)
            {
                ImGuiNative.igImFontAtlasBuildMultiplyCalcLookupTable(native_out_table, in_multiply_factor);
            }
        }
        public static void ImFontAtlasBuildMultiplyRectAlpha8(ref byte table, ref byte pixels, int x, int y, int w, int h, int stride)
        {
            fixed (byte* native_table = &table)
            {
                fixed (byte* native_pixels = &pixels)
                {
                    ImGuiNative.igImFontAtlasBuildMultiplyRectAlpha8(native_table, native_pixels, x, y, w, h, stride);
                }
            }
        }
        public static void ImFontAtlasBuildPackCustomRects(ImFontAtlasPtr atlas, IntPtr stbrp_context_opaque)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            void* native_stbrp_context_opaque = (void*)stbrp_context_opaque.ToPointer();
            ImGuiNative.igImFontAtlasBuildPackCustomRects(native_atlas, native_stbrp_context_opaque);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ImFontAtlasBuildRender32bppRectFromString(ImFontAtlasPtr atlas, int x, int y, int w, int h, ReadOnlySpan<char> in_str, byte in_marker_char, uint in_marker_pixel_value)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            byte* native_in_str;
            int in_str_byteCount = 0;
            if (in_str != null)
            {
                in_str_byteCount = Encoding.UTF8.GetByteCount(in_str);
                if (in_str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_str = Util.Allocate(in_str_byteCount + 1);
                }
                else
                {
                    byte* native_in_str_stackBytes = stackalloc byte[in_str_byteCount + 1];
                    native_in_str = native_in_str_stackBytes;
                }
                int native_in_str_offset = Util.GetUtf8(in_str, native_in_str, in_str_byteCount);
                native_in_str[native_in_str_offset] = 0;
            }
            else { native_in_str = null; }
            ImGuiNative.igImFontAtlasBuildRender32bppRectFromString(native_atlas, x, y, w, h, native_in_str, in_marker_char, in_marker_pixel_value);
            if (in_str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_str);
            }
        }
#endif
        public static void ImFontAtlasBuildRender32bppRectFromString(ImFontAtlasPtr atlas, int x, int y, int w, int h, string in_str, byte in_marker_char, uint in_marker_pixel_value)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            byte* native_in_str;
            int in_str_byteCount = 0;
            if (in_str != null)
            {
                in_str_byteCount = Encoding.UTF8.GetByteCount(in_str);
                if (in_str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_str = Util.Allocate(in_str_byteCount + 1);
                }
                else
                {
                    byte* native_in_str_stackBytes = stackalloc byte[in_str_byteCount + 1];
                    native_in_str = native_in_str_stackBytes;
                }
                int native_in_str_offset = Util.GetUtf8(in_str, native_in_str, in_str_byteCount);
                native_in_str[native_in_str_offset] = 0;
            }
            else { native_in_str = null; }
            ImGuiNative.igImFontAtlasBuildRender32bppRectFromString(native_atlas, x, y, w, h, native_in_str, in_marker_char, in_marker_pixel_value);
            if (in_str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_str);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ImFontAtlasBuildRender8bppRectFromString(ImFontAtlasPtr atlas, int x, int y, int w, int h, ReadOnlySpan<char> in_str, byte in_marker_char, byte in_marker_pixel_value)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            byte* native_in_str;
            int in_str_byteCount = 0;
            if (in_str != null)
            {
                in_str_byteCount = Encoding.UTF8.GetByteCount(in_str);
                if (in_str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_str = Util.Allocate(in_str_byteCount + 1);
                }
                else
                {
                    byte* native_in_str_stackBytes = stackalloc byte[in_str_byteCount + 1];
                    native_in_str = native_in_str_stackBytes;
                }
                int native_in_str_offset = Util.GetUtf8(in_str, native_in_str, in_str_byteCount);
                native_in_str[native_in_str_offset] = 0;
            }
            else { native_in_str = null; }
            ImGuiNative.igImFontAtlasBuildRender8bppRectFromString(native_atlas, x, y, w, h, native_in_str, in_marker_char, in_marker_pixel_value);
            if (in_str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_str);
            }
        }
#endif
        public static void ImFontAtlasBuildRender8bppRectFromString(ImFontAtlasPtr atlas, int x, int y, int w, int h, string in_str, byte in_marker_char, byte in_marker_pixel_value)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            byte* native_in_str;
            int in_str_byteCount = 0;
            if (in_str != null)
            {
                in_str_byteCount = Encoding.UTF8.GetByteCount(in_str);
                if (in_str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_str = Util.Allocate(in_str_byteCount + 1);
                }
                else
                {
                    byte* native_in_str_stackBytes = stackalloc byte[in_str_byteCount + 1];
                    native_in_str = native_in_str_stackBytes;
                }
                int native_in_str_offset = Util.GetUtf8(in_str, native_in_str, in_str_byteCount);
                native_in_str[native_in_str_offset] = 0;
            }
            else { native_in_str = null; }
            ImGuiNative.igImFontAtlasBuildRender8bppRectFromString(native_atlas, x, y, w, h, native_in_str, in_marker_char, in_marker_pixel_value);
            if (in_str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_str);
            }
        }
        public static void ImFontAtlasBuildSetupFont(ImFontAtlasPtr atlas, ImFontPtr font, ImFontConfigPtr font_config, float ascent, float descent)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            ImFont* native_font = font.NativePtr;
            ImFontConfig* native_font_config = font_config.NativePtr;
            ImGuiNative.igImFontAtlasBuildSetupFont(native_atlas, native_font, native_font_config, ascent, descent);
        }
        public static IntPtr* ImFontAtlasGetBuilderForStbTruetype()
        {
            IntPtr* ret = ImGuiNative.igImFontAtlasGetBuilderForStbTruetype();
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImFormatString(ReadOnlySpan<char> buf, uint buf_size, ReadOnlySpan<char> fmt)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte* native_fmt;
            int fmt_byteCount = 0;
            if (fmt != null)
            {
                fmt_byteCount = Encoding.UTF8.GetByteCount(fmt);
                if (fmt_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt = Util.Allocate(fmt_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_stackBytes = stackalloc byte[fmt_byteCount + 1];
                    native_fmt = native_fmt_stackBytes;
                }
                int native_fmt_offset = Util.GetUtf8(fmt, native_fmt, fmt_byteCount);
                native_fmt[native_fmt_offset] = 0;
            }
            else { native_fmt = null; }
            int ret = ImGuiNative.igImFormatString(native_buf, buf_size, native_fmt);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (fmt_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt);
            }
            return ret;
        }
#endif
        public static int ImFormatString(string buf, uint buf_size, string fmt)
        {
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte* native_fmt;
            int fmt_byteCount = 0;
            if (fmt != null)
            {
                fmt_byteCount = Encoding.UTF8.GetByteCount(fmt);
                if (fmt_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt = Util.Allocate(fmt_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_stackBytes = stackalloc byte[fmt_byteCount + 1];
                    native_fmt = native_fmt_stackBytes;
                }
                int native_fmt_offset = Util.GetUtf8(fmt, native_fmt, fmt_byteCount);
                native_fmt[native_fmt_offset] = 0;
            }
            else { native_fmt = null; }
            int ret = ImGuiNative.igImFormatString(native_buf, buf_size, native_fmt);
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            if (fmt_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt);
            }
            return ret;
        }
        public static uint ImHashData(IntPtr data, uint data_size)
        {
            void* native_data = (void*)data.ToPointer();
            uint seed = 0;
            uint ret = ImGuiNative.igImHashData(native_data, data_size, seed);
            return ret;
        }
        public static uint ImHashData(IntPtr data, uint data_size, uint seed)
        {
            void* native_data = (void*)data.ToPointer();
            uint ret = ImGuiNative.igImHashData(native_data, data_size, seed);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static uint ImHashStr(ReadOnlySpan<char> data)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint data_size = 0;
            uint seed = 0;
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
#endif
        public static uint ImHashStr(string data)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint data_size = 0;
            uint seed = 0;
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static uint ImHashStr(ReadOnlySpan<char> data, uint data_size)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint seed = 0;
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
#endif
        public static uint ImHashStr(string data, uint data_size)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint seed = 0;
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static uint ImHashStr(ReadOnlySpan<char> data, uint data_size, uint seed)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
#endif
        public static uint ImHashStr(string data, uint data_size, uint seed)
        {
            byte* native_data;
            int data_byteCount = 0;
            if (data != null)
            {
                data_byteCount = Encoding.UTF8.GetByteCount(data);
                if (data_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_data = Util.Allocate(data_byteCount + 1);
                }
                else
                {
                    byte* native_data_stackBytes = stackalloc byte[data_byteCount + 1];
                    native_data = native_data_stackBytes;
                }
                int native_data_offset = Util.GetUtf8(data, native_data, data_byteCount);
                native_data[native_data_offset] = 0;
            }
            else { native_data = null; }
            uint ret = ImGuiNative.igImHashStr(native_data, data_size, seed);
            if (data_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_data);
            }
            return ret;
        }
        public static float ImInvLength(Vector2 lhs, float fail_value)
        {
            float ret = ImGuiNative.igImInvLength(lhs, fail_value);
            return ret;
        }
        public static bool ImIsFloatAboveGuaranteedIntegerPrecision(float f)
        {
            byte ret = ImGuiNative.igImIsFloatAboveGuaranteedIntegerPrecision(f);
            return ret != 0;
        }
        public static bool ImIsPowerOfTwo(int v)
        {
            byte ret = ImGuiNative.igImIsPowerOfTwo_Int(v);
            return ret != 0;
        }
        public static bool ImIsPowerOfTwo(ulong v)
        {
            byte ret = ImGuiNative.igImIsPowerOfTwo_U64(v);
            return ret != 0;
        }
        public static float ImLengthSqr(Vector2 lhs)
        {
            float ret = ImGuiNative.igImLengthSqr_Vec2(lhs);
            return ret;
        }
        public static float ImLengthSqr(Vector4 lhs)
        {
            float ret = ImGuiNative.igImLengthSqr_Vec4(lhs);
            return ret;
        }
        public static Vector2 ImLerp(Vector2 a, Vector2 b, float t)
        {
            Vector2 __retval;
            ImGuiNative.igImLerp_Vec2Float(&__retval, a, b, t);
            return __retval;
        }
        public static Vector2 ImLerp(Vector2 a, Vector2 b, Vector2 t)
        {
            Vector2 __retval;
            ImGuiNative.igImLerp_Vec2Vec2(&__retval, a, b, t);
            return __retval;
        }
        public static Vector4 ImLerp(Vector4 a, Vector4 b, float t)
        {
            Vector4 __retval;
            ImGuiNative.igImLerp_Vec4(&__retval, a, b, t);
            return __retval;
        }
        public static float ImLinearSweep(float current, float target, float speed)
        {
            float ret = ImGuiNative.igImLinearSweep(current, target, speed);
            return ret;
        }
        public static Vector2 ImLineClosestPoint(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 __retval;
            ImGuiNative.igImLineClosestPoint(&__retval, a, b, p);
            return __retval;
        }
        public static float ImLog(float x)
        {
            float ret = ImGuiNative.igImLog_Float(x);
            return ret;
        }
        public static double ImLog(double x)
        {
            double ret = ImGuiNative.igImLog_double(x);
            return ret;
        }
        public static Vector2 ImMax(Vector2 lhs, Vector2 rhs)
        {
            Vector2 __retval;
            ImGuiNative.igImMax(&__retval, lhs, rhs);
            return __retval;
        }
        public static Vector2 ImMin(Vector2 lhs, Vector2 rhs)
        {
            Vector2 __retval;
            ImGuiNative.igImMin(&__retval, lhs, rhs);
            return __retval;
        }
        public static int ImModPositive(int a, int b)
        {
            int ret = ImGuiNative.igImModPositive(a, b);
            return ret;
        }
        public static Vector2 ImMul(Vector2 lhs, Vector2 rhs)
        {
            Vector2 __retval;
            ImGuiNative.igImMul(&__retval, lhs, rhs);
            return __retval;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImParseFormatFindEnd(ReadOnlySpan<char> format)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* ret = ImGuiNative.igImParseFormatFindEnd(native_format);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImParseFormatFindEnd(string format)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* ret = ImGuiNative.igImParseFormatFindEnd(native_format);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImParseFormatFindStart(ReadOnlySpan<char> format)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* ret = ImGuiNative.igImParseFormatFindStart(native_format);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImParseFormatFindStart(string format)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* ret = ImGuiNative.igImParseFormatFindStart(native_format);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImParseFormatPrecision(ReadOnlySpan<char> format, int default_value)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            int ret = ImGuiNative.igImParseFormatPrecision(native_format, default_value);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret;
        }
#endif
        public static int ImParseFormatPrecision(string format, int default_value)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            int ret = ImGuiNative.igImParseFormatPrecision(native_format, default_value);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ImParseFormatSanitizeForPrinting(ReadOnlySpan<char> fmt_in, ReadOnlySpan<char> fmt_out, uint fmt_out_size)
        {
            byte* native_fmt_in;
            int fmt_in_byteCount = 0;
            if (fmt_in != null)
            {
                fmt_in_byteCount = Encoding.UTF8.GetByteCount(fmt_in);
                if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_in = Util.Allocate(fmt_in_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_in_stackBytes = stackalloc byte[fmt_in_byteCount + 1];
                    native_fmt_in = native_fmt_in_stackBytes;
                }
                int native_fmt_in_offset = Util.GetUtf8(fmt_in, native_fmt_in, fmt_in_byteCount);
                native_fmt_in[native_fmt_in_offset] = 0;
            }
            else { native_fmt_in = null; }
            byte* native_fmt_out;
            int fmt_out_byteCount = 0;
            if (fmt_out != null)
            {
                fmt_out_byteCount = Encoding.UTF8.GetByteCount(fmt_out);
                if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_out = Util.Allocate(fmt_out_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_out_stackBytes = stackalloc byte[fmt_out_byteCount + 1];
                    native_fmt_out = native_fmt_out_stackBytes;
                }
                int native_fmt_out_offset = Util.GetUtf8(fmt_out, native_fmt_out, fmt_out_byteCount);
                native_fmt_out[native_fmt_out_offset] = 0;
            }
            else { native_fmt_out = null; }
            ImGuiNative.igImParseFormatSanitizeForPrinting(native_fmt_in, native_fmt_out, fmt_out_size);
            if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_in);
            }
            if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_out);
            }
        }
#endif
        public static void ImParseFormatSanitizeForPrinting(string fmt_in, string fmt_out, uint fmt_out_size)
        {
            byte* native_fmt_in;
            int fmt_in_byteCount = 0;
            if (fmt_in != null)
            {
                fmt_in_byteCount = Encoding.UTF8.GetByteCount(fmt_in);
                if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_in = Util.Allocate(fmt_in_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_in_stackBytes = stackalloc byte[fmt_in_byteCount + 1];
                    native_fmt_in = native_fmt_in_stackBytes;
                }
                int native_fmt_in_offset = Util.GetUtf8(fmt_in, native_fmt_in, fmt_in_byteCount);
                native_fmt_in[native_fmt_in_offset] = 0;
            }
            else { native_fmt_in = null; }
            byte* native_fmt_out;
            int fmt_out_byteCount = 0;
            if (fmt_out != null)
            {
                fmt_out_byteCount = Encoding.UTF8.GetByteCount(fmt_out);
                if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_out = Util.Allocate(fmt_out_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_out_stackBytes = stackalloc byte[fmt_out_byteCount + 1];
                    native_fmt_out = native_fmt_out_stackBytes;
                }
                int native_fmt_out_offset = Util.GetUtf8(fmt_out, native_fmt_out, fmt_out_byteCount);
                native_fmt_out[native_fmt_out_offset] = 0;
            }
            else { native_fmt_out = null; }
            ImGuiNative.igImParseFormatSanitizeForPrinting(native_fmt_in, native_fmt_out, fmt_out_size);
            if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_in);
            }
            if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_out);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImParseFormatSanitizeForScanning(ReadOnlySpan<char> fmt_in, ReadOnlySpan<char> fmt_out, uint fmt_out_size)
        {
            byte* native_fmt_in;
            int fmt_in_byteCount = 0;
            if (fmt_in != null)
            {
                fmt_in_byteCount = Encoding.UTF8.GetByteCount(fmt_in);
                if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_in = Util.Allocate(fmt_in_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_in_stackBytes = stackalloc byte[fmt_in_byteCount + 1];
                    native_fmt_in = native_fmt_in_stackBytes;
                }
                int native_fmt_in_offset = Util.GetUtf8(fmt_in, native_fmt_in, fmt_in_byteCount);
                native_fmt_in[native_fmt_in_offset] = 0;
            }
            else { native_fmt_in = null; }
            byte* native_fmt_out;
            int fmt_out_byteCount = 0;
            if (fmt_out != null)
            {
                fmt_out_byteCount = Encoding.UTF8.GetByteCount(fmt_out);
                if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_out = Util.Allocate(fmt_out_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_out_stackBytes = stackalloc byte[fmt_out_byteCount + 1];
                    native_fmt_out = native_fmt_out_stackBytes;
                }
                int native_fmt_out_offset = Util.GetUtf8(fmt_out, native_fmt_out, fmt_out_byteCount);
                native_fmt_out[native_fmt_out_offset] = 0;
            }
            else { native_fmt_out = null; }
            byte* ret = ImGuiNative.igImParseFormatSanitizeForScanning(native_fmt_in, native_fmt_out, fmt_out_size);
            if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_in);
            }
            if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_out);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImParseFormatSanitizeForScanning(string fmt_in, string fmt_out, uint fmt_out_size)
        {
            byte* native_fmt_in;
            int fmt_in_byteCount = 0;
            if (fmt_in != null)
            {
                fmt_in_byteCount = Encoding.UTF8.GetByteCount(fmt_in);
                if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_in = Util.Allocate(fmt_in_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_in_stackBytes = stackalloc byte[fmt_in_byteCount + 1];
                    native_fmt_in = native_fmt_in_stackBytes;
                }
                int native_fmt_in_offset = Util.GetUtf8(fmt_in, native_fmt_in, fmt_in_byteCount);
                native_fmt_in[native_fmt_in_offset] = 0;
            }
            else { native_fmt_in = null; }
            byte* native_fmt_out;
            int fmt_out_byteCount = 0;
            if (fmt_out != null)
            {
                fmt_out_byteCount = Encoding.UTF8.GetByteCount(fmt_out);
                if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_fmt_out = Util.Allocate(fmt_out_byteCount + 1);
                }
                else
                {
                    byte* native_fmt_out_stackBytes = stackalloc byte[fmt_out_byteCount + 1];
                    native_fmt_out = native_fmt_out_stackBytes;
                }
                int native_fmt_out_offset = Util.GetUtf8(fmt_out, native_fmt_out, fmt_out_byteCount);
                native_fmt_out[native_fmt_out_offset] = 0;
            }
            else { native_fmt_out = null; }
            byte* ret = ImGuiNative.igImParseFormatSanitizeForScanning(native_fmt_in, native_fmt_out, fmt_out_size);
            if (fmt_in_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_in);
            }
            if (fmt_out_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_fmt_out);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImParseFormatTrimDecorations(ReadOnlySpan<char> format, ReadOnlySpan<char> buf, uint buf_size)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte* ret = ImGuiNative.igImParseFormatTrimDecorations(native_format, native_buf, buf_size);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImParseFormatTrimDecorations(string format, string buf, uint buf_size)
        {
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte* ret = ImGuiNative.igImParseFormatTrimDecorations(native_format, native_buf, buf_size);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return Util.StringFromPtr(ret);
        }
        public static float ImPow(float x, float y)
        {
            float ret = ImGuiNative.igImPow_Float(x, y);
            return ret;
        }
        public static double ImPow(double x, double y)
        {
            double ret = ImGuiNative.igImPow_double(x, y);
            return ret;
        }
        public static Vector2 ImRotate(Vector2 v, float cos_a, float sin_a)
        {
            Vector2 __retval;
            ImGuiNative.igImRotate(&__retval, v, cos_a, sin_a);
            return __retval;
        }
        public static float ImRsqrt(float x)
        {
            float ret = ImGuiNative.igImRsqrt_Float(x);
            return ret;
        }
        public static double ImRsqrt(double x)
        {
            double ret = ImGuiNative.igImRsqrt_double(x);
            return ret;
        }
        public static float ImSaturate(float f)
        {
            float ret = ImGuiNative.igImSaturate(f);
            return ret;
        }
        public static float ImSign(float x)
        {
            float ret = ImGuiNative.igImSign_Float(x);
            return ret;
        }
        public static double ImSign(double x)
        {
            double ret = ImGuiNative.igImSign_double(x);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStrchrRange(ReadOnlySpan<char> str_begin, byte c)
        {
            byte* native_str_begin;
            int str_begin_byteCount = 0;
                str_begin_byteCount = Encoding.UTF8.GetByteCount(str_begin);
                if (str_begin_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_begin = Util.Allocate(str_begin_byteCount + 1);
                }
                else
                {
                    byte* native_str_begin_stackBytes = stackalloc byte[str_begin_byteCount + 1];
                    native_str_begin = native_str_begin_stackBytes;
                }
                int native_str_begin_offset = Util.GetUtf8(str_begin, native_str_begin, str_begin_byteCount);
                native_str_begin[native_str_begin_offset] = 0;
            byte* ret = ImGuiNative.igImStrchrRange(native_str_begin, native_str_begin+str_begin_byteCount, c);
            if (str_begin_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_begin);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImStrchrRange(string str_begin, byte c)
        {
            byte* native_str_begin;
            int str_begin_byteCount = 0;
                str_begin_byteCount = Encoding.UTF8.GetByteCount(str_begin);
                if (str_begin_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str_begin = Util.Allocate(str_begin_byteCount + 1);
                }
                else
                {
                    byte* native_str_begin_stackBytes = stackalloc byte[str_begin_byteCount + 1];
                    native_str_begin = native_str_begin_stackBytes;
                }
                int native_str_begin_offset = Util.GetUtf8(str_begin, native_str_begin, str_begin_byteCount);
                native_str_begin[native_str_begin_offset] = 0;
            byte* ret = ImGuiNative.igImStrchrRange(native_str_begin, native_str_begin+str_begin_byteCount, c);
            if (str_begin_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str_begin);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStrdup(ReadOnlySpan<char> str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStrdup(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImStrdup(string str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStrdup(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStrdupcpy(ReadOnlySpan<char> dst, ref uint p_dst_size, ReadOnlySpan<char> str)
        {
            byte* native_dst;
            int dst_byteCount = 0;
            if (dst != null)
            {
                dst_byteCount = Encoding.UTF8.GetByteCount(dst);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst = Util.Allocate(dst_byteCount + 1);
                }
                else
                {
                    byte* native_dst_stackBytes = stackalloc byte[dst_byteCount + 1];
                    native_dst = native_dst_stackBytes;
                }
                int native_dst_offset = Util.GetUtf8(dst, native_dst, dst_byteCount);
                native_dst[native_dst_offset] = 0;
            }
            else { native_dst = null; }
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            fixed (uint* native_p_dst_size = &p_dst_size)
            {
                byte* ret = ImGuiNative.igImStrdupcpy(native_dst, native_p_dst_size, native_str);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_dst);
                }
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_str);
                }
                return Util.StringFromPtr(ret);
            }
        }
#endif
        public static string ImStrdupcpy(string dst, ref uint p_dst_size, string str)
        {
            byte* native_dst;
            int dst_byteCount = 0;
            if (dst != null)
            {
                dst_byteCount = Encoding.UTF8.GetByteCount(dst);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst = Util.Allocate(dst_byteCount + 1);
                }
                else
                {
                    byte* native_dst_stackBytes = stackalloc byte[dst_byteCount + 1];
                    native_dst = native_dst_stackBytes;
                }
                int native_dst_offset = Util.GetUtf8(dst, native_dst, dst_byteCount);
                native_dst[native_dst_offset] = 0;
            }
            else { native_dst = null; }
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            fixed (uint* native_p_dst_size = &p_dst_size)
            {
                byte* ret = ImGuiNative.igImStrdupcpy(native_dst, native_p_dst_size, native_str);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_dst);
                }
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_str);
                }
                return Util.StringFromPtr(ret);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStreolRange(ReadOnlySpan<char> str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStreolRange(native_str, native_str+str_byteCount);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImStreolRange(string str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStreolRange(native_str, native_str+str_byteCount);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImStricmp(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            byte* native_str1;
            int str1_byteCount = 0;
            if (str1 != null)
            {
                str1_byteCount = Encoding.UTF8.GetByteCount(str1);
                if (str1_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str1 = Util.Allocate(str1_byteCount + 1);
                }
                else
                {
                    byte* native_str1_stackBytes = stackalloc byte[str1_byteCount + 1];
                    native_str1 = native_str1_stackBytes;
                }
                int native_str1_offset = Util.GetUtf8(str1, native_str1, str1_byteCount);
                native_str1[native_str1_offset] = 0;
            }
            else { native_str1 = null; }
            byte* native_str2;
            int str2_byteCount = 0;
            if (str2 != null)
            {
                str2_byteCount = Encoding.UTF8.GetByteCount(str2);
                if (str2_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str2 = Util.Allocate(str2_byteCount + 1);
                }
                else
                {
                    byte* native_str2_stackBytes = stackalloc byte[str2_byteCount + 1];
                    native_str2 = native_str2_stackBytes;
                }
                int native_str2_offset = Util.GetUtf8(str2, native_str2, str2_byteCount);
                native_str2[native_str2_offset] = 0;
            }
            else { native_str2 = null; }
            int ret = ImGuiNative.igImStricmp(native_str1, native_str2);
            if (str1_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str1);
            }
            if (str2_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str2);
            }
            return ret;
        }
#endif
        public static int ImStricmp(string str1, string str2)
        {
            byte* native_str1;
            int str1_byteCount = 0;
            if (str1 != null)
            {
                str1_byteCount = Encoding.UTF8.GetByteCount(str1);
                if (str1_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str1 = Util.Allocate(str1_byteCount + 1);
                }
                else
                {
                    byte* native_str1_stackBytes = stackalloc byte[str1_byteCount + 1];
                    native_str1 = native_str1_stackBytes;
                }
                int native_str1_offset = Util.GetUtf8(str1, native_str1, str1_byteCount);
                native_str1[native_str1_offset] = 0;
            }
            else { native_str1 = null; }
            byte* native_str2;
            int str2_byteCount = 0;
            if (str2 != null)
            {
                str2_byteCount = Encoding.UTF8.GetByteCount(str2);
                if (str2_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str2 = Util.Allocate(str2_byteCount + 1);
                }
                else
                {
                    byte* native_str2_stackBytes = stackalloc byte[str2_byteCount + 1];
                    native_str2 = native_str2_stackBytes;
                }
                int native_str2_offset = Util.GetUtf8(str2, native_str2, str2_byteCount);
                native_str2[native_str2_offset] = 0;
            }
            else { native_str2 = null; }
            int ret = ImGuiNative.igImStricmp(native_str1, native_str2);
            if (str1_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str1);
            }
            if (str2_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str2);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStristr(ReadOnlySpan<char> haystack, ReadOnlySpan<char> needle)
        {
            byte* native_haystack;
            int haystack_byteCount = 0;
            if (haystack != null)
            {
                haystack_byteCount = Encoding.UTF8.GetByteCount(haystack);
                if (haystack_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_haystack = Util.Allocate(haystack_byteCount + 1);
                }
                else
                {
                    byte* native_haystack_stackBytes = stackalloc byte[haystack_byteCount + 1];
                    native_haystack = native_haystack_stackBytes;
                }
                int native_haystack_offset = Util.GetUtf8(haystack, native_haystack, haystack_byteCount);
                native_haystack[native_haystack_offset] = 0;
            }
            else { native_haystack = null; }
            byte* native_needle;
            int needle_byteCount = 0;
            if (needle != null)
            {
                needle_byteCount = Encoding.UTF8.GetByteCount(needle);
                if (needle_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_needle = Util.Allocate(needle_byteCount + 1);
                }
                else
                {
                    byte* native_needle_stackBytes = stackalloc byte[needle_byteCount + 1];
                    native_needle = native_needle_stackBytes;
                }
                int native_needle_offset = Util.GetUtf8(needle, native_needle, needle_byteCount);
                native_needle[native_needle_offset] = 0;
            }
            else { native_needle = null; }
            byte* ret = ImGuiNative.igImStristr(native_haystack, native_haystack+haystack_byteCount, native_needle, native_needle+needle_byteCount);
            if (haystack_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_haystack);
            }
            if (needle_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_needle);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImStristr(string haystack, string needle)
        {
            byte* native_haystack;
            int haystack_byteCount = 0;
            if (haystack != null)
            {
                haystack_byteCount = Encoding.UTF8.GetByteCount(haystack);
                if (haystack_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_haystack = Util.Allocate(haystack_byteCount + 1);
                }
                else
                {
                    byte* native_haystack_stackBytes = stackalloc byte[haystack_byteCount + 1];
                    native_haystack = native_haystack_stackBytes;
                }
                int native_haystack_offset = Util.GetUtf8(haystack, native_haystack, haystack_byteCount);
                native_haystack[native_haystack_offset] = 0;
            }
            else { native_haystack = null; }
            byte* native_needle;
            int needle_byteCount = 0;
            if (needle != null)
            {
                needle_byteCount = Encoding.UTF8.GetByteCount(needle);
                if (needle_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_needle = Util.Allocate(needle_byteCount + 1);
                }
                else
                {
                    byte* native_needle_stackBytes = stackalloc byte[needle_byteCount + 1];
                    native_needle = native_needle_stackBytes;
                }
                int native_needle_offset = Util.GetUtf8(needle, native_needle, needle_byteCount);
                native_needle[native_needle_offset] = 0;
            }
            else { native_needle = null; }
            byte* ret = ImGuiNative.igImStristr(native_haystack, native_haystack+haystack_byteCount, native_needle, native_needle+needle_byteCount);
            if (haystack_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_haystack);
            }
            if (needle_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_needle);
            }
            return Util.StringFromPtr(ret);
        }
        public static int ImStrlenW(IntPtr str)
        {
            ushort* native_str = (ushort*)str.ToPointer();
            int ret = ImGuiNative.igImStrlenW(native_str);
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ImStrncpy(ReadOnlySpan<char> dst, ReadOnlySpan<char> src, uint count)
        {
            byte* native_dst;
            int dst_byteCount = 0;
            if (dst != null)
            {
                dst_byteCount = Encoding.UTF8.GetByteCount(dst);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst = Util.Allocate(dst_byteCount + 1);
                }
                else
                {
                    byte* native_dst_stackBytes = stackalloc byte[dst_byteCount + 1];
                    native_dst = native_dst_stackBytes;
                }
                int native_dst_offset = Util.GetUtf8(dst, native_dst, dst_byteCount);
                native_dst[native_dst_offset] = 0;
            }
            else { native_dst = null; }
            byte* native_src;
            int src_byteCount = 0;
            if (src != null)
            {
                src_byteCount = Encoding.UTF8.GetByteCount(src);
                if (src_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_src = Util.Allocate(src_byteCount + 1);
                }
                else
                {
                    byte* native_src_stackBytes = stackalloc byte[src_byteCount + 1];
                    native_src = native_src_stackBytes;
                }
                int native_src_offset = Util.GetUtf8(src, native_src, src_byteCount);
                native_src[native_src_offset] = 0;
            }
            else { native_src = null; }
            ImGuiNative.igImStrncpy(native_dst, native_src, count);
            if (dst_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_dst);
            }
            if (src_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_src);
            }
        }
#endif
        public static void ImStrncpy(string dst, string src, uint count)
        {
            byte* native_dst;
            int dst_byteCount = 0;
            if (dst != null)
            {
                dst_byteCount = Encoding.UTF8.GetByteCount(dst);
                if (dst_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_dst = Util.Allocate(dst_byteCount + 1);
                }
                else
                {
                    byte* native_dst_stackBytes = stackalloc byte[dst_byteCount + 1];
                    native_dst = native_dst_stackBytes;
                }
                int native_dst_offset = Util.GetUtf8(dst, native_dst, dst_byteCount);
                native_dst[native_dst_offset] = 0;
            }
            else { native_dst = null; }
            byte* native_src;
            int src_byteCount = 0;
            if (src != null)
            {
                src_byteCount = Encoding.UTF8.GetByteCount(src);
                if (src_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_src = Util.Allocate(src_byteCount + 1);
                }
                else
                {
                    byte* native_src_stackBytes = stackalloc byte[src_byteCount + 1];
                    native_src = native_src_stackBytes;
                }
                int native_src_offset = Util.GetUtf8(src, native_src, src_byteCount);
                native_src[native_src_offset] = 0;
            }
            else { native_src = null; }
            ImGuiNative.igImStrncpy(native_dst, native_src, count);
            if (dst_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_dst);
            }
            if (src_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_src);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImStrnicmp(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, uint count)
        {
            byte* native_str1;
            int str1_byteCount = 0;
            if (str1 != null)
            {
                str1_byteCount = Encoding.UTF8.GetByteCount(str1);
                if (str1_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str1 = Util.Allocate(str1_byteCount + 1);
                }
                else
                {
                    byte* native_str1_stackBytes = stackalloc byte[str1_byteCount + 1];
                    native_str1 = native_str1_stackBytes;
                }
                int native_str1_offset = Util.GetUtf8(str1, native_str1, str1_byteCount);
                native_str1[native_str1_offset] = 0;
            }
            else { native_str1 = null; }
            byte* native_str2;
            int str2_byteCount = 0;
            if (str2 != null)
            {
                str2_byteCount = Encoding.UTF8.GetByteCount(str2);
                if (str2_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str2 = Util.Allocate(str2_byteCount + 1);
                }
                else
                {
                    byte* native_str2_stackBytes = stackalloc byte[str2_byteCount + 1];
                    native_str2 = native_str2_stackBytes;
                }
                int native_str2_offset = Util.GetUtf8(str2, native_str2, str2_byteCount);
                native_str2[native_str2_offset] = 0;
            }
            else { native_str2 = null; }
            int ret = ImGuiNative.igImStrnicmp(native_str1, native_str2, count);
            if (str1_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str1);
            }
            if (str2_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str2);
            }
            return ret;
        }
#endif
        public static int ImStrnicmp(string str1, string str2, uint count)
        {
            byte* native_str1;
            int str1_byteCount = 0;
            if (str1 != null)
            {
                str1_byteCount = Encoding.UTF8.GetByteCount(str1);
                if (str1_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str1 = Util.Allocate(str1_byteCount + 1);
                }
                else
                {
                    byte* native_str1_stackBytes = stackalloc byte[str1_byteCount + 1];
                    native_str1 = native_str1_stackBytes;
                }
                int native_str1_offset = Util.GetUtf8(str1, native_str1, str1_byteCount);
                native_str1[native_str1_offset] = 0;
            }
            else { native_str1 = null; }
            byte* native_str2;
            int str2_byteCount = 0;
            if (str2 != null)
            {
                str2_byteCount = Encoding.UTF8.GetByteCount(str2);
                if (str2_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str2 = Util.Allocate(str2_byteCount + 1);
                }
                else
                {
                    byte* native_str2_stackBytes = stackalloc byte[str2_byteCount + 1];
                    native_str2 = native_str2_stackBytes;
                }
                int native_str2_offset = Util.GetUtf8(str2, native_str2, str2_byteCount);
                native_str2[native_str2_offset] = 0;
            }
            else { native_str2 = null; }
            int ret = ImGuiNative.igImStrnicmp(native_str1, native_str2, count);
            if (str1_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str1);
            }
            if (str2_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str2);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static string ImStrSkipBlank(ReadOnlySpan<char> str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStrSkipBlank(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#endif
        public static string ImStrSkipBlank(string str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            byte* ret = ImGuiNative.igImStrSkipBlank(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
            return Util.StringFromPtr(ret);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void ImStrTrimBlanks(ReadOnlySpan<char> str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            ImGuiNative.igImStrTrimBlanks(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
        }
#endif
        public static void ImStrTrimBlanks(string str)
        {
            byte* native_str;
            int str_byteCount = 0;
            if (str != null)
            {
                str_byteCount = Encoding.UTF8.GetByteCount(str);
                if (str_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_str = Util.Allocate(str_byteCount + 1);
                }
                else
                {
                    byte* native_str_stackBytes = stackalloc byte[str_byteCount + 1];
                    native_str = native_str_stackBytes;
                }
                int native_str_offset = Util.GetUtf8(str, native_str, str_byteCount);
                native_str[native_str_offset] = 0;
            }
            else { native_str = null; }
            ImGuiNative.igImStrTrimBlanks(native_str);
            if (str_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_str);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImTextCharFromUtf8(out uint out_char, ReadOnlySpan<char> in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            fixed (uint* native_out_char = &out_char)
            {
                int ret = ImGuiNative.igImTextCharFromUtf8(native_out_char, native_in_text, native_in_text+in_text_byteCount);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_in_text);
                }
                return ret;
            }
        }
#endif
        public static int ImTextCharFromUtf8(out uint out_char, string in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            fixed (uint* native_out_char = &out_char)
            {
                int ret = ImGuiNative.igImTextCharFromUtf8(native_out_char, native_in_text, native_in_text+in_text_byteCount);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_in_text);
                }
                return ret;
            }
        }
        public static string ImTextCharToUtf8(out byte out_buf, uint c)
        {
            fixed (byte* native_out_buf = &out_buf)
            {
                byte* ret = ImGuiNative.igImTextCharToUtf8(native_out_buf, c);
                return Util.StringFromPtr(ret);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImTextCountCharsFromUtf8(ReadOnlySpan<char> in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            int ret = ImGuiNative.igImTextCountCharsFromUtf8(native_in_text, native_in_text+in_text_byteCount);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#endif
        public static int ImTextCountCharsFromUtf8(string in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            int ret = ImGuiNative.igImTextCountCharsFromUtf8(native_in_text, native_in_text+in_text_byteCount);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImTextCountUtf8BytesFromChar(ReadOnlySpan<char> in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            int ret = ImGuiNative.igImTextCountUtf8BytesFromChar(native_in_text, native_in_text+in_text_byteCount);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#endif
        public static int ImTextCountUtf8BytesFromChar(string in_text)
        {
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            int ret = ImGuiNative.igImTextCountUtf8BytesFromChar(native_in_text, native_in_text+in_text_byteCount);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImTextStrFromUtf8(IntPtr out_buf, int out_buf_size, ReadOnlySpan<char> in_text)
        {
            ushort* native_out_buf = (ushort*)out_buf.ToPointer();
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            byte** in_remaining = null;
            int ret = ImGuiNative.igImTextStrFromUtf8(native_out_buf, out_buf_size, native_in_text, native_in_text+in_text_byteCount, in_remaining);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#endif
        public static int ImTextStrFromUtf8(IntPtr out_buf, int out_buf_size, string in_text)
        {
            ushort* native_out_buf = (ushort*)out_buf.ToPointer();
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            byte** in_remaining = null;
            int ret = ImGuiNative.igImTextStrFromUtf8(native_out_buf, out_buf_size, native_in_text, native_in_text+in_text_byteCount, in_remaining);
            if (in_text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_in_text);
            }
            return ret;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static int ImTextStrFromUtf8(IntPtr out_buf, int out_buf_size, ReadOnlySpan<char> in_text, ref byte* in_remaining)
        {
            ushort* native_out_buf = (ushort*)out_buf.ToPointer();
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            fixed (byte** native_in_remaining = &in_remaining)
            {
                int ret = ImGuiNative.igImTextStrFromUtf8(native_out_buf, out_buf_size, native_in_text, native_in_text+in_text_byteCount, native_in_remaining);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_in_text);
                }
                return ret;
            }
        }
#endif
        public static int ImTextStrFromUtf8(IntPtr out_buf, int out_buf_size, string in_text, ref byte* in_remaining)
        {
            ushort* native_out_buf = (ushort*)out_buf.ToPointer();
            byte* native_in_text;
            int in_text_byteCount = 0;
            if (in_text != null)
            {
                in_text_byteCount = Encoding.UTF8.GetByteCount(in_text);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_in_text = Util.Allocate(in_text_byteCount + 1);
                }
                else
                {
                    byte* native_in_text_stackBytes = stackalloc byte[in_text_byteCount + 1];
                    native_in_text = native_in_text_stackBytes;
                }
                int native_in_text_offset = Util.GetUtf8(in_text, native_in_text, in_text_byteCount);
                native_in_text[native_in_text_offset] = 0;
            }
            else { native_in_text = null; }
            fixed (byte** native_in_remaining = &in_remaining)
            {
                int ret = ImGuiNative.igImTextStrFromUtf8(native_out_buf, out_buf_size, native_in_text, native_in_text+in_text_byteCount, native_in_remaining);
                if (in_text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_in_text);
                }
                return ret;
            }
        }
        public static byte ImToUpper(byte c)
        {
            byte ret = ImGuiNative.igImToUpper(c);
            return ret;
        }
        public static float ImTriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            float ret = ImGuiNative.igImTriangleArea(a, b, c);
            return ret;
        }
        public static void ImTriangleBarycentricCoords(Vector2 a, Vector2 b, Vector2 c, Vector2 p, out float out_u, out float out_v, out float out_w)
        {
            fixed (float* native_out_u = &out_u)
            {
                fixed (float* native_out_v = &out_v)
                {
                    fixed (float* native_out_w = &out_w)
                    {
                        ImGuiNative.igImTriangleBarycentricCoords(a, b, c, p, native_out_u, native_out_v, native_out_w);
                    }
                }
            }
        }
        public static Vector2 ImTriangleClosestPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            Vector2 __retval;
            ImGuiNative.igImTriangleClosestPoint(&__retval, a, b, c, p);
            return __retval;
        }
        public static bool ImTriangleContainsPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            byte ret = ImGuiNative.igImTriangleContainsPoint(a, b, c, p);
            return ret != 0;
        }
        public static int ImUpperPowerOfTwo(int v)
        {
            int ret = ImGuiNative.igImUpperPowerOfTwo(v);
            return ret;
        }
        public static void Initialize()
        {
            ImGuiNative.igInitialize();
        }
        public static void InputTextDeactivateHook(uint id)
        {
            ImGuiNative.igInputTextDeactivateHook(id);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool InputTextEx(ReadOnlySpan<char> label, ReadOnlySpan<char> hint, ReadOnlySpan<char> buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            ImGuiInputTextCallback callback = null;
            void* user_data = null;
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#endif
        public static bool InputTextEx(string label, string hint, string buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            ImGuiInputTextCallback callback = null;
            void* user_data = null;
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool InputTextEx(ReadOnlySpan<char> label, ReadOnlySpan<char> hint, ReadOnlySpan<char> buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* user_data = null;
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#endif
        public static bool InputTextEx(string label, string hint, string buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* user_data = null;
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool InputTextEx(ReadOnlySpan<char> label, ReadOnlySpan<char> hint, ReadOnlySpan<char> buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_user_data = (void*)user_data.ToPointer();
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, native_user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#endif
        public static bool InputTextEx(string label, string hint, string buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, IntPtr user_data)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_hint;
            int hint_byteCount = 0;
            if (hint != null)
            {
                hint_byteCount = Encoding.UTF8.GetByteCount(hint);
                if (hint_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_hint = Util.Allocate(hint_byteCount + 1);
                }
                else
                {
                    byte* native_hint_stackBytes = stackalloc byte[hint_byteCount + 1];
                    native_hint = native_hint_stackBytes;
                }
                int native_hint_offset = Util.GetUtf8(hint, native_hint, hint_byteCount);
                native_hint[native_hint_offset] = 0;
            }
            else { native_hint = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            void* native_user_data = (void*)user_data.ToPointer();
            byte ret = ImGuiNative.igInputTextEx(native_label, native_hint, native_buf, buf_size, size_arg, flags, callback, native_user_data);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (hint_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_hint);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
        public static bool IsActiveIdUsingNavDir(ImGuiDir dir)
        {
            byte ret = ImGuiNative.igIsActiveIdUsingNavDir(dir);
            return ret != 0;
        }
        public static bool IsAliasKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsAliasKey(key);
            return ret != 0;
        }
        public static bool IsClippedEx(ImRect bb, uint id)
        {
            byte ret = ImGuiNative.igIsClippedEx(bb, id);
            return ret != 0;
        }
        public static bool IsDragDropActive()
        {
            byte ret = ImGuiNative.igIsDragDropActive();
            return ret != 0;
        }
        public static bool IsDragDropPayloadBeingAccepted()
        {
            byte ret = ImGuiNative.igIsDragDropPayloadBeingAccepted();
            return ret != 0;
        }
        public static bool IsGamepadKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsGamepadKey(key);
            return ret != 0;
        }
        public static bool IsItemToggledSelection()
        {
            byte ret = ImGuiNative.igIsItemToggledSelection();
            return ret != 0;
        }
        public static bool IsKeyboardKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsKeyboardKey(key);
            return ret != 0;
        }
        public static bool IsKeyDown(ImGuiKey key, uint owner_id)
        {
            byte ret = ImGuiNative.igIsKeyDown_ID(key, owner_id);
            return ret != 0;
        }
        public static bool IsKeyPressed(ImGuiKey key, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igIsKeyPressed_ID(key, owner_id, flags);
            return ret != 0;
        }
        public static bool IsKeyPressed(ImGuiKey key, uint owner_id, ImGuiInputFlags flags)
        {
            byte ret = ImGuiNative.igIsKeyPressed_ID(key, owner_id, flags);
            return ret != 0;
        }
        public static bool IsKeyPressedMap(ImGuiKey key)
        {
            byte repeat = 1;
            byte ret = ImGuiNative.igIsKeyPressedMap(key, repeat);
            return ret != 0;
        }
        public static bool IsKeyPressedMap(ImGuiKey key, bool repeat)
        {
            byte native_repeat = repeat ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igIsKeyPressedMap(key, native_repeat);
            return ret != 0;
        }
        public static bool IsKeyReleased(ImGuiKey key, uint owner_id)
        {
            byte ret = ImGuiNative.igIsKeyReleased_ID(key, owner_id);
            return ret != 0;
        }
        public static bool IsLegacyKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsLegacyKey(key);
            return ret != 0;
        }
        public static bool IsMouseClicked(ImGuiMouseButton button, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igIsMouseClicked_ID(button, owner_id, flags);
            return ret != 0;
        }
        public static bool IsMouseClicked(ImGuiMouseButton button, uint owner_id, ImGuiInputFlags flags)
        {
            byte ret = ImGuiNative.igIsMouseClicked_ID(button, owner_id, flags);
            return ret != 0;
        }
        public static bool IsMouseDown(ImGuiMouseButton button, uint owner_id)
        {
            byte ret = ImGuiNative.igIsMouseDown_ID(button, owner_id);
            return ret != 0;
        }
        public static bool IsMouseDragPastThreshold(ImGuiMouseButton button)
        {
            float lock_threshold = -1.0f;
            byte ret = ImGuiNative.igIsMouseDragPastThreshold(button, lock_threshold);
            return ret != 0;
        }
        public static bool IsMouseDragPastThreshold(ImGuiMouseButton button, float lock_threshold)
        {
            byte ret = ImGuiNative.igIsMouseDragPastThreshold(button, lock_threshold);
            return ret != 0;
        }
        public static bool IsMouseKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsMouseKey(key);
            return ret != 0;
        }
        public static bool IsMouseReleased(ImGuiMouseButton button, uint owner_id)
        {
            byte ret = ImGuiNative.igIsMouseReleased_ID(button, owner_id);
            return ret != 0;
        }
        public static bool IsNamedKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsNamedKey(key);
            return ret != 0;
        }
        public static bool IsNamedKeyOrModKey(ImGuiKey key)
        {
            byte ret = ImGuiNative.igIsNamedKeyOrModKey(key);
            return ret != 0;
        }
        public static bool IsPopupOpen(uint id, ImGuiPopupFlags popup_flags)
        {
            byte ret = ImGuiNative.igIsPopupOpen_ID(id, popup_flags);
            return ret != 0;
        }
        public static bool IsWindowAbove(ImGuiWindowPtr potential_above, ImGuiWindowPtr potential_below)
        {
            ImGuiWindow* native_potential_above = potential_above.NativePtr;
            ImGuiWindow* native_potential_below = potential_below.NativePtr;
            byte ret = ImGuiNative.igIsWindowAbove(native_potential_above, native_potential_below);
            return ret != 0;
        }
        public static bool IsWindowChildOf(ImGuiWindowPtr window, ImGuiWindowPtr potential_parent, bool popup_hierarchy, bool dock_hierarchy)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* native_potential_parent = potential_parent.NativePtr;
            byte native_popup_hierarchy = popup_hierarchy ? (byte)1 : (byte)0;
            byte native_dock_hierarchy = dock_hierarchy ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igIsWindowChildOf(native_window, native_potential_parent, native_popup_hierarchy, native_dock_hierarchy);
            return ret != 0;
        }
        public static bool IsWindowContentHoverable(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiHoveredFlags flags = (ImGuiHoveredFlags)0;
            byte ret = ImGuiNative.igIsWindowContentHoverable(native_window, flags);
            return ret != 0;
        }
        public static bool IsWindowContentHoverable(ImGuiWindowPtr window, ImGuiHoveredFlags flags)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte ret = ImGuiNative.igIsWindowContentHoverable(native_window, flags);
            return ret != 0;
        }
        public static bool IsWindowNavFocusable(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte ret = ImGuiNative.igIsWindowNavFocusable(native_window);
            return ret != 0;
        }
        public static bool IsWindowWithinBeginStackOf(ImGuiWindowPtr window, ImGuiWindowPtr potential_parent)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* native_potential_parent = potential_parent.NativePtr;
            byte ret = ImGuiNative.igIsWindowWithinBeginStackOf(native_window, native_potential_parent);
            return ret != 0;
        }
        public static bool ItemAdd(ImRect bb, uint id)
        {
            ImRect* nav_bb = null;
            ImGuiItemFlags extra_flags = (ImGuiItemFlags)0;
            byte ret = ImGuiNative.igItemAdd(bb, id, nav_bb, extra_flags);
            return ret != 0;
        }
        public static bool ItemAdd(ImRect bb, uint id, ImRectPtr nav_bb)
        {
            ImRect* native_nav_bb = nav_bb.NativePtr;
            ImGuiItemFlags extra_flags = (ImGuiItemFlags)0;
            byte ret = ImGuiNative.igItemAdd(bb, id, native_nav_bb, extra_flags);
            return ret != 0;
        }
        public static bool ItemAdd(ImRect bb, uint id, ImRectPtr nav_bb, ImGuiItemFlags extra_flags)
        {
            ImRect* native_nav_bb = nav_bb.NativePtr;
            byte ret = ImGuiNative.igItemAdd(bb, id, native_nav_bb, extra_flags);
            return ret != 0;
        }
        public static bool ItemHoverable(ImRect bb, uint id, ImGuiItemFlags item_flags)
        {
            byte ret = ImGuiNative.igItemHoverable(bb, id, item_flags);
            return ret != 0;
        }
        public static void ItemSize(Vector2 size)
        {
            float text_baseline_y = -1.0f;
            ImGuiNative.igItemSize_Vec2(size, text_baseline_y);
        }
        public static void ItemSize(Vector2 size, float text_baseline_y)
        {
            ImGuiNative.igItemSize_Vec2(size, text_baseline_y);
        }
        public static void ItemSize(ImRect bb)
        {
            float text_baseline_y = -1.0f;
            ImGuiNative.igItemSize_Rect(bb, text_baseline_y);
        }
        public static void ItemSize(ImRect bb, float text_baseline_y)
        {
            ImGuiNative.igItemSize_Rect(bb, text_baseline_y);
        }
        public static void KeepAliveID(uint id)
        {
            ImGuiNative.igKeepAliveID(id);
        }
        public static string LocalizeGetMsg(ImGuiLocKey key)
        {
            byte* ret = ImGuiNative.igLocalizeGetMsg(key);
            return Util.StringFromPtr(ret);
        }
        public static void LocalizeRegisterEntries(ImGuiLocEntryPtr entries, int count)
        {
            ImGuiLocEntry* native_entries = entries.NativePtr;
            ImGuiNative.igLocalizeRegisterEntries(native_entries, count);
        }
        public static void LogBegin(ImGuiLogType type, int auto_open_depth)
        {
            ImGuiNative.igLogBegin(type, auto_open_depth);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void LogRenderedText(ref Vector2 ref_pos, ReadOnlySpan<char> text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (Vector2* native_ref_pos = &ref_pos)
            {
                ImGuiNative.igLogRenderedText(native_ref_pos, native_text, native_text+text_byteCount);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void LogRenderedText(ref Vector2 ref_pos, string text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (Vector2* native_ref_pos = &ref_pos)
            {
                ImGuiNative.igLogRenderedText(native_ref_pos, native_text, native_text+text_byteCount);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void LogSetNextTextDecoration(ReadOnlySpan<char> prefix, ReadOnlySpan<char> suffix)
        {
            byte* native_prefix;
            int prefix_byteCount = 0;
            if (prefix != null)
            {
                prefix_byteCount = Encoding.UTF8.GetByteCount(prefix);
                if (prefix_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_prefix = Util.Allocate(prefix_byteCount + 1);
                }
                else
                {
                    byte* native_prefix_stackBytes = stackalloc byte[prefix_byteCount + 1];
                    native_prefix = native_prefix_stackBytes;
                }
                int native_prefix_offset = Util.GetUtf8(prefix, native_prefix, prefix_byteCount);
                native_prefix[native_prefix_offset] = 0;
            }
            else { native_prefix = null; }
            byte* native_suffix;
            int suffix_byteCount = 0;
            if (suffix != null)
            {
                suffix_byteCount = Encoding.UTF8.GetByteCount(suffix);
                if (suffix_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_suffix = Util.Allocate(suffix_byteCount + 1);
                }
                else
                {
                    byte* native_suffix_stackBytes = stackalloc byte[suffix_byteCount + 1];
                    native_suffix = native_suffix_stackBytes;
                }
                int native_suffix_offset = Util.GetUtf8(suffix, native_suffix, suffix_byteCount);
                native_suffix[native_suffix_offset] = 0;
            }
            else { native_suffix = null; }
            ImGuiNative.igLogSetNextTextDecoration(native_prefix, native_suffix);
            if (prefix_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_prefix);
            }
            if (suffix_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_suffix);
            }
        }
#endif
        public static void LogSetNextTextDecoration(string prefix, string suffix)
        {
            byte* native_prefix;
            int prefix_byteCount = 0;
            if (prefix != null)
            {
                prefix_byteCount = Encoding.UTF8.GetByteCount(prefix);
                if (prefix_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_prefix = Util.Allocate(prefix_byteCount + 1);
                }
                else
                {
                    byte* native_prefix_stackBytes = stackalloc byte[prefix_byteCount + 1];
                    native_prefix = native_prefix_stackBytes;
                }
                int native_prefix_offset = Util.GetUtf8(prefix, native_prefix, prefix_byteCount);
                native_prefix[native_prefix_offset] = 0;
            }
            else { native_prefix = null; }
            byte* native_suffix;
            int suffix_byteCount = 0;
            if (suffix != null)
            {
                suffix_byteCount = Encoding.UTF8.GetByteCount(suffix);
                if (suffix_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_suffix = Util.Allocate(suffix_byteCount + 1);
                }
                else
                {
                    byte* native_suffix_stackBytes = stackalloc byte[suffix_byteCount + 1];
                    native_suffix = native_suffix_stackBytes;
                }
                int native_suffix_offset = Util.GetUtf8(suffix, native_suffix, suffix_byteCount);
                native_suffix[native_suffix_offset] = 0;
            }
            else { native_suffix = null; }
            ImGuiNative.igLogSetNextTextDecoration(native_prefix, native_suffix);
            if (prefix_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_prefix);
            }
            if (suffix_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_suffix);
            }
        }
        public static void LogToBuffer()
        {
            int auto_open_depth = -1;
            ImGuiNative.igLogToBuffer(auto_open_depth);
        }
        public static void LogToBuffer(int auto_open_depth)
        {
            ImGuiNative.igLogToBuffer(auto_open_depth);
        }
        public static void MarkIniSettingsDirty()
        {
            ImGuiNative.igMarkIniSettingsDirty_Nil();
        }
        public static void MarkIniSettingsDirty(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igMarkIniSettingsDirty_WindowPtr(native_window);
        }
        public static void MarkItemEdited(uint id)
        {
            ImGuiNative.igMarkItemEdited(id);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool MenuItemEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut = null;
            byte selected = 0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
#endif
        public static bool MenuItemEx(string label, string icon)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut = null;
            byte selected = 0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool MenuItemEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon, ReadOnlySpan<char> shortcut)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte selected = 0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
#endif
        public static bool MenuItemEx(string label, string icon, string shortcut)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte selected = 0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool MenuItemEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon, ReadOnlySpan<char> shortcut, bool selected)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte native_selected = selected ? (byte)1 : (byte)0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, native_selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
#endif
        public static bool MenuItemEx(string label, string icon, string shortcut, bool selected)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte native_selected = selected ? (byte)1 : (byte)0;
            byte enabled = 1;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, native_selected, enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool MenuItemEx(ReadOnlySpan<char> label, ReadOnlySpan<char> icon, ReadOnlySpan<char> shortcut, bool selected, bool enabled)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte native_selected = selected ? (byte)1 : (byte)0;
            byte native_enabled = enabled ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, native_selected, native_enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
#endif
        public static bool MenuItemEx(string label, string icon, string shortcut, bool selected, bool enabled)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_icon;
            int icon_byteCount = 0;
            if (icon != null)
            {
                icon_byteCount = Encoding.UTF8.GetByteCount(icon);
                if (icon_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_icon = Util.Allocate(icon_byteCount + 1);
                }
                else
                {
                    byte* native_icon_stackBytes = stackalloc byte[icon_byteCount + 1];
                    native_icon = native_icon_stackBytes;
                }
                int native_icon_offset = Util.GetUtf8(icon, native_icon, icon_byteCount);
                native_icon[native_icon_offset] = 0;
            }
            else { native_icon = null; }
            byte* native_shortcut;
            int shortcut_byteCount = 0;
            if (shortcut != null)
            {
                shortcut_byteCount = Encoding.UTF8.GetByteCount(shortcut);
                if (shortcut_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_shortcut = Util.Allocate(shortcut_byteCount + 1);
                }
                else
                {
                    byte* native_shortcut_stackBytes = stackalloc byte[shortcut_byteCount + 1];
                    native_shortcut = native_shortcut_stackBytes;
                }
                int native_shortcut_offset = Util.GetUtf8(shortcut, native_shortcut, shortcut_byteCount);
                native_shortcut[native_shortcut_offset] = 0;
            }
            else { native_shortcut = null; }
            byte native_selected = selected ? (byte)1 : (byte)0;
            byte native_enabled = enabled ? (byte)1 : (byte)0;
            byte ret = ImGuiNative.igMenuItemEx(native_label, native_icon, native_shortcut, native_selected, native_enabled);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (icon_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_icon);
            }
            if (shortcut_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_shortcut);
            }
            return ret != 0;
        }
        public static ImGuiKey MouseButtonToKey(ImGuiMouseButton button)
        {
            ImGuiKey ret = ImGuiNative.igMouseButtonToKey(button);
            return ret;
        }
        public static void NavClearPreferredPosForAxis(ImGuiAxis axis)
        {
            ImGuiNative.igNavClearPreferredPosForAxis(axis);
        }
        public static void NavInitRequestApplyResult()
        {
            ImGuiNative.igNavInitRequestApplyResult();
        }
        public static void NavInitWindow(ImGuiWindowPtr window, bool force_reinit)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte native_force_reinit = force_reinit ? (byte)1 : (byte)0;
            ImGuiNative.igNavInitWindow(native_window, native_force_reinit);
        }
        public static void NavMoveRequestApplyResult()
        {
            ImGuiNative.igNavMoveRequestApplyResult();
        }
        public static bool NavMoveRequestButNoResultYet()
        {
            byte ret = ImGuiNative.igNavMoveRequestButNoResultYet();
            return ret != 0;
        }
        public static void NavMoveRequestCancel()
        {
            ImGuiNative.igNavMoveRequestCancel();
        }
        public static void NavMoveRequestForward(ImGuiDir move_dir, ImGuiDir clip_dir, ImGuiNavMoveFlags move_flags, ImGuiScrollFlags scroll_flags)
        {
            ImGuiNative.igNavMoveRequestForward(move_dir, clip_dir, move_flags, scroll_flags);
        }
        public static void NavMoveRequestResolveWithLastItem(ImGuiNavItemDataPtr result)
        {
            ImGuiNavItemData* native_result = result.NativePtr;
            ImGuiNative.igNavMoveRequestResolveWithLastItem(native_result);
        }
        public static void NavMoveRequestSubmit(ImGuiDir move_dir, ImGuiDir clip_dir, ImGuiNavMoveFlags move_flags, ImGuiScrollFlags scroll_flags)
        {
            ImGuiNative.igNavMoveRequestSubmit(move_dir, clip_dir, move_flags, scroll_flags);
        }
        public static void NavMoveRequestTryWrapping(ImGuiWindowPtr window, ImGuiNavMoveFlags move_flags)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igNavMoveRequestTryWrapping(native_window, move_flags);
        }
        public static void NavUpdateCurrentWindowIsScrollPushableX()
        {
            ImGuiNative.igNavUpdateCurrentWindowIsScrollPushableX();
        }
        public static void OpenPopupEx(uint id)
        {
            ImGuiPopupFlags popup_flags = ImGuiPopupFlags.None;
            ImGuiNative.igOpenPopupEx(id, popup_flags);
        }
        public static void OpenPopupEx(uint id, ImGuiPopupFlags popup_flags)
        {
            ImGuiNative.igOpenPopupEx(id, popup_flags);
        }
        public static void PopColumnsBackground()
        {
            ImGuiNative.igPopColumnsBackground();
        }
        public static void PopFocusScope()
        {
            ImGuiNative.igPopFocusScope();
        }
        public static void PopItemFlag()
        {
            ImGuiNative.igPopItemFlag();
        }
        public static void PushColumnClipRect(int column_index)
        {
            ImGuiNative.igPushColumnClipRect(column_index);
        }
        public static void PushColumnsBackground()
        {
            ImGuiNative.igPushColumnsBackground();
        }
        public static void PushFocusScope(uint id)
        {
            ImGuiNative.igPushFocusScope(id);
        }
        public static void PushItemFlag(ImGuiItemFlags option, bool enabled)
        {
            byte native_enabled = enabled ? (byte)1 : (byte)0;
            ImGuiNative.igPushItemFlag(option, native_enabled);
        }
        public static void PushMultiItemsWidths(int components, float width_full)
        {
            ImGuiNative.igPushMultiItemsWidths(components, width_full);
        }
        public static void PushOverrideID(uint id)
        {
            ImGuiNative.igPushOverrideID(id);
        }
        public static void RemoveContextHook(IntPtr context, uint hook_to_remove)
        {
            ImGuiNative.igRemoveContextHook(context, hook_to_remove);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RemoveSettingsHandler(ReadOnlySpan<char> type_name)
        {
            byte* native_type_name;
            int type_name_byteCount = 0;
            if (type_name != null)
            {
                type_name_byteCount = Encoding.UTF8.GetByteCount(type_name);
                if (type_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_type_name = Util.Allocate(type_name_byteCount + 1);
                }
                else
                {
                    byte* native_type_name_stackBytes = stackalloc byte[type_name_byteCount + 1];
                    native_type_name = native_type_name_stackBytes;
                }
                int native_type_name_offset = Util.GetUtf8(type_name, native_type_name, type_name_byteCount);
                native_type_name[native_type_name_offset] = 0;
            }
            else { native_type_name = null; }
            ImGuiNative.igRemoveSettingsHandler(native_type_name);
            if (type_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_type_name);
            }
        }
#endif
        public static void RemoveSettingsHandler(string type_name)
        {
            byte* native_type_name;
            int type_name_byteCount = 0;
            if (type_name != null)
            {
                type_name_byteCount = Encoding.UTF8.GetByteCount(type_name);
                if (type_name_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_type_name = Util.Allocate(type_name_byteCount + 1);
                }
                else
                {
                    byte* native_type_name_stackBytes = stackalloc byte[type_name_byteCount + 1];
                    native_type_name = native_type_name_stackBytes;
                }
                int native_type_name_offset = Util.GetUtf8(type_name, native_type_name, type_name_byteCount);
                native_type_name[native_type_name_offset] = 0;
            }
            else { native_type_name = null; }
            ImGuiNative.igRemoveSettingsHandler(native_type_name);
            if (type_name_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_type_name);
            }
        }
        public static void RenderArrow(ImDrawListPtr draw_list, Vector2 pos, uint col, ImGuiDir dir)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            float scale = 1.0f;
            ImGuiNative.igRenderArrow(native_draw_list, pos, col, dir, scale);
        }
        public static void RenderArrow(ImDrawListPtr draw_list, Vector2 pos, uint col, ImGuiDir dir, float scale)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderArrow(native_draw_list, pos, col, dir, scale);
        }
        public static void RenderArrowDockMenu(ImDrawListPtr draw_list, Vector2 p_min, float sz, uint col)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderArrowDockMenu(native_draw_list, p_min, sz, col);
        }
        public static void RenderArrowPointingAt(ImDrawListPtr draw_list, Vector2 pos, Vector2 half_sz, ImGuiDir direction, uint col)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderArrowPointingAt(native_draw_list, pos, half_sz, direction, col);
        }
        public static void RenderBullet(ImDrawListPtr draw_list, Vector2 pos, uint col)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderBullet(native_draw_list, pos, col);
        }
        public static void RenderCheckMark(ImDrawListPtr draw_list, Vector2 pos, uint col, float sz)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderCheckMark(native_draw_list, pos, col, sz);
        }
        public static void RenderColorRectWithAlphaCheckerboard(ImDrawListPtr draw_list, Vector2 p_min, Vector2 p_max, uint fill_col, float grid_step, Vector2 grid_off)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            float rounding = 0.0f;
            ImDrawFlags flags = (ImDrawFlags)0;
            ImGuiNative.igRenderColorRectWithAlphaCheckerboard(native_draw_list, p_min, p_max, fill_col, grid_step, grid_off, rounding, flags);
        }
        public static void RenderColorRectWithAlphaCheckerboard(ImDrawListPtr draw_list, Vector2 p_min, Vector2 p_max, uint fill_col, float grid_step, Vector2 grid_off, float rounding)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImDrawFlags flags = (ImDrawFlags)0;
            ImGuiNative.igRenderColorRectWithAlphaCheckerboard(native_draw_list, p_min, p_max, fill_col, grid_step, grid_off, rounding, flags);
        }
        public static void RenderColorRectWithAlphaCheckerboard(ImDrawListPtr draw_list, Vector2 p_min, Vector2 p_max, uint fill_col, float grid_step, Vector2 grid_off, float rounding, ImDrawFlags flags)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderColorRectWithAlphaCheckerboard(native_draw_list, p_min, p_max, fill_col, grid_step, grid_off, rounding, flags);
        }
        public static void RenderDragDropTargetRect(ImRect bb)
        {
            ImGuiNative.igRenderDragDropTargetRect(bb);
        }
        public static void RenderFrame(Vector2 p_min, Vector2 p_max, uint fill_col)
        {
            byte border = 1;
            float rounding = 0.0f;
            ImGuiNative.igRenderFrame(p_min, p_max, fill_col, border, rounding);
        }
        public static void RenderFrame(Vector2 p_min, Vector2 p_max, uint fill_col, bool border)
        {
            byte native_border = border ? (byte)1 : (byte)0;
            float rounding = 0.0f;
            ImGuiNative.igRenderFrame(p_min, p_max, fill_col, native_border, rounding);
        }
        public static void RenderFrame(Vector2 p_min, Vector2 p_max, uint fill_col, bool border, float rounding)
        {
            byte native_border = border ? (byte)1 : (byte)0;
            ImGuiNative.igRenderFrame(p_min, p_max, fill_col, native_border, rounding);
        }
        public static void RenderFrameBorder(Vector2 p_min, Vector2 p_max)
        {
            float rounding = 0.0f;
            ImGuiNative.igRenderFrameBorder(p_min, p_max, rounding);
        }
        public static void RenderFrameBorder(Vector2 p_min, Vector2 p_max, float rounding)
        {
            ImGuiNative.igRenderFrameBorder(p_min, p_max, rounding);
        }
        public static void RenderMouseCursor(Vector2 pos, float scale, ImGuiMouseCursor mouse_cursor, uint col_fill, uint col_border, uint col_shadow)
        {
            ImGuiNative.igRenderMouseCursor(pos, scale, mouse_cursor, col_fill, col_border, col_shadow);
        }
        public static void RenderNavHighlight(ImRect bb, uint id)
        {
            ImGuiNavHighlightFlags flags = ImGuiNavHighlightFlags.TypeDefault;
            ImGuiNative.igRenderNavHighlight(bb, id, flags);
        }
        public static void RenderNavHighlight(ImRect bb, uint id, ImGuiNavHighlightFlags flags)
        {
            ImGuiNative.igRenderNavHighlight(bb, id, flags);
        }
        public static void RenderRectFilledRangeH(ImDrawListPtr draw_list, ImRect rect, uint col, float x_start_norm, float x_end_norm, float rounding)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderRectFilledRangeH(native_draw_list, rect, col, x_start_norm, x_end_norm, rounding);
        }
        public static void RenderRectFilledWithHole(ImDrawListPtr draw_list, ImRect outer, ImRect inner, uint col, float rounding)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igRenderRectFilledWithHole(native_draw_list, outer, inner, col, rounding);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderText(Vector2 pos, ReadOnlySpan<char> text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte hide_text_after_hash = 1;
            ImGuiNative.igRenderText(pos, native_text, native_text+text_byteCount, hide_text_after_hash);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#endif
        public static void RenderText(Vector2 pos, string text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte hide_text_after_hash = 1;
            ImGuiNative.igRenderText(pos, native_text, native_text+text_byteCount, hide_text_after_hash);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderText(Vector2 pos, ReadOnlySpan<char> text, bool hide_text_after_hash)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte native_hide_text_after_hash = hide_text_after_hash ? (byte)1 : (byte)0;
            ImGuiNative.igRenderText(pos, native_text, native_text+text_byteCount, native_hide_text_after_hash);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#endif
        public static void RenderText(Vector2 pos, string text, bool hide_text_after_hash)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            byte native_hide_text_after_hash = hide_text_after_hash ? (byte)1 : (byte)0;
            ImGuiNative.igRenderText(pos, native_text, native_text+text_byteCount, native_hide_text_after_hash);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            Vector2 align = new Vector2();
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            Vector2 align = new Vector2();
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known, Vector2 align)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known, Vector2 align)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known, Vector2 align, ImRectPtr clip_rect)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* native_clip_rect = clip_rect.NativePtr;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, native_clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClipped(Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known, Vector2 align, ImRectPtr clip_rect)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* native_clip_rect = clip_rect.NativePtr;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClipped(pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, native_clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            Vector2 align = new Vector2();
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            Vector2 align = new Vector2();
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known, Vector2 align)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known, Vector2 align)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* clip_rect = null;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, ReadOnlySpan<char> text, ref Vector2 text_size_if_known, Vector2 align, ImRectPtr clip_rect)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* native_clip_rect = clip_rect.NativePtr;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, native_clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextClippedEx(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, string text, ref Vector2 text_size_if_known, Vector2 align, ImRectPtr clip_rect)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImRect* native_clip_rect = clip_rect.NativePtr;
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextClippedEx(native_draw_list, pos_min, pos_max, native_text, native_text+text_byteCount, native_text_size_if_known, align, native_clip_rect);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextEllipsis(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, float clip_max_x, float ellipsis_max_x, ReadOnlySpan<char> text, ref Vector2 text_size_if_known)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextEllipsis(native_draw_list, pos_min, pos_max, clip_max_x, ellipsis_max_x, native_text, native_text+text_byteCount, native_text_size_if_known);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#endif
        public static void RenderTextEllipsis(ImDrawListPtr draw_list, Vector2 pos_min, Vector2 pos_max, float clip_max_x, float ellipsis_max_x, string text, ref Vector2 text_size_if_known)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            fixed (Vector2* native_text_size_if_known = &text_size_if_known)
            {
                ImGuiNative.igRenderTextEllipsis(native_draw_list, pos_min, pos_max, clip_max_x, ellipsis_max_x, native_text, native_text+text_byteCount, native_text_size_if_known);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    Util.Free(native_text);
                }
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void RenderTextWrapped(Vector2 pos, ReadOnlySpan<char> text, float wrap_width)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiNative.igRenderTextWrapped(pos, native_text, native_text+text_byteCount, wrap_width);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#endif
        public static void RenderTextWrapped(Vector2 pos, string text, float wrap_width)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiNative.igRenderTextWrapped(pos, native_text, native_text+text_byteCount, wrap_width);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
        public static void ScaleWindowsInViewport(ImGuiViewportPPtr viewport, float scale)
        {
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igScaleWindowsInViewport(native_viewport, scale);
        }
        public static void Scrollbar(ImGuiAxis axis)
        {
            ImGuiNative.igScrollbar(axis);
        }
        public static bool ScrollbarEx(ImRect bb, uint id, ImGuiAxis axis, ref long p_scroll_v, long avail_v, long contents_v, ImDrawFlags flags)
        {
            fixed (long* native_p_scroll_v = &p_scroll_v)
            {
                byte ret = ImGuiNative.igScrollbarEx(bb, id, axis, native_p_scroll_v, avail_v, contents_v, flags);
                return ret != 0;
            }
        }
        public static void ScrollToBringRectIntoView(ImGuiWindowPtr window, ImRect rect)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igScrollToBringRectIntoView(native_window, rect);
        }
        public static void ScrollToItem()
        {
            ImGuiScrollFlags flags = (ImGuiScrollFlags)0;
            ImGuiNative.igScrollToItem(flags);
        }
        public static void ScrollToItem(ImGuiScrollFlags flags)
        {
            ImGuiNative.igScrollToItem(flags);
        }
        public static void ScrollToRect(ImGuiWindowPtr window, ImRect rect)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiScrollFlags flags = (ImGuiScrollFlags)0;
            ImGuiNative.igScrollToRect(native_window, rect, flags);
        }
        public static void ScrollToRect(ImGuiWindowPtr window, ImRect rect, ImGuiScrollFlags flags)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igScrollToRect(native_window, rect, flags);
        }
        public static Vector2 ScrollToRectEx(ImGuiWindowPtr window, ImRect rect)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiScrollFlags flags = (ImGuiScrollFlags)0;
            ImGuiNative.igScrollToRectEx(&__retval, native_window, rect, flags);
            return __retval;
        }
        public static Vector2 ScrollToRectEx(ImGuiWindowPtr window, ImRect rect, ImGuiScrollFlags flags)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igScrollToRectEx(&__retval, native_window, rect, flags);
            return __retval;
        }
        public static void SeparatorEx(ImGuiSeparatorFlags flags)
        {
            float thickness = 1.0f;
            ImGuiNative.igSeparatorEx(flags, thickness);
        }
        public static void SeparatorEx(ImGuiSeparatorFlags flags, float thickness)
        {
            ImGuiNative.igSeparatorEx(flags, thickness);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void SeparatorTextEx(uint id, ReadOnlySpan<char> label, float extra_width)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igSeparatorTextEx(id, native_label, native_label+label_byteCount, extra_width);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
#endif
        public static void SeparatorTextEx(uint id, string label, float extra_width)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            ImGuiNative.igSeparatorTextEx(id, native_label, native_label+label_byteCount, extra_width);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
        }
        public static void SetActiveID(uint id, ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetActiveID(id, native_window);
        }
        public static void SetActiveIdUsingAllKeyboardKeys()
        {
            ImGuiNative.igSetActiveIdUsingAllKeyboardKeys();
        }
        public static void SetCurrentFont(ImFontPtr font)
        {
            ImFont* native_font = font.NativePtr;
            ImGuiNative.igSetCurrentFont(native_font);
        }
        public static void SetCurrentViewport(ImGuiWindowPtr window, ImGuiViewportPPtr viewport)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igSetCurrentViewport(native_window, native_viewport);
        }
        public static void SetFocusID(uint id, ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetFocusID(id, native_window);
        }
        public static void SetHoveredID(uint id)
        {
            ImGuiNative.igSetHoveredID(id);
        }
        public static void SetItemKeyOwner(ImGuiKey key)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            ImGuiNative.igSetItemKeyOwner(key, flags);
        }
        public static void SetItemKeyOwner(ImGuiKey key, ImGuiInputFlags flags)
        {
            ImGuiNative.igSetItemKeyOwner(key, flags);
        }
        public static void SetKeyOwner(ImGuiKey key, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            ImGuiNative.igSetKeyOwner(key, owner_id, flags);
        }
        public static void SetKeyOwner(ImGuiKey key, uint owner_id, ImGuiInputFlags flags)
        {
            ImGuiNative.igSetKeyOwner(key, owner_id, flags);
        }
        public static void SetKeyOwnersForKeyChord(ImGuiKey key, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            ImGuiNative.igSetKeyOwnersForKeyChord(key, owner_id, flags);
        }
        public static void SetKeyOwnersForKeyChord(ImGuiKey key, uint owner_id, ImGuiInputFlags flags)
        {
            ImGuiNative.igSetKeyOwnersForKeyChord(key, owner_id, flags);
        }
        public static void SetLastItemData(uint item_id, ImGuiItemFlags in_flags, ImGuiItemStatusFlags status_flags, ImRect item_rect)
        {
            ImGuiNative.igSetLastItemData(item_id, in_flags, status_flags, item_rect);
        }
        public static void SetNavID(uint id, ImGuiNavLayer nav_layer, uint focus_scope_id, ImRect rect_rel)
        {
            ImGuiNative.igSetNavID(id, nav_layer, focus_scope_id, rect_rel);
        }
        public static void SetNavWindow(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetNavWindow(native_window);
        }
        public static void SetScrollFromPosX(ImGuiWindowPtr window, float local_x, float center_x_ratio)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetScrollFromPosX_WindowPtr(native_window, local_x, center_x_ratio);
        }
        public static void SetScrollFromPosY(ImGuiWindowPtr window, float local_y, float center_y_ratio)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetScrollFromPosY_WindowPtr(native_window, local_y, center_y_ratio);
        }
        public static void SetScrollX(ImGuiWindowPtr window, float scroll_x)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetScrollX_WindowPtr(native_window, scroll_x);
        }
        public static void SetScrollY(ImGuiWindowPtr window, float scroll_y)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetScrollY_WindowPtr(native_window, scroll_y);
        }
        public static bool SetShortcutRouting(ImGuiKey key_chord)
        {
            uint owner_id = 0;
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igSetShortcutRouting(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static bool SetShortcutRouting(ImGuiKey key_chord, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igSetShortcutRouting(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static bool SetShortcutRouting(ImGuiKey key_chord, uint owner_id, ImGuiInputFlags flags)
        {
            byte ret = ImGuiNative.igSetShortcutRouting(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static void SetWindowClipRectBeforeSetChannel(ImGuiWindowPtr window, ImRect clip_rect)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowClipRectBeforeSetChannel(native_window, clip_rect);
        }
        public static void SetWindowCollapsed(ImGuiWindowPtr window, bool collapsed)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte native_collapsed = collapsed ? (byte)1 : (byte)0;
            ImGuiCond cond = (ImGuiCond)0;
            ImGuiNative.igSetWindowCollapsed_WindowPtr(native_window, native_collapsed, cond);
        }
        public static void SetWindowCollapsed(ImGuiWindowPtr window, bool collapsed, ImGuiCond cond)
        {
            ImGuiWindow* native_window = window.NativePtr;
            byte native_collapsed = collapsed ? (byte)1 : (byte)0;
            ImGuiNative.igSetWindowCollapsed_WindowPtr(native_window, native_collapsed, cond);
        }
        public static void SetWindowDock(ImGuiWindowPtr window, uint dock_id, ImGuiCond cond)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowDock(native_window, dock_id, cond);
        }
        public static void SetWindowHiddendAndSkipItemsForCurrentFrame(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowHiddendAndSkipItemsForCurrentFrame(native_window);
        }
        public static void SetWindowHitTestHole(ImGuiWindowPtr window, Vector2 pos, Vector2 size)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowHitTestHole(native_window, pos, size);
        }
        public static void SetWindowPos(ImGuiWindowPtr window, Vector2 pos)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiCond cond = (ImGuiCond)0;
            ImGuiNative.igSetWindowPos_WindowPtr(native_window, pos, cond);
        }
        public static void SetWindowPos(ImGuiWindowPtr window, Vector2 pos, ImGuiCond cond)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowPos_WindowPtr(native_window, pos, cond);
        }
        public static void SetWindowSize(ImGuiWindowPtr window, Vector2 size)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiCond cond = (ImGuiCond)0;
            ImGuiNative.igSetWindowSize_WindowPtr(native_window, size, cond);
        }
        public static void SetWindowSize(ImGuiWindowPtr window, Vector2 size, ImGuiCond cond)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igSetWindowSize_WindowPtr(native_window, size, cond);
        }
        public static void SetWindowViewport(ImGuiWindowPtr window, ImGuiViewportPPtr viewport)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igSetWindowViewport(native_window, native_viewport);
        }
        public static void ShadeVertsLinearColorGradientKeepAlpha(ImDrawListPtr draw_list, int vert_start_idx, int vert_end_idx, Vector2 gradient_p0, Vector2 gradient_p1, uint col0, uint col1)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igShadeVertsLinearColorGradientKeepAlpha(native_draw_list, vert_start_idx, vert_end_idx, gradient_p0, gradient_p1, col0, col1);
        }
        public static void ShadeVertsLinearUV(ImDrawListPtr draw_list, int vert_start_idx, int vert_end_idx, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, bool clamp)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte native_clamp = clamp ? (byte)1 : (byte)0;
            ImGuiNative.igShadeVertsLinearUV(native_draw_list, vert_start_idx, vert_end_idx, a, b, uv_a, uv_b, native_clamp);
        }
        public static bool Shortcut(ImGuiKey key_chord)
        {
            uint owner_id = 0;
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igShortcut(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static bool Shortcut(ImGuiKey key_chord, uint owner_id)
        {
            ImGuiInputFlags flags = (ImGuiInputFlags)0;
            byte ret = ImGuiNative.igShortcut(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static bool Shortcut(ImGuiKey key_chord, uint owner_id, ImGuiInputFlags flags)
        {
            byte ret = ImGuiNative.igShortcut(key_chord, owner_id, flags);
            return ret != 0;
        }
        public static void ShowFontAtlas(ImFontAtlasPtr atlas)
        {
            ImFontAtlas* native_atlas = atlas.NativePtr;
            ImGuiNative.igShowFontAtlas(native_atlas);
        }
        public static void ShrinkWidths(ImGuiShrinkWidthItemPtr items, int count, float width_excess)
        {
            ImGuiShrinkWidthItem* native_items = items.NativePtr;
            ImGuiNative.igShrinkWidths(native_items, count, width_excess);
        }
        public static void Shutdown()
        {
            ImGuiNative.igShutdown();
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool SliderBehavior(ImRect bb, uint id, ImGuiDataType data_type, IntPtr p_v, IntPtr p_min, IntPtr p_max, ReadOnlySpan<char> format, ImGuiSliderFlags flags, ImRectPtr out_grab_bb)
        {
            void* native_p_v = (void*)p_v.ToPointer();
            void* native_p_min = (void*)p_min.ToPointer();
            void* native_p_max = (void*)p_max.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            ImRect* native_out_grab_bb = out_grab_bb.NativePtr;
            byte ret = ImGuiNative.igSliderBehavior(bb, id, data_type, native_p_v, native_p_min, native_p_max, native_format, flags, native_out_grab_bb);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool SliderBehavior(ImRect bb, uint id, ImGuiDataType data_type, IntPtr p_v, IntPtr p_min, IntPtr p_max, string format, ImGuiSliderFlags flags, ImRectPtr out_grab_bb)
        {
            void* native_p_v = (void*)p_v.ToPointer();
            void* native_p_min = (void*)p_min.ToPointer();
            void* native_p_max = (void*)p_max.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            ImRect* native_out_grab_bb = out_grab_bb.NativePtr;
            byte ret = ImGuiNative.igSliderBehavior(bb, id, data_type, native_p_v, native_p_min, native_p_max, native_format, flags, native_out_grab_bb);
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
        public static bool SplitterBehavior(ImRect bb, uint id, ImGuiAxis axis, ref float size1, ref float size2, float min_size1, float min_size2)
        {
            float hover_extend = 0.0f;
            float hover_visibility_delay = 0.0f;
            uint bg_col = 0;
            fixed (float* native_size1 = &size1)
            {
                fixed (float* native_size2 = &size2)
                {
                    byte ret = ImGuiNative.igSplitterBehavior(bb, id, axis, native_size1, native_size2, min_size1, min_size2, hover_extend, hover_visibility_delay, bg_col);
                    return ret != 0;
                }
            }
        }
        public static bool SplitterBehavior(ImRect bb, uint id, ImGuiAxis axis, ref float size1, ref float size2, float min_size1, float min_size2, float hover_extend)
        {
            float hover_visibility_delay = 0.0f;
            uint bg_col = 0;
            fixed (float* native_size1 = &size1)
            {
                fixed (float* native_size2 = &size2)
                {
                    byte ret = ImGuiNative.igSplitterBehavior(bb, id, axis, native_size1, native_size2, min_size1, min_size2, hover_extend, hover_visibility_delay, bg_col);
                    return ret != 0;
                }
            }
        }
        public static bool SplitterBehavior(ImRect bb, uint id, ImGuiAxis axis, ref float size1, ref float size2, float min_size1, float min_size2, float hover_extend, float hover_visibility_delay)
        {
            uint bg_col = 0;
            fixed (float* native_size1 = &size1)
            {
                fixed (float* native_size2 = &size2)
                {
                    byte ret = ImGuiNative.igSplitterBehavior(bb, id, axis, native_size1, native_size2, min_size1, min_size2, hover_extend, hover_visibility_delay, bg_col);
                    return ret != 0;
                }
            }
        }
        public static bool SplitterBehavior(ImRect bb, uint id, ImGuiAxis axis, ref float size1, ref float size2, float min_size1, float min_size2, float hover_extend, float hover_visibility_delay, uint bg_col)
        {
            fixed (float* native_size1 = &size1)
            {
                fixed (float* native_size2 = &size2)
                {
                    byte ret = ImGuiNative.igSplitterBehavior(bb, id, axis, native_size1, native_size2, min_size1, min_size2, hover_extend, hover_visibility_delay, bg_col);
                    return ret != 0;
                }
            }
        }
        public static void StartMouseMovingWindow(ImGuiWindowPtr window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igStartMouseMovingWindow(native_window);
        }
        public static void StartMouseMovingWindowOrNode(ImGuiWindowPtr window, ImGuiDockNodePtr node, bool undock_floating_node)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiDockNode* native_node = node.NativePtr;
            byte native_undock_floating_node = undock_floating_node ? (byte)1 : (byte)0;
            ImGuiNative.igStartMouseMovingWindowOrNode(native_window, native_node, native_undock_floating_node);
        }
        public static void TabBarAddTab(ImGuiTabBarPtr tab_bar, ImGuiTabItemFlags tab_flags, ImGuiWindowPtr window)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igTabBarAddTab(native_tab_bar, tab_flags, native_window);
        }
        public static void TabBarCloseTab(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            ImGuiNative.igTabBarCloseTab(native_tab_bar, native_tab);
        }
        public static ImGuiTabItemPtr TabBarFindMostRecentlySelectedTabForActiveWindow(ImGuiTabBarPtr tab_bar)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* ret = ImGuiNative.igTabBarFindMostRecentlySelectedTabForActiveWindow(native_tab_bar);
            return new ImGuiTabItemPtr(ret);
        }
        public static ImGuiTabItemPtr TabBarFindTabByID(ImGuiTabBarPtr tab_bar, uint tab_id)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* ret = ImGuiNative.igTabBarFindTabByID(native_tab_bar, tab_id);
            return new ImGuiTabItemPtr(ret);
        }
        public static ImGuiTabItemPtr TabBarFindTabByOrder(ImGuiTabBarPtr tab_bar, int order)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* ret = ImGuiNative.igTabBarFindTabByOrder(native_tab_bar, order);
            return new ImGuiTabItemPtr(ret);
        }
        public static ImGuiTabItemPtr TabBarGetCurrentTab(ImGuiTabBarPtr tab_bar)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* ret = ImGuiNative.igTabBarGetCurrentTab(native_tab_bar);
            return new ImGuiTabItemPtr(ret);
        }
        public static string TabBarGetTabName(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            byte* ret = ImGuiNative.igTabBarGetTabName(native_tab_bar, native_tab);
            return Util.StringFromPtr(ret);
        }
        public static int TabBarGetTabOrder(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            int ret = ImGuiNative.igTabBarGetTabOrder(native_tab_bar, native_tab);
            return ret;
        }
        public static bool TabBarProcessReorder(ImGuiTabBarPtr tab_bar)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            byte ret = ImGuiNative.igTabBarProcessReorder(native_tab_bar);
            return ret != 0;
        }
        public static void TabBarQueueFocus(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            ImGuiNative.igTabBarQueueFocus(native_tab_bar, native_tab);
        }
        public static void TabBarQueueReorder(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab, int offset)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            ImGuiNative.igTabBarQueueReorder(native_tab_bar, native_tab, offset);
        }
        public static void TabBarQueueReorderFromMousePos(ImGuiTabBarPtr tab_bar, ImGuiTabItemPtr tab, Vector2 mouse_pos)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiTabItem* native_tab = tab.NativePtr;
            ImGuiNative.igTabBarQueueReorderFromMousePos(native_tab_bar, native_tab, mouse_pos);
        }
        public static void TabBarRemoveTab(ImGuiTabBarPtr tab_bar, uint tab_id)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            ImGuiNative.igTabBarRemoveTab(native_tab_bar, tab_id);
        }
        public static void TabItemBackground(ImDrawListPtr draw_list, ImRect bb, ImGuiTabItemFlags flags, uint col)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            ImGuiNative.igTabItemBackground(native_draw_list, bb, flags, col);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static Vector2 TabItemCalcSize(ReadOnlySpan<char> label, bool has_close_button_or_unsaved_marker)
        {
            Vector2 __retval;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_has_close_button_or_unsaved_marker = has_close_button_or_unsaved_marker ? (byte)1 : (byte)0;
            ImGuiNative.igTabItemCalcSize_Str(&__retval, native_label, native_has_close_button_or_unsaved_marker);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return __retval;
        }
#endif
        public static Vector2 TabItemCalcSize(string label, bool has_close_button_or_unsaved_marker)
        {
            Vector2 __retval;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_has_close_button_or_unsaved_marker = has_close_button_or_unsaved_marker ? (byte)1 : (byte)0;
            ImGuiNative.igTabItemCalcSize_Str(&__retval, native_label, native_has_close_button_or_unsaved_marker);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return __retval;
        }
        public static Vector2 TabItemCalcSize(ImGuiWindowPtr window)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igTabItemCalcSize_WindowPtr(&__retval, native_window);
            return __retval;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TabItemEx(ImGuiTabBarPtr tab_bar, ReadOnlySpan<char> label, ref bool p_open, ImGuiTabItemFlags flags, ImGuiWindowPtr docked_window)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_p_open_val = p_open ? (byte)1 : (byte)0;
            byte* native_p_open = &native_p_open_val;
            ImGuiWindow* native_docked_window = docked_window.NativePtr;
            byte ret = ImGuiNative.igTabItemEx(native_tab_bar, native_label, native_p_open, flags, native_docked_window);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            p_open = native_p_open_val != 0;
            return ret != 0;
        }
#endif
        public static bool TabItemEx(ImGuiTabBarPtr tab_bar, string label, ref bool p_open, ImGuiTabItemFlags flags, ImGuiWindowPtr docked_window)
        {
            ImGuiTabBar* native_tab_bar = tab_bar.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_p_open_val = p_open ? (byte)1 : (byte)0;
            byte* native_p_open = &native_p_open_val;
            ImGuiWindow* native_docked_window = docked_window.NativePtr;
            byte ret = ImGuiNative.igTabItemEx(native_tab_bar, native_label, native_p_open, flags, native_docked_window);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            p_open = native_p_open_val != 0;
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void TabItemLabelAndCloseButton(ImDrawListPtr draw_list, ImRect bb, ImGuiTabItemFlags flags, Vector2 frame_padding, ReadOnlySpan<char> label, uint tab_id, uint close_button_id, bool is_contents_visible, ref bool out_just_closed, ref bool out_text_clipped)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_is_contents_visible = is_contents_visible ? (byte)1 : (byte)0;
            byte native_out_just_closed_val = out_just_closed ? (byte)1 : (byte)0;
            byte* native_out_just_closed = &native_out_just_closed_val;
            byte native_out_text_clipped_val = out_text_clipped ? (byte)1 : (byte)0;
            byte* native_out_text_clipped = &native_out_text_clipped_val;
            ImGuiNative.igTabItemLabelAndCloseButton(native_draw_list, bb, flags, frame_padding, native_label, tab_id, close_button_id, native_is_contents_visible, native_out_just_closed, native_out_text_clipped);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            out_just_closed = native_out_just_closed_val != 0;
            out_text_clipped = native_out_text_clipped_val != 0;
        }
#endif
        public static void TabItemLabelAndCloseButton(ImDrawListPtr draw_list, ImRect bb, ImGuiTabItemFlags flags, Vector2 frame_padding, string label, uint tab_id, uint close_button_id, bool is_contents_visible, ref bool out_just_closed, ref bool out_text_clipped)
        {
            ImDrawList* native_draw_list = draw_list.NativePtr;
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte native_is_contents_visible = is_contents_visible ? (byte)1 : (byte)0;
            byte native_out_just_closed_val = out_just_closed ? (byte)1 : (byte)0;
            byte* native_out_just_closed = &native_out_just_closed_val;
            byte native_out_text_clipped_val = out_text_clipped ? (byte)1 : (byte)0;
            byte* native_out_text_clipped = &native_out_text_clipped_val;
            ImGuiNative.igTabItemLabelAndCloseButton(native_draw_list, bb, flags, frame_padding, native_label, tab_id, close_button_id, native_is_contents_visible, native_out_just_closed, native_out_text_clipped);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            out_just_closed = native_out_just_closed_val != 0;
            out_text_clipped = native_out_text_clipped_val != 0;
        }
        
        public static ImGuiSortDirection TableGetColumnNextSortDirection(ImGuiTableColumnPtr column)
        {
            ImGuiTableColumn* native_column = column.NativePtr;
            ImGuiSortDirection ret = ImGuiNative.igTableGetColumnNextSortDirection(native_column);
            return ret;
        }
        
        public static float TableGetHeaderRowHeight()
        {
            float ret = ImGuiNative.igTableGetHeaderRowHeight();
            return ret;
        }
        public static int TableGetHoveredColumn()
        {
            int ret = ImGuiNative.igTableGetHoveredColumn();
            return ret;
        }
        
        public static void TableOpenContextMenu()
        {
            int column_n = -1;
            ImGuiNative.igTableOpenContextMenu(column_n);
        }
        public static void TableOpenContextMenu(int column_n)
        {
            ImGuiNative.igTableOpenContextMenu(column_n);
        }
        public static void TablePopBackgroundChannel()
        {
            ImGuiNative.igTablePopBackgroundChannel();
        }
        public static void TablePushBackgroundChannel()
        {
            ImGuiNative.igTablePushBackgroundChannel();
        }
        
        public static void TableSetColumnSortDirection(int column_n, ImGuiSortDirection sort_direction, bool append_to_sort_specs)
        {
            byte native_append_to_sort_specs = append_to_sort_specs ? (byte)1 : (byte)0;
            ImGuiNative.igTableSetColumnSortDirection(column_n, sort_direction, native_append_to_sort_specs);
        }
        public static void TableSetColumnWidth(int column_n, float width)
        {
            ImGuiNative.igTableSetColumnWidth(column_n, width);
        }
        public static void TableSettingsAddSettingsHandler()
        {
            ImGuiNative.igTableSettingsAddSettingsHandler();
        }
        public static ImGuiTableSettingsPtr TableSettingsCreate(uint id, int columns_count)
        {
            ImGuiTableSettings* ret = ImGuiNative.igTableSettingsCreate(id, columns_count);
            return new ImGuiTableSettingsPtr(ret);
        }
        public static ImGuiTableSettingsPtr TableSettingsFindByID(uint id)
        {
            ImGuiTableSettings* ret = ImGuiNative.igTableSettingsFindByID(id);
            return new ImGuiTableSettingsPtr(ret);
        }
        public static bool TempInputIsActive(uint id)
        {
            byte ret = ImGuiNative.igTempInputIsActive(id);
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TempInputScalar(ImRect bb, uint id, ReadOnlySpan<char> label, ImGuiDataType data_type, IntPtr p_data, ReadOnlySpan<char> format)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* p_clamp_min = null;
            void* p_clamp_max = null;
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, p_clamp_min, p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool TempInputScalar(ImRect bb, uint id, string label, ImGuiDataType data_type, IntPtr p_data, string format)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* p_clamp_min = null;
            void* p_clamp_max = null;
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, p_clamp_min, p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TempInputScalar(ImRect bb, uint id, ReadOnlySpan<char> label, ImGuiDataType data_type, IntPtr p_data, ReadOnlySpan<char> format, IntPtr p_clamp_min)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* native_p_clamp_min = (void*)p_clamp_min.ToPointer();
            void* p_clamp_max = null;
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, native_p_clamp_min, p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool TempInputScalar(ImRect bb, uint id, string label, ImGuiDataType data_type, IntPtr p_data, string format, IntPtr p_clamp_min)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* native_p_clamp_min = (void*)p_clamp_min.ToPointer();
            void* p_clamp_max = null;
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, native_p_clamp_min, p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TempInputScalar(ImRect bb, uint id, ReadOnlySpan<char> label, ImGuiDataType data_type, IntPtr p_data, ReadOnlySpan<char> format, IntPtr p_clamp_min, IntPtr p_clamp_max)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* native_p_clamp_min = (void*)p_clamp_min.ToPointer();
            void* native_p_clamp_max = (void*)p_clamp_max.ToPointer();
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, native_p_clamp_min, native_p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#endif
        public static bool TempInputScalar(ImRect bb, uint id, string label, ImGuiDataType data_type, IntPtr p_data, string format, IntPtr p_clamp_min, IntPtr p_clamp_max)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            void* native_p_data = (void*)p_data.ToPointer();
            byte* native_format;
            int format_byteCount = 0;
            if (format != null)
            {
                format_byteCount = Encoding.UTF8.GetByteCount(format);
                if (format_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_format = Util.Allocate(format_byteCount + 1);
                }
                else
                {
                    byte* native_format_stackBytes = stackalloc byte[format_byteCount + 1];
                    native_format = native_format_stackBytes;
                }
                int native_format_offset = Util.GetUtf8(format, native_format, format_byteCount);
                native_format[native_format_offset] = 0;
            }
            else { native_format = null; }
            void* native_p_clamp_min = (void*)p_clamp_min.ToPointer();
            void* native_p_clamp_max = (void*)p_clamp_max.ToPointer();
            byte ret = ImGuiNative.igTempInputScalar(bb, id, native_label, data_type, native_p_data, native_format, native_p_clamp_min, native_p_clamp_max);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (format_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_format);
            }
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TempInputText(ImRect bb, uint id, ReadOnlySpan<char> label, ReadOnlySpan<char> buf, int buf_size, ImGuiInputTextFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte ret = ImGuiNative.igTempInputText(bb, id, native_label, native_buf, buf_size, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
#endif
        public static bool TempInputText(ImRect bb, uint id, string label, string buf, int buf_size, ImGuiInputTextFlags flags)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte* native_buf;
            int buf_byteCount = 0;
            if (buf != null)
            {
                buf_byteCount = Encoding.UTF8.GetByteCount(buf);
                if (buf_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_buf = Util.Allocate(buf_byteCount + 1);
                }
                else
                {
                    byte* native_buf_stackBytes = stackalloc byte[buf_byteCount + 1];
                    native_buf = native_buf_stackBytes;
                }
                int native_buf_offset = Util.GetUtf8(buf, native_buf, buf_byteCount);
                native_buf[native_buf_offset] = 0;
            }
            else { native_buf = null; }
            byte ret = ImGuiNative.igTempInputText(bb, id, native_label, native_buf, buf_size, flags);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            if (buf_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_buf);
            }
            return ret != 0;
        }
        public static bool TestKeyOwner(ImGuiKey key, uint owner_id)
        {
            byte ret = ImGuiNative.igTestKeyOwner(key, owner_id);
            return ret != 0;
        }
        public static bool TestShortcutRouting(ImGuiKey key_chord, uint owner_id)
        {
            byte ret = ImGuiNative.igTestShortcutRouting(key_chord, owner_id);
            return ret != 0;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void TextEx(ReadOnlySpan<char> text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiTextFlags flags = (ImGuiTextFlags)0;
            ImGuiNative.igTextEx(native_text, native_text+text_byteCount, flags);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#endif
        public static void TextEx(string text)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiTextFlags flags = (ImGuiTextFlags)0;
            ImGuiNative.igTextEx(native_text, native_text+text_byteCount, flags);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static void TextEx(ReadOnlySpan<char> text, ImGuiTextFlags flags)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiNative.igTextEx(native_text, native_text+text_byteCount, flags);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
#endif
        public static void TextEx(string text, ImGuiTextFlags flags)
        {
            byte* native_text;
            int text_byteCount = 0;
            if (text != null)
            {
                text_byteCount = Encoding.UTF8.GetByteCount(text);
                if (text_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_text = Util.Allocate(text_byteCount + 1);
                }
                else
                {
                    byte* native_text_stackBytes = stackalloc byte[text_byteCount + 1];
                    native_text = native_text_stackBytes;
                }
                int native_text_offset = Util.GetUtf8(text, native_text, text_byteCount);
                native_text[native_text_offset] = 0;
            }
            else { native_text = null; }
            ImGuiNative.igTextEx(native_text, native_text+text_byteCount, flags);
            if (text_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_text);
            }
        }
        public static void TranslateWindowsInViewport(ImGuiViewportPPtr viewport, Vector2 old_pos, Vector2 new_pos)
        {
            ImGuiViewportP* native_viewport = viewport.NativePtr;
            ImGuiNative.igTranslateWindowsInViewport(native_viewport, old_pos, new_pos);
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        public static bool TreeNodeBehavior(uint id, ImGuiTreeNodeFlags flags, ReadOnlySpan<char> label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte ret = ImGuiNative.igTreeNodeBehavior(id, flags, native_label, native_label+label_byteCount);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
#endif
        public static bool TreeNodeBehavior(uint id, ImGuiTreeNodeFlags flags, string label)
        {
            byte* native_label;
            int label_byteCount = 0;
            if (label != null)
            {
                label_byteCount = Encoding.UTF8.GetByteCount(label);
                if (label_byteCount > Util.StackAllocationSizeLimit)
                {
                    native_label = Util.Allocate(label_byteCount + 1);
                }
                else
                {
                    byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                    native_label = native_label_stackBytes;
                }
                int native_label_offset = Util.GetUtf8(label, native_label, label_byteCount);
                native_label[native_label_offset] = 0;
            }
            else { native_label = null; }
            byte ret = ImGuiNative.igTreeNodeBehavior(id, flags, native_label, native_label+label_byteCount);
            if (label_byteCount > Util.StackAllocationSizeLimit)
            {
                Util.Free(native_label);
            }
            return ret != 0;
        }
        public static void TreeNodeSetOpen(uint id, bool open)
        {
            byte native_open = open ? (byte)1 : (byte)0;
            ImGuiNative.igTreeNodeSetOpen(id, native_open);
        }
        public static bool TreeNodeUpdateNextOpen(uint id, ImGuiTreeNodeFlags flags)
        {
            byte ret = ImGuiNative.igTreeNodeUpdateNextOpen(id, flags);
            return ret != 0;
        }
        public static void TreePushOverrideID(uint id)
        {
            ImGuiNative.igTreePushOverrideID(id);
        }
        public static void UpdateHoveredWindowAndCaptureFlags()
        {
            ImGuiNative.igUpdateHoveredWindowAndCaptureFlags();
        }
        public static void UpdateInputEvents(bool trickle_fast_inputs)
        {
            byte native_trickle_fast_inputs = trickle_fast_inputs ? (byte)1 : (byte)0;
            ImGuiNative.igUpdateInputEvents(native_trickle_fast_inputs);
        }
        public static void UpdateMouseMovingWindowEndFrame()
        {
            ImGuiNative.igUpdateMouseMovingWindowEndFrame();
        }
        public static void UpdateMouseMovingWindowNewFrame()
        {
            ImGuiNative.igUpdateMouseMovingWindowNewFrame();
        }
        public static void UpdateWindowParentAndRootLinks(ImGuiWindowPtr window, ImGuiWindowFlags flags, ImGuiWindowPtr parent_window)
        {
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiWindow* native_parent_window = parent_window.NativePtr;
            ImGuiNative.igUpdateWindowParentAndRootLinks(native_window, flags, native_parent_window);
        }
        public static Vector2 WindowPosRelToAbs(ImGuiWindowPtr window, Vector2 p)
        {
            Vector2 __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igWindowPosRelToAbs(&__retval, native_window, p);
            return __retval;
        }
        public static ImRect WindowRectAbsToRel(ImGuiWindowPtr window, ImRect r)
        {
            ImRect __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igWindowRectAbsToRel(&__retval, native_window, r);
            return __retval;
        }
        public static ImRect WindowRectRelToAbs(ImGuiWindowPtr window, ImRect r)
        {
            ImRect __retval;
            ImGuiWindow* native_window = window.NativePtr;
            ImGuiNative.igWindowRectRelToAbs(&__retval, native_window, r);
            return __retval;
        }
    }
}
