using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET.Internal;

namespace ImGuiNET
{
    public static unsafe partial class ImGuiNative
    {
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igActivateItemByID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igAddContextHook(IntPtr context, ImGuiContextHook* hook);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igAddSettingsHandler(ImGuiSettingsHandler* handler);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igArrowButtonEx(byte* str_id, ImGuiDir dir, Vector2 size_arg, ImGuiButtonFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginChildEx(byte* name, uint id, Vector2 size_arg, byte border, ImGuiWindowFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBeginColumns(byte* str_id, int count, ImGuiOldColumnFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginComboPopup(uint popup_id, ImRect bb, ImGuiComboFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginComboPreview();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBeginDockableDragDropSource(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBeginDockableDragDropTarget(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBeginDocked(ImGuiWindow* window, byte* p_open);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginDragDropTargetCustom(ImRect bb, uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginMenuEx(byte* label, byte* icon, byte enabled);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginPopupEx(uint id, ImGuiWindowFlags extra_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginTabBarEx(ImGuiTabBar* tab_bar, ImRect bb, ImGuiTabBarFlags flags, ImGuiDockNode* dock_node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginTableEx(byte* name, uint id, int columns_count, ImGuiTableFlags flags, Vector2 outer_size, float inner_width);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginTooltipEx(ImGuiTooltipFlags tooltip_flags, ImGuiWindowFlags extra_window_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igBeginViewportSideBar(byte* name, ImGuiViewport* viewport, ImGuiDir dir, float size, ImGuiWindowFlags window_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToDisplayBack(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToDisplayBehind(ImGuiWindow* window, ImGuiWindow* above_window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToDisplayFront(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igBringWindowToFocusFront(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igButtonBehavior(ImRect bb, uint id, byte* out_hovered, byte* out_held, ImGuiButtonFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igButtonEx(byte* label, Vector2 size_arg, ImGuiButtonFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igCalcItemSize(Vector2* pOut, Vector2 size, float default_w, float default_h);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImDrawFlags igCalcRoundingFlagsForRectInRect(ImRect r_in, ImRect r_outer, float threshold);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igCalcTypematicRepeatAmount(float t0, float t1, float repeat_delay, float repeat_rate);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igCalcWindowNextAutoFitSize(Vector2* pOut, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igCalcWrapWidthForPos(Vector2 pos, float wrap_pos_x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igCallContextHooks(IntPtr context, ImGuiContextHookType type);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igCheckboxFlags_S64Ptr(byte* label, long* flags, long flags_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igCheckboxFlags_U64Ptr(byte* label, ulong* flags, ulong flags_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClearActiveID();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClearDragDrop();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClearIniSettings();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClearWindowSettings(byte* name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igCloseButton(uint id, Vector2 pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClosePopupsExceptModals();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClosePopupsOverWindow(ImGuiWindow* ref_window, byte restore_focus_to_window_under_popup);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igClosePopupToLevel(int remaining, byte restore_focus_to_window_under_popup);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igCollapseButton(uint id, Vector2 pos, ImGuiDockNode* dock_node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igColorEditOptionsPopup(float* col, ImGuiColorEditFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igColorPickerOptionsPopup(float* ref_col, ImGuiColorEditFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igColorTooltip(byte* text, float* col, ImGuiColorEditFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKey igConvertShortcutMod(ImGuiKey key_chord);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKey igConvertSingleModFlagToKey(IntPtr ctx, ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindowSettings* igCreateNewWindowSettings(byte* name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDataTypeApplyFromText(byte* buf, ImGuiDataType data_type, void* p_data, byte* format);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDataTypeApplyOp(ImGuiDataType data_type, int op, void* output, void* arg_1, void* arg_2);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDataTypeClamp(ImGuiDataType data_type, void* p_data, void* p_min, void* p_max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igDataTypeCompare(ImGuiDataType data_type, void* arg_1, void* arg_2);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igDataTypeFormatString(byte* buf, int buf_size, ImGuiDataType data_type, void* p_data, byte* format);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDataTypeInfo* igDataTypeGetInfo(ImGuiDataType data_type);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugDrawItemRect(uint col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugHookIdInfo(uint id, ImGuiDataType data_type, void* data_id, void* data_id_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugLocateItem(uint target_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugLocateItemOnHover(uint target_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugLocateItemResolveWithLastItem();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugLog(byte* fmt);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeColumns(ImGuiOldColumns* columns);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeDockNode(ImGuiDockNode* node, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeDrawCmdShowMeshAndBoundingBox(ImDrawList* out_draw_list, ImDrawList* draw_list, ImDrawCmd* draw_cmd, byte show_mesh, byte show_aabb);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeDrawList(ImGuiWindow* window, ImGuiViewportP* viewport, ImDrawList* draw_list, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeFont(ImFont* font);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeFontGlyph(ImFont* font, ImFontGlyph* glyph);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeInputTextState(ImGuiInputTextState* state);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeStorage(ImGuiStorage* storage, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeTabBar(ImGuiTabBar* tab_bar, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeTableSettings(ImGuiTableSettings* settings);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeViewport(ImGuiViewportP* viewport);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeWindow(ImGuiWindow* window, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeWindowSettings(ImGuiWindowSettings* settings);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeWindowsList(ImVector* windows, byte* label);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugNodeWindowsListByBeginStackParent(ImGuiWindow** windows, int windows_size, ImGuiWindow* parent_in_begin_stack);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugRenderKeyboardPreview(ImDrawList* draw_list);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugRenderViewportThumbnail(ImDrawList* draw_list, ImGuiViewportP* viewport, ImRect bb);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDebugStartItemPicker();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDestroyPlatformWindow(ImGuiViewportP* viewport);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igDockBuilderAddNode(uint node_id, ImGuiDockNodeFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderCopyDockSpace(uint src_dockspace_id, uint dst_dockspace_id, ImVector* in_window_remap_pairs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderCopyNode(uint src_node_id, uint dst_node_id, ImVector* out_node_remap_pairs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderCopyWindowSettings(byte* src_name, byte* dst_name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderDockWindow(byte* window_name, uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderFinish(uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* igDockBuilderGetCentralNode(uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* igDockBuilderGetNode(uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderRemoveNode(uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderRemoveNodeChildNodes(uint node_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderRemoveNodeDockedWindows(uint node_id, byte clear_settings_refs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderSetNodePos(uint node_id, Vector2 pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockBuilderSetNodeSize(uint node_id, Vector2 size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igDockBuilderSplitNode(uint node_id, ImGuiDir split_dir, float size_ratio_for_node_at_dir, uint* out_id_at_dir, uint* out_id_at_opposite_dir);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDockContextCalcDropPosForDocking(ImGuiWindow* target, ImGuiDockNode* target_node, ImGuiWindow* payload_window, ImGuiDockNode* payload_node, ImGuiDir split_dir, byte split_outer, Vector2* out_pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextClearNodes(IntPtr ctx, uint root_id, byte clear_settings_refs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextEndFrame(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* igDockContextFindNodeByID(IntPtr ctx, uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igDockContextGenNodeID(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextInitialize(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextNewFrameUpdateDocking(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextNewFrameUpdateUndocking(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextProcessUndockNode(IntPtr ctx, ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextProcessUndockWindow(IntPtr ctx, ImGuiWindow* window, byte clear_persistent_docking_ref);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextQueueDock(IntPtr ctx, ImGuiWindow* target, ImGuiDockNode* target_node, ImGuiWindow* payload, ImGuiDir split_dir, float split_ratio, byte split_outer);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextQueueUndockNode(IntPtr ctx, ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextQueueUndockWindow(IntPtr ctx, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextRebuildNodes(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockContextShutdown(IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDockNodeBeginAmendTabBar(ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockNodeEndAmendTabBar();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igDockNodeGetDepth(ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* igDockNodeGetRootNode(ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igDockNodeGetWindowMenuButtonId(ImGuiDockNode* node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDockNodeIsInHierarchyOf(ImGuiDockNode* node, ImGuiDockNode* parent);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igDockNodeWindowMenuHandler_Default(IntPtr ctx, ImGuiDockNode* node, ImGuiTabBar* tab_bar);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igDragBehavior(uint id, ImGuiDataType data_type, void* p_v, float v_speed, void* p_min, void* p_max, byte* format, ImGuiSliderFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igEndColumns();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igEndComboPreview();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igErrorCheckEndFrameRecover(IntPtr log_callback, void* user_data);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igErrorCheckEndWindowRecover(IntPtr log_callback, void* user_data);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igErrorCheckUsingSetCursorPosToExtendParentBoundaries();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFindBestWindowPosForPopup(Vector2* pOut, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFindBestWindowPosForPopupEx(Vector2* pOut, Vector2 ref_pos, Vector2 size, IntPtr last_dir, ImRect r_outer, ImRect r_avoid, ImGuiPopupPositionPolicy policy);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igFindBlockingModal(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igFindBottomMostVisibleWindowWithinBeginStack(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiViewportP* igFindHoveredViewportFromPlatformWindowStack(Vector2 mouse_platform_pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiOldColumns* igFindOrCreateColumns(ImGuiWindow* window, uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igFindRenderedTextEnd(byte* text, byte* text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiSettingsHandler* igFindSettingsHandler(byte* type_name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igFindWindowByID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igFindWindowByName(byte* name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igFindWindowDisplayIndex(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindowSettings* igFindWindowSettingsByID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindowSettings* igFindWindowSettingsByWindow(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFocusItem();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFocusTopMostWindowUnderOne(ImGuiWindow* under_this_window, ImGuiWindow* ignore_window, ImGuiViewport* filter_viewport, ImGuiFocusRequestFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFocusWindow(ImGuiWindow* window, ImGuiFocusRequestFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGcAwakeTransientWindowBuffers(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGcCompactTransientMiscBuffers();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGcCompactTransientWindowBuffers(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetActiveID();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igGetColumnNormFromOffset(ImGuiOldColumns* columns, float offset);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igGetColumnOffsetFromNorm(ImGuiOldColumns* columns, float offset_norm);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetColumnsID(byte* str_id, int count);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetContentRegionMaxAbs(Vector2* pOut);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetCurrentFocusScope();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabBar* igGetCurrentTabBar();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetCurrentWindow();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetCurrentWindowRead();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImFont* igGetDefaultFont();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetFocusID();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImDrawList* igGetForegroundDrawList_WindowPtr(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetHoveredID();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetIDWithSeed_Str(byte* str_id_begin, byte* str_id_end, uint seed);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetIDWithSeed_Int(int n, uint seed);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiInputTextState* igGetInputTextState(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiItemFlags igGetItemFlags();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiItemStatusFlags igGetItemStatusFlags();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetKeyChordName(ImGuiKey key_chord, byte* out_buf, int out_buf_size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyData* igGetKeyData_ContextPtr(IntPtr ctx, ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyData* igGetKeyData_Key(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetKeyMagnitude2d(Vector2* pOut, ImGuiKey key_left, ImGuiKey key_right, ImGuiKey key_up, ImGuiKey key_down);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetKeyOwner(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyOwnerData* igGetKeyOwnerData(IntPtr ctx, ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igGetNavTweakPressedAmount(ImGuiAxis axis);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetPopupAllowedExtentRect(ImRect* pOut, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyRoutingData* igGetShortcutRoutingData(ImGuiKey key_chord);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDataVarInfo* igGetStyleVarInfo(ImGuiStyleVar idx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetTopMostAndVisiblePopupModal();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetTopMostPopupModal();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetTypematicRepeatRate(ImGuiInputFlags flags, float* repeat_delay, float* repeat_rate);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiPlatformMonitor* igGetViewportPlatformMonitor(ImGuiViewport* viewport);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igGetWindowAlwaysWantOwnTabBar(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* igGetWindowDockNode();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetWindowResizeBorderID(ImGuiWindow* window, ImGuiDir dir);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetWindowResizeCornerID(ImGuiWindow* window, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igGetWindowScrollbarID(ImGuiWindow* window, ImGuiAxis axis);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igGetWindowScrollbarRect(ImRect* pOut, ImGuiWindow* window, ImGuiAxis axis);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImAbs_Int(int x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImAbs_Float(float x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern double igImAbs_double(double x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImageButtonEx(uint id, IntPtr texture_id, Vector2 size, Vector2 uv0, Vector2 uv1, Vector4 bg_col, Vector4 tint_col, ImGuiButtonFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igImAlphaBlendColors(uint col_a, uint col_b);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBezierCubicCalc(Vector2* pOut, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBezierCubicClosestPoint(Vector2* pOut, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p, int num_segments);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBezierCubicClosestPointCasteljau(Vector2* pOut, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p, float tess_tol);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBezierQuadraticCalc(Vector2* pOut, Vector2 p1, Vector2 p2, Vector2 p3, float t);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBitArrayClearAllBits(uint* arr, int bitcount);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBitArrayClearBit(uint* arr, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igImBitArrayGetStorageSizeInBytes(int bitcount);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBitArraySetBit(uint* arr, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImBitArraySetBitRange(uint* arr, int n, int n2);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImBitArrayTestBit(uint* arr, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImCharIsBlankA(byte c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImCharIsBlankW(uint c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImClamp(Vector2* pOut, Vector2 v, Vector2 mn, Vector2 mx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImDot(Vector2 a, Vector2 b);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImExponentialMovingAverage(float avg, float sample, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImFileClose(IntPtr file);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong igImFileGetSize(IntPtr file);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* igImFileLoadToMemory(byte* filename, byte* mode, uint* out_file_size, int padding_bytes);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr igImFileOpen(byte* filename, byte* mode);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong igImFileRead(void* data, ulong size, ulong count, IntPtr file);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong igImFileWrite(void* data, ulong size, ulong count, IntPtr file);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImFloor_Float(float f);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFloor_Vec2(Vector2* pOut, Vector2 v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImFloorSigned_Float(float f);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFloorSigned_Vec2(Vector2* pOut, Vector2 v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildFinish(ImFontAtlas* atlas);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildInit(ImFontAtlas* atlas);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildMultiplyCalcLookupTable(byte* out_table, float in_multiply_factor);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildMultiplyRectAlpha8(byte* table, byte* pixels, int x, int y, int w, int h, int stride);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildPackCustomRects(ImFontAtlas* atlas, void* stbrp_context_opaque);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildRender32bppRectFromString(ImFontAtlas* atlas, int x, int y, int w, int h, byte* in_str, byte in_marker_char, uint in_marker_pixel_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildRender8bppRectFromString(ImFontAtlas* atlas, int x, int y, int w, int h, byte* in_str, byte in_marker_char, byte in_marker_pixel_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFontAtlasBuildSetupFont(ImFontAtlas* atlas, ImFont* font, ImFontConfig* font_config, float ascent, float descent);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr* igImFontAtlasGetBuilderForStbTruetype();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImFormatString(byte* buf, uint buf_size, byte* fmt);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImFormatStringToTempBuffer(byte** out_buf, byte** out_buf_end, byte* fmt);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igImHashData(void* data, uint data_size, uint seed);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint igImHashStr(byte* data, uint data_size, uint seed);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImInvLength(Vector2 lhs, float fail_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImIsFloatAboveGuaranteedIntegerPrecision(float f);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImIsPowerOfTwo_Int(int v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImIsPowerOfTwo_U64(ulong v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImLengthSqr_Vec2(Vector2 lhs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImLengthSqr_Vec4(Vector4 lhs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImLerp_Vec2Float(Vector2* pOut, Vector2 a, Vector2 b, float t);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImLerp_Vec2Vec2(Vector2* pOut, Vector2 a, Vector2 b, Vector2 t);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImLerp_Vec4(Vector4* pOut, Vector4 a, Vector4 b, float t);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImLinearSweep(float current, float target, float speed);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImLineClosestPoint(Vector2* pOut, Vector2 a, Vector2 b, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImLog_Float(float x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern double igImLog_double(double x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImMax(Vector2* pOut, Vector2 lhs, Vector2 rhs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImMin(Vector2* pOut, Vector2 lhs, Vector2 rhs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImModPositive(int a, int b);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImMul(Vector2* pOut, Vector2 lhs, Vector2 rhs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImParseFormatFindEnd(byte* format);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImParseFormatFindStart(byte* format);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImParseFormatPrecision(byte* format, int default_value);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImParseFormatSanitizeForPrinting(byte* fmt_in, byte* fmt_out, uint fmt_out_size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImParseFormatSanitizeForScanning(byte* fmt_in, byte* fmt_out, uint fmt_out_size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImParseFormatTrimDecorations(byte* format, byte* buf, uint buf_size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImPow_Float(float x, float y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern double igImPow_double(double x, double y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImRotate(Vector2* pOut, Vector2 v, float cos_a, float sin_a);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImRsqrt_Float(float x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern double igImRsqrt_double(double x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImSaturate(float f);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImSign_Float(float x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern double igImSign_double(double x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort* igImStrbolW(ushort* buf_mid_line, ushort* buf_begin);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStrchrRange(byte* str_begin, byte* str_end, byte c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStrdup(byte* str);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStrdupcpy(byte* dst, uint* p_dst_size, byte* str);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStreolRange(byte* str, byte* str_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImStricmp(byte* str1, byte* str2);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStristr(byte* haystack, byte* haystack_end, byte* needle, byte* needle_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImStrlenW(ushort* str);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImStrncpy(byte* dst, byte* src, uint count);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImStrnicmp(byte* str1, byte* str2, uint count);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImStrSkipBlank(byte* str);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImStrTrimBlanks(byte* str);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextCharFromUtf8(uint* out_char, byte* in_text, byte* in_text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igImTextCharToUtf8(byte* out_buf, uint c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextCountCharsFromUtf8(byte* in_text, byte* in_text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextCountUtf8BytesFromChar(byte* in_text, byte* in_text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextCountUtf8BytesFromStr(ushort* in_text, ushort* in_text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextStrFromUtf8(ushort* out_buf, int out_buf_size, byte* in_text, byte* in_text_end, byte** in_remaining);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImTextStrToUtf8(byte* out_buf, int out_buf_size, ushort* in_text, ushort* in_text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImToUpper(byte c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igImTriangleArea(Vector2 a, Vector2 b, Vector2 c);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImTriangleBarycentricCoords(Vector2 a, Vector2 b, Vector2 c, Vector2 p, float* out_u, float* out_v, float* out_w);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igImTriangleClosestPoint(Vector2* pOut, Vector2 a, Vector2 b, Vector2 c, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igImTriangleContainsPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igImUpperPowerOfTwo(int v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igInitialize();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igInputTextDeactivateHook(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igInputTextEx(byte* label, byte* hint, byte* buf, int buf_size, Vector2 size_arg, ImGuiInputTextFlags flags, ImGuiInputTextCallback callback, void* user_data);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsActiveIdUsingNavDir(ImGuiDir dir);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsAliasKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsClippedEx(ImRect bb, uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsDragDropActive();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsDragDropPayloadBeingAccepted();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsGamepadKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsItemToggledSelection();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsKeyboardKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsKeyDown_ID(ImGuiKey key, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsKeyPressed_ID(ImGuiKey key, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsKeyPressedMap(ImGuiKey key, byte repeat);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsKeyReleased_ID(ImGuiKey key, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsLegacyKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsMouseClicked_ID(ImGuiMouseButton button, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsMouseDown_ID(ImGuiMouseButton button, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsMouseDragPastThreshold(ImGuiMouseButton button, float lock_threshold);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsMouseKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsMouseReleased_ID(ImGuiMouseButton button, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsNamedKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsNamedKeyOrModKey(ImGuiKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsPopupOpen_ID(uint id, ImGuiPopupFlags popup_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsWindowAbove(ImGuiWindow* potential_above, ImGuiWindow* potential_below);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsWindowChildOf(ImGuiWindow* window, ImGuiWindow* potential_parent, byte popup_hierarchy, byte dock_hierarchy);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsWindowContentHoverable(ImGuiWindow* window, ImGuiHoveredFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsWindowNavFocusable(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igIsWindowWithinBeginStackOf(ImGuiWindow* window, ImGuiWindow* potential_parent);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igItemAdd(ImRect bb, uint id, ImRect* nav_bb, ImGuiItemFlags extra_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igItemHoverable(ImRect bb, uint id, ImGuiItemFlags item_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igItemSize_Vec2(Vector2 size, float text_baseline_y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igItemSize_Rect(ImRect bb, float text_baseline_y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igKeepAliveID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igLocalizeGetMsg(ImGuiLocKey key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igLocalizeRegisterEntries(ImGuiLocEntry* entries, int count);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igLogBegin(ImGuiLogType type, int auto_open_depth);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igLogRenderedText(Vector2* ref_pos, byte* text, byte* text_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igLogSetNextTextDecoration(byte* prefix, byte* suffix);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igLogToBuffer(int auto_open_depth);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igMarkIniSettingsDirty_Nil();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igMarkIniSettingsDirty_WindowPtr(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igMarkItemEdited(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igMenuItemEx(byte* label, byte* icon, byte* shortcut, byte selected, byte enabled);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKey igMouseButtonToKey(ImGuiMouseButton button);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavClearPreferredPosForAxis(ImGuiAxis axis);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavInitRequestApplyResult();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavInitWindow(ImGuiWindow* window, byte force_reinit);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestApplyResult();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igNavMoveRequestButNoResultYet();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestCancel();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestForward(ImGuiDir move_dir, ImGuiDir clip_dir, ImGuiNavMoveFlags move_flags, ImGuiScrollFlags scroll_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestResolveWithLastItem(ImGuiNavItemData* result);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestSubmit(ImGuiDir move_dir, ImGuiDir clip_dir, ImGuiNavMoveFlags move_flags, ImGuiScrollFlags scroll_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavMoveRequestTryWrapping(ImGuiWindow* window, ImGuiNavMoveFlags move_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igNavUpdateCurrentWindowIsScrollPushableX();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igOpenPopupEx(uint id, ImGuiPopupFlags popup_flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPopColumnsBackground();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPopFocusScope();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPopItemFlag();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushColumnClipRect(int column_index);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushColumnsBackground();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushFocusScope(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushItemFlag(ImGuiItemFlags option, byte enabled);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushMultiItemsWidths(int components, float width_full);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igPushOverrideID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRemoveContextHook(IntPtr context, uint hook_to_remove);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRemoveSettingsHandler(byte* type_name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderArrow(ImDrawList* draw_list, Vector2 pos, uint col, ImGuiDir dir, float scale);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderArrowDockMenu(ImDrawList* draw_list, Vector2 p_min, float sz, uint col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderArrowPointingAt(ImDrawList* draw_list, Vector2 pos, Vector2 half_sz, ImGuiDir direction, uint col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderBullet(ImDrawList* draw_list, Vector2 pos, uint col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderCheckMark(ImDrawList* draw_list, Vector2 pos, uint col, float sz);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderColorRectWithAlphaCheckerboard(ImDrawList* draw_list, Vector2 p_min, Vector2 p_max, uint fill_col, float grid_step, Vector2 grid_off, float rounding, ImDrawFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderDragDropTargetRect(ImRect bb);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderFrame(Vector2 p_min, Vector2 p_max, uint fill_col, byte border, float rounding);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderFrameBorder(Vector2 p_min, Vector2 p_max, float rounding);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderMouseCursor(Vector2 pos, float scale, ImGuiMouseCursor mouse_cursor, uint col_fill, uint col_border, uint col_shadow);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderNavHighlight(ImRect bb, uint id, ImGuiNavHighlightFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderRectFilledRangeH(ImDrawList* draw_list, ImRect rect, uint col, float x_start_norm, float x_end_norm, float rounding);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderRectFilledWithHole(ImDrawList* draw_list, ImRect outer, ImRect inner, uint col, float rounding);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderText(Vector2 pos, byte* text, byte* text_end, byte hide_text_after_hash);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderTextClipped(Vector2 pos_min, Vector2 pos_max, byte* text, byte* text_end, Vector2* text_size_if_known, Vector2 align, ImRect* clip_rect);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderTextClippedEx(ImDrawList* draw_list, Vector2 pos_min, Vector2 pos_max, byte* text, byte* text_end, Vector2* text_size_if_known, Vector2 align, ImRect* clip_rect);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderTextEllipsis(ImDrawList* draw_list, Vector2 pos_min, Vector2 pos_max, float clip_max_x, float ellipsis_max_x, byte* text, byte* text_end, Vector2* text_size_if_known);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igRenderTextWrapped(Vector2 pos, byte* text, byte* text_end, float wrap_width);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScaleWindowsInViewport(ImGuiViewportP* viewport, float scale);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScrollbar(ImGuiAxis axis);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igScrollbarEx(ImRect bb, uint id, ImGuiAxis axis, long* p_scroll_v, long avail_v, long contents_v, ImDrawFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScrollToBringRectIntoView(ImGuiWindow* window, ImRect rect);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScrollToItem(ImGuiScrollFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScrollToRect(ImGuiWindow* window, ImRect rect, ImGuiScrollFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igScrollToRectEx(Vector2* pOut, ImGuiWindow* window, ImRect rect, ImGuiScrollFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSeparatorEx(ImGuiSeparatorFlags flags, float thickness);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSeparatorTextEx(uint id, byte* label, byte* label_end, float extra_width);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetActiveID(uint id, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetActiveIdUsingAllKeyboardKeys();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetCurrentFont(ImFont* font);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetCurrentViewport(ImGuiWindow* window, ImGuiViewportP* viewport);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetFocusID(uint id, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetHoveredID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetItemKeyOwner(ImGuiKey key, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetKeyOwner(ImGuiKey key, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetKeyOwnersForKeyChord(ImGuiKey key, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetLastItemData(uint item_id, ImGuiItemFlags in_flags, ImGuiItemStatusFlags status_flags, ImRect item_rect);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetNavID(uint id, ImGuiNavLayer nav_layer, uint focus_scope_id, ImRect rect_rel);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetNavWindow(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetScrollFromPosX_WindowPtr(ImGuiWindow* window, float local_x, float center_x_ratio);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetScrollFromPosY_WindowPtr(ImGuiWindow* window, float local_y, float center_y_ratio);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetScrollX_WindowPtr(ImGuiWindow* window, float scroll_x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetScrollY_WindowPtr(ImGuiWindow* window, float scroll_y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igSetShortcutRouting(ImGuiKey key_chord, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowClipRectBeforeSetChannel(ImGuiWindow* window, ImRect clip_rect);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowCollapsed_WindowPtr(ImGuiWindow* window, byte collapsed, ImGuiCond cond);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowDock(ImGuiWindow* window, uint dock_id, ImGuiCond cond);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowHiddendAndSkipItemsForCurrentFrame(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowHitTestHole(ImGuiWindow* window, Vector2 pos, Vector2 size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowPos_WindowPtr(ImGuiWindow* window, Vector2 pos, ImGuiCond cond);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowSize_WindowPtr(ImGuiWindow* window, Vector2 size, ImGuiCond cond);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igSetWindowViewport(ImGuiWindow* window, ImGuiViewportP* viewport);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igShadeVertsLinearColorGradientKeepAlpha(ImDrawList* draw_list, int vert_start_idx, int vert_end_idx, Vector2 gradient_p0, Vector2 gradient_p1, uint col0, uint col1);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igShadeVertsLinearUV(ImDrawList* draw_list, int vert_start_idx, int vert_end_idx, Vector2 a, Vector2 b, Vector2 uv_a, Vector2 uv_b, byte clamp);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igShortcut(ImGuiKey key_chord, uint owner_id, ImGuiInputFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igShowFontAtlas(ImFontAtlas* atlas);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igShrinkWidths(ImGuiShrinkWidthItem* items, int count, float width_excess);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igShutdown();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igSliderBehavior(ImRect bb, uint id, ImGuiDataType data_type, void* p_v, void* p_min, void* p_max, byte* format, ImGuiSliderFlags flags, ImRect* out_grab_bb);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igSplitterBehavior(ImRect bb, uint id, ImGuiAxis axis, float* size1, float* size2, float min_size1, float min_size2, float hover_extend, float hover_visibility_delay, uint bg_col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igStartMouseMovingWindow(ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igStartMouseMovingWindowOrNode(ImGuiWindow* window, ImGuiDockNode* node, byte undock_floating_node);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarAddTab(ImGuiTabBar* tab_bar, ImGuiTabItemFlags tab_flags, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarCloseTab(ImGuiTabBar* tab_bar, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* igTabBarFindMostRecentlySelectedTabForActiveWindow(ImGuiTabBar* tab_bar);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* igTabBarFindTabByID(ImGuiTabBar* tab_bar, uint tab_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* igTabBarFindTabByOrder(ImGuiTabBar* tab_bar, int order);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* igTabBarGetCurrentTab(ImGuiTabBar* tab_bar);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* igTabBarGetTabName(ImGuiTabBar* tab_bar, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igTabBarGetTabOrder(ImGuiTabBar* tab_bar, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTabBarProcessReorder(ImGuiTabBar* tab_bar);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarQueueFocus(ImGuiTabBar* tab_bar, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarQueueReorder(ImGuiTabBar* tab_bar, ImGuiTabItem* tab, int offset);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarQueueReorderFromMousePos(ImGuiTabBar* tab_bar, ImGuiTabItem* tab, Vector2 mouse_pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabBarRemoveTab(ImGuiTabBar* tab_bar, uint tab_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabItemBackground(ImDrawList* draw_list, ImRect bb, ImGuiTabItemFlags flags, uint col);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabItemCalcSize_Str(Vector2* pOut, byte* label, byte has_close_button_or_unsaved_marker);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabItemCalcSize_WindowPtr(Vector2* pOut, ImGuiWindow* window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTabItemEx(ImGuiTabBar* tab_bar, byte* label, byte* p_open, ImGuiTabItemFlags flags, ImGuiWindow* docked_window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTabItemLabelAndCloseButton(ImDrawList* draw_list, ImRect bb, ImGuiTabItemFlags flags, Vector2 frame_padding, byte* label, uint tab_id, uint close_button_id, byte is_contents_visible, byte* out_just_closed, byte* out_text_clipped);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableGcCompactSettings();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableGcCompactTransientBuffers_TableTempDataPtr(ImGuiTableTempData* table);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiSortDirection igTableGetColumnNextSortDirection(ImGuiTableColumn* column);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float igTableGetHeaderRowHeight();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int igTableGetHoveredColumn();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableOpenContextMenu(int column_n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTablePopBackgroundChannel();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTablePushBackgroundChannel();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableSetColumnSortDirection(int column_n, ImGuiSortDirection sort_direction, byte append_to_sort_specs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableSetColumnWidth(int column_n, float width);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTableSettingsAddSettingsHandler();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableSettings* igTableSettingsCreate(uint id, int columns_count);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableSettings* igTableSettingsFindByID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTempInputIsActive(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTempInputScalar(ImRect bb, uint id, byte* label, ImGuiDataType data_type, void* p_data, byte* format, void* p_clamp_min, void* p_clamp_max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTempInputText(ImRect bb, uint id, byte* label, byte* buf, int buf_size, ImGuiInputTextFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTestKeyOwner(ImGuiKey key, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTestShortcutRouting(ImGuiKey key_chord, uint owner_id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTextEx(byte* text, byte* text_end, ImGuiTextFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTranslateWindowsInViewport(ImGuiViewportP* viewport, Vector2 old_pos, Vector2 new_pos);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTreeNodeBehavior(uint id, ImGuiTreeNodeFlags flags, byte* label, byte* label_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTreeNodeSetOpen(uint id, byte open);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte igTreeNodeUpdateNextOpen(uint id, ImGuiTreeNodeFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igTreePushOverrideID(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igUpdateHoveredWindowAndCaptureFlags();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igUpdateInputEvents(byte trickle_fast_inputs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igUpdateMouseMovingWindowEndFrame();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igUpdateMouseMovingWindowNewFrame();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igUpdateWindowParentAndRootLinks(ImGuiWindow* window, ImGuiWindowFlags flags, ImGuiWindow* parent_window);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igWindowPosRelToAbs(Vector2* pOut, ImGuiWindow* window, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igWindowRectAbsToRel(ImRect* pOut, ImGuiWindow* window, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igWindowRectRelToAbs(ImRect* pOut, ImGuiWindow* window, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImBitVector_Clear(ImBitVector* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImBitVector_ClearBit(ImBitVector* self, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImBitVector_Create(ImBitVector* self, int sz);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImBitVector_SetBit(ImBitVector* self, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImBitVector_TestBit(ImBitVector* self, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_Clear(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_ClearFreeMemory(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_FlattenIntoSingleLayer(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImDrawDataBuilder_GetDrawListCount(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawListSharedData_destroy(IntPtr self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ImDrawListSharedData_ImDrawListSharedData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawListSharedData_SetCircleTessellationMaxError(IntPtr self, float max_error);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiComboPreviewData_destroy(ImGuiComboPreviewData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiComboPreviewData* ImGuiComboPreviewData_ImGuiComboPreviewData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiContext_destroy(IntPtr self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ImGuiContext_ImGuiContext(ImFontAtlas* shared_font_atlas);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiContextHook_destroy(ImGuiContextHook* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiContextHook* ImGuiContextHook_ImGuiContextHook();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* ImGuiDataVarInfo_GetVarPtr(ImGuiDataVarInfo* self, void* parent);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiDockContext_destroy(ImGuiDockContext* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockContext* ImGuiDockContext_ImGuiDockContext();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiDockNode_destroy(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiDockNode* ImGuiDockNode_ImGuiDockNode(uint id);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsCentralNode(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsDockSpace(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsEmpty(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsFloatingNode(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsHiddenTabBar(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsLeafNode(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsNoTabBar(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsRootNode(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiDockNode_IsSplitNode(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiDockNode_Rect(ImRect* pOut, ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiDockNode_SetLocalFlags(ImGuiDockNode* self, ImGuiDockNodeFlags flags);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiDockNode_UpdateMergedFlags(ImGuiDockNode* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextDeactivatedState_ClearFreeMemory(ImGuiInputTextDeactivatedState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextDeactivatedState_destroy(ImGuiInputTextDeactivatedState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiInputTextDeactivatedState* ImGuiInputTextDeactivatedState_ImGuiInputTextDeactivatedState();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_ClearFreeMemory(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_ClearSelection(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_ClearText(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_CursorAnimReset(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_CursorClamp(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_destroy(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiInputTextState_GetCursorPos(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiInputTextState_GetRedoAvailCount(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiInputTextState_GetSelectionEnd(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiInputTextState_GetSelectionStart(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiInputTextState_GetUndoAvailCount(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuiInputTextState_HasSelection(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiInputTextState* ImGuiInputTextState_ImGuiInputTextState();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_OnKeyPressed(ImGuiInputTextState* self, int key);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiInputTextState_SelectAll(ImGuiInputTextState* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiKeyOwnerData_destroy(ImGuiKeyOwnerData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyOwnerData* ImGuiKeyOwnerData_ImGuiKeyOwnerData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiKeyRoutingData_destroy(ImGuiKeyRoutingData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyRoutingData* ImGuiKeyRoutingData_ImGuiKeyRoutingData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiKeyRoutingTable_Clear(ImGuiKeyRoutingTable* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiKeyRoutingTable_destroy(ImGuiKeyRoutingTable* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiKeyRoutingTable* ImGuiKeyRoutingTable_ImGuiKeyRoutingTable();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiLastItemData_destroy(ImGuiLastItemData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiLastItemData* ImGuiLastItemData_ImGuiLastItemData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiListClipperData_destroy(ImGuiListClipperData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiListClipperData* ImGuiListClipperData_ImGuiListClipperData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiListClipperData_Reset(ImGuiListClipperData* self, ImGuiListClipper* clipper);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiListClipperRange ImGuiListClipperRange_FromIndices(int min, int max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiListClipperRange ImGuiListClipperRange_FromPositions(float y1, float y2, int off_min, int off_max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiMenuColumns_CalcNextTotalWidth(ImGuiMenuColumns* self, byte update_offsets);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImGuiMenuColumns_DeclColumns(ImGuiMenuColumns* self, float w_icon, float w_label, float w_shortcut, float w_mark);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiMenuColumns_destroy(ImGuiMenuColumns* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiMenuColumns* ImGuiMenuColumns_ImGuiMenuColumns();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiMenuColumns_Update(ImGuiMenuColumns* self, float spacing, byte window_reappearing);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNavItemData_Clear(ImGuiNavItemData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNavItemData_destroy(ImGuiNavItemData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiNavItemData* ImGuiNavItemData_ImGuiNavItemData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNextItemData_ClearFlags(ImGuiNextItemData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNextItemData_destroy(ImGuiNextItemData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiNextItemData* ImGuiNextItemData_ImGuiNextItemData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNextWindowData_ClearFlags(ImGuiNextWindowData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiNextWindowData_destroy(ImGuiNextWindowData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiNextWindowData* ImGuiNextWindowData_ImGuiNextWindowData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiOldColumnData_destroy(ImGuiOldColumnData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiOldColumnData* ImGuiOldColumnData_ImGuiOldColumnData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiOldColumns_destroy(ImGuiOldColumns* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiOldColumns* ImGuiOldColumns_ImGuiOldColumns();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiPopupData_destroy(ImGuiPopupData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiPopupData* ImGuiPopupData_ImGuiPopupData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiPtrOrIndex_destroy(ImGuiPtrOrIndex* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiPtrOrIndex* ImGuiPtrOrIndex_ImGuiPtrOrIndex_Ptr(void* ptr);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiPtrOrIndex* ImGuiPtrOrIndex_ImGuiPtrOrIndex_Int(int index);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiSettingsHandler_destroy(ImGuiSettingsHandler* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiSettingsHandler* ImGuiSettingsHandler_ImGuiSettingsHandler();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiStackLevelInfo_destroy(ImGuiStackLevelInfo* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiStackLevelInfo* ImGuiStackLevelInfo_ImGuiStackLevelInfo();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiStackSizes_CompareWithContextState(ImGuiStackSizes* self, IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiStackSizes_destroy(ImGuiStackSizes* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiStackSizes* ImGuiStackSizes_ImGuiStackSizes();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiStackSizes_SetToContextState(ImGuiStackSizes* self, IntPtr ctx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiStackTool_destroy(ImGuiStackTool* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiStackTool* ImGuiStackTool_ImGuiStackTool();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTabBar_destroy(ImGuiTabBar* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabBar* ImGuiTabBar_ImGuiTabBar();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTabItem_destroy(ImGuiTabItem* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* ImGuiTabItem_ImGuiTabItem();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTableColumn_destroy(ImGuiTableColumn* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableColumn* ImGuiTableColumn_ImGuiTableColumn();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTableColumnSettings_destroy(ImGuiTableColumnSettings* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableColumnSettings* ImGuiTableColumnSettings_ImGuiTableColumnSettings();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTableInstanceData_destroy(ImGuiTableInstanceData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableInstanceData* ImGuiTableInstanceData_ImGuiTableInstanceData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTableSettings_destroy(ImGuiTableSettings* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableColumnSettings* ImGuiTableSettings_GetColumnSettings(ImGuiTableSettings* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableSettings* ImGuiTableSettings_ImGuiTableSettings();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTableTempData_destroy(ImGuiTableTempData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTableTempData* ImGuiTableTempData_ImGuiTableTempData();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTextIndex_append(ImGuiTextIndex* self, byte* @base, int old_size, int new_size);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTextIndex_clear(ImGuiTextIndex* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ImGuiTextIndex_get_line_begin(ImGuiTextIndex* self, byte* @base, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ImGuiTextIndex_get_line_end(ImGuiTextIndex* self, byte* @base, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiTextIndex_size(ImGuiTextIndex* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_CalcWorkRectPos(Vector2* pOut, ImGuiViewportP* self, Vector2 off_min);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_CalcWorkRectSize(Vector2* pOut, ImGuiViewportP* self, Vector2 off_min, Vector2 off_max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_ClearRequestFlags(ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_destroy(ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_GetBuildWorkRect(ImRect* pOut, ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_GetMainRect(ImRect* pOut, ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_GetWorkRect(ImRect* pOut, ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiViewportP* ImGuiViewportP_ImGuiViewportP();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiViewportP_UpdateWorkRect(ImGuiViewportP* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImGuiWindow_CalcFontSize(ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiWindow_destroy(ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImGuiWindow_GetID_Str(ImGuiWindow* self, byte* str, byte* str_end);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImGuiWindow_GetID_Ptr(ImGuiWindow* self, void* ptr);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImGuiWindow_GetID_Int(ImGuiWindow* self, int n);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ImGuiWindow_GetIDFromRectangle(ImGuiWindow* self, ImRect r_abs);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* ImGuiWindow_ImGuiWindow(IntPtr context, byte* name);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImGuiWindow_MenuBarHeight(ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiWindow_MenuBarRect(ImRect* pOut, ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiWindow_Rect(ImRect* pOut, ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImGuiWindow_TitleBarHeight(ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiWindow_TitleBarRect(ImRect* pOut, ImGuiWindow* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiWindowSettings_destroy(ImGuiWindowSettings* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ImGuiWindowSettings_GetName(ImGuiWindowSettings* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindowSettings* ImGuiWindowSettings_ImGuiWindowSettings();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Add_Vec2(ImRect* self, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Add_Rect(ImRect* self, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_ClipWith(ImRect* self, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_ClipWithFull(ImRect* self, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImRect_Contains_Vec2(ImRect* self, Vector2 p);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImRect_Contains_Rect(ImRect* self, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_destroy(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Expand_Float(ImRect* self, float amount);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Expand_Vec2(ImRect* self, Vector2 amount);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Floor(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImRect_GetArea(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetBL(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetBR(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetCenter(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImRect_GetHeight(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetSize(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetTL(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_GetTR(Vector2* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float ImRect_GetWidth(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImRect* ImRect_ImRect_Nil();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImRect* ImRect_ImRect_Vec2(Vector2 min, Vector2 max);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImRect* ImRect_ImRect_Vec4(Vector4 v);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImRect* ImRect_ImRect_Float(float x1, float y1, float x2, float y2);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImRect_IsInverted(ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImRect_Overlaps(ImRect* self, ImRect r);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_ToVec4(Vector4* pOut, ImRect* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_Translate(ImRect* self, Vector2 d);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_TranslateX(ImRect* self, float dx);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImRect_TranslateY(ImRect* self, float dy);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImVec1_destroy(float* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float* ImVec1_ImVec1_Nil();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern float* ImVec1_ImVec1_Float(float _x);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImVec2ih_destroy(ImVec2ih* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImVec2ih* ImVec2ih_ImVec2ih_Nil();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImVec2ih* ImVec2ih_ImVec2ih_short(short _x, short _y);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImVec2ih* ImVec2ih_ImVec2ih_Vec2(Vector2 rhs);
    }
}
