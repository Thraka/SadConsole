using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET.Internal;

namespace ImGuiNET.Internal
{
    public static unsafe partial class ImGuiNative
    {
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetCurrentWindow();






        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_Clear(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_ClearFreeMemory(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImDrawDataBuilder_FlattenIntoSingleLayer(ImDrawDataBuilder* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImDrawDataBuilder_GetDrawListCount(ImDrawDataBuilder* self);

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
        public static extern void ImGuiOldColumns_destroy(ImGuiOldColumns* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiOldColumns* ImGuiOldColumns_ImGuiOldColumns();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiOldColumnData_destroy(ImGuiOldColumnData* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiOldColumnData* ImGuiOldColumnData_ImGuiOldColumnData();




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
        public static extern void ImGuiTabBar_destroy(ImGuiTabBar* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* ImGuiTabBar_GetTabName(ImGuiTabBar* self, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ImGuiTabBar_GetTabOrder(ImGuiTabBar* self, ImGuiTabItem* tab);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabBar* ImGuiTabBar_ImGuiTabBar();
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuiTabItem_destroy(ImGuiTabItem* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiTabItem* ImGuiTabItem_ImGuiTabItem();



        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImVec1_destroy(ImVec1* self);
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImVec2ih_destroy(ImVec2ih* self);


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
    }
}
