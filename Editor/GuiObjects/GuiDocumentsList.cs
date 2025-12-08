using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;

public class GuiDocumentsList : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin(GuiDockSpace.ID_LEFT_PANEL);

        Document? oldDocument = Core.State.SelectedDocument;

        // Render the hierarchical document list
        if (ImGui.BeginChild("##doclist", new System.Numerics.Vector2(-1, ImGui.GetTextLineHeightWithSpacing() * 6), ImGuiChildFlags.Borders))
        {
            RenderDocumentHierarchy(Core.State.Documents.Objects, ref oldDocument);
        }
        ImGui.EndChild();

        // Handle selection changes
        if (oldDocument != Core.State.SelectedDocument)
        {
            oldDocument?.OnDeselected();
            Core.State.Tools.Objects.Clear();
            Core.State.SelectedDocument?.OnSelected();
        }

        // Render settings for selected document
        if (Core.State.SelectedDocument != null)
        {
            // Show breadcrumb if this is a child document
            if (Core.State.SelectedDocument.Parent != null)
            {
                ImGui.Separator();
                RenderBreadcrumb(Core.State.SelectedDocument);
                ImGui.Separator();
            }

            Core.State.SelectedDocument.BuildUiDocumentSettings(renderer);
        }

        ImGui.End();
    }

    private void RenderDocumentHierarchy(IEnumerable<Document> documents, ref Document? oldDocument)
    {
        foreach (Document doc in documents)
            RenderDocumentItem(doc, ref oldDocument);
    }

    private void RenderDocumentItem(Document doc, ref Document? oldDocument)
    {
        ImGui.PushID(doc.GetHashCode());

        float indent = doc.Depth * ImGui.GetFontSize();
        float padding = ImGui.GetStyle().FramePadding.X / 2;
        bool hasChildren = doc.CanHaveChildren && doc.ShowChildrenInHierarchy && doc.Children.Count > 0;
        bool isSelected = ReferenceEquals(Core.State.SelectedDocument, doc);

        // Calculate total width for the selectable
        float availableWidth = ImGui.GetContentRegionAvail().X;

        // Add indent
        if (indent > 0)
        {
            ImGui.Dummy(new System.Numerics.Vector2(indent, 0));
            ImGui.SameLine(0, 0);
        }

        // Show document type icon for all items
        ImGui.Text(doc.DocumentIcon);
        ImGui.SameLine();
        ImGui.Dummy(new System.Numerics.Vector2(padding, 0));
        ImGui.SameLine();

        // Render the selectable item
        if (ImGui.Selectable(doc.Title, isSelected, ImGuiSelectableFlags.None, new System.Numerics.Vector2(availableWidth - indent - 25, 0)))
        {
            oldDocument = Core.State.SelectedDocument;
            Core.State.SelectedDocument = doc;
        }

        ImGui.PopID();

        // Always render children (no expand/collapse)
        if (hasChildren)
        {
            foreach (Document child in doc.Children)
            {
                RenderDocumentItem(child, ref oldDocument);
            }
        }
    }

    private void RenderBreadcrumb(Document doc)
    {
        List<Document> path = HierarchyHelper.GetAncestorPath(doc);
        
        for (int i = 0; i < path.Count; i++)
        {
            if (i > 0)
            {
                ImGui.SameLine();
                ImGui.TextDisabled(">");
                ImGui.SameLine();
            }

            if (i < path.Count - 1)
            {
                // Clickable ancestor
                if (ImGui.SmallButton(path[i].Title))
                {
                    // Navigate to ancestor
                    Core.State.SelectedDocument?.OnDeselected();
                    Core.State.Tools.Objects.Clear();
                    Core.State.SelectedDocument = path[i];
                    Core.State.SelectedDocument.OnSelected();
                }
            }
            else
            {
                // Current item (not clickable)
                ImGui.Text(path[i].Title);
            }
        }
    }
}
