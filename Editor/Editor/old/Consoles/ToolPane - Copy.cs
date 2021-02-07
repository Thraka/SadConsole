using System;
using SadConsole;
using Console = SadConsole.Consoles.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Consoles;
using Microsoft.Xna.Framework;
using SadConsoleEditor.Editors;
using SadConsoleEditor.Tools;
using SadConsoleEditor.Windows;
using SadConsole.Controls;
using SadConsoleEditor.Controls;
using SadConsoleEditor.Panels;
using SadConsole.Input;

namespace SadConsoleEditor.Consoles
{
    class ToolPane : ControlsConsole
    {
        private int RowFile = 1;
        private int RowTools = 6;
        private int RowToolSettings = 13;

        private ITool selectedTool;

        private Dictionary<string, ITool> _tools;

        

        public ITool SelectedTool
        {
            get { return selectedTool; }
            set
            {
                ToolsPanel.ToolsListBox.SelectedItem = value;
            }
        }

       
        

        public CharacterPickPanel CommonCharacterPickerPanel;
        public LayersPanel LayersPanel;
        public FilesPanel FilesPanel;
        public ToolsPanel ToolsPanel;

        private List<Tuple<CustomPanel, int>> _hotSpots;

        public static int PanelWidth;

        public ToolPane() : base(20, Settings.Config.WindowHeight * 2)
        {
            PanelWidth = 18;
            CommonCharacterPickerPanel = new CharacterPickPanel("Settings", false, false, false);
            CommonCharacterPickerPanel.PickerFont = Settings.Config.ScreenFont;
            CanUseKeyboard = false;

            _hotSpots = new List<Tuple<CustomPanel, int>>();
        }

        public void FinishCreating()
        {
            ProcessMouseWithoutFocus = true;
            
            textSurface.DefaultBackground = Settings.Color_MenuBack;
            textSurface.DefaultForeground = Settings.Color_TitleText;
            Clear();

            _tools = new Dictionary<string, ITool>();
            _tools.Add(PaintTool.ID, new PaintTool());
            _tools.Add(RecolorTool.ID, new RecolorTool());
            _tools.Add(FillTool.ID, new FillTool());
            _tools.Add(TextTool.ID, new TextTool());
            _tools.Add(LineTool.ID, new LineTool());
            _tools.Add(BoxTool.ID, new BoxTool());
            //_tools.Add(ObjectTool.ID, new ObjectTool());
            _tools.Add(SelectionTool.ID, new SelectionTool());
            _tools.Add(CircleTool.ID, new CircleTool());
            _tools.Add(EntityCenterTool.ID, new EntityCenterTool());
            _tools.Add(SceneEntityMoveTool.ID, new SceneEntityMoveTool());

            FilesPanel = new FilesPanel();
            ToolsPanel = new ToolsPanel();
            LayersPanel = new LayersPanel();

            FilesPanel.IsCollapsed = true;
            LayersPanel.IsCollapsed = true;

            ToolsPanel.ToolsListBox.SelectedItemChanged += (sender, e) =>
            {
                if (selectedTool != null)
                {
                    selectedTool.OnDeselected();
                    if (selectedTool.ControlPanels != null)
                        foreach (var pane in selectedTool.ControlPanels)
                        {
                            foreach (var control in pane.Controls)
                            {
                                Remove(control);
                            }
                        }
                }

                if (e.Item != null)
                {
                    selectedTool = (ITool)e.Item;
                    EditorConsoleManager.Instance.ToolName = selectedTool.Title;

                    EditorConsoleManager.Instance.AllowKeyboardToMoveConsole = true;
                    CommonCharacterPickerPanel.HideCharacter = false;
                    CommonCharacterPickerPanel.HideForeground = false;
                    CommonCharacterPickerPanel.HideBackground = false;

                    selectedTool.OnSelected();
                    CommonCharacterPickerPanel.Reset();
                    RefreshControls();
                }

            };
        }

        public void SetupEditor()
        {
            ToolsPanel.ToolsListBox.Items.Clear();

            foreach (var toolId in EditorConsoleManager.Instance.SelectedEditor.Tools)
                ToolsPanel.ToolsListBox.Items.Add(_tools[toolId]);

            ToolsPanel.ToolsListBox.SelectedItem = ToolsPanel.ToolsListBox.Items.Cast<ITool>().First();
        }

        public void RefreshControls()
        {
            int activeRow = 0;
            Clear();
            RemoveAll();
            _hotSpots.Clear();

            char open = (char)31;
            char closed = (char)16;

            List<CustomPanel> allPanels = new List<CustomPanel>();

            // Custom panels from the selected editor
            if (EditorConsoleManager.Instance.SelectedEditor.ControlPanels != null && EditorConsoleManager.Instance.SelectedEditor.ControlPanels.Length != 0)
                allPanels.AddRange(EditorConsoleManager.Instance.SelectedEditor.ControlPanels);

            // Custom panels from the selected tool
            if (SelectedTool.ControlPanels != null && SelectedTool.ControlPanels.Length != 0)
                allPanels.AddRange(SelectedTool.ControlPanels);

            // Display all panels needed
            if (allPanels.Count != 0)
            {
                foreach (var pane in allPanels)
                {
					if (pane.IsVisible)
					{
						pane.Loaded();
						_hotSpots.Add(new Tuple<CustomPanel, int>(pane, activeRow));
						if (pane.IsCollapsed == false)
						{
							Print(1, activeRow++, open + " " + pane.Title);
							Print(0, activeRow++, new string((char)196, textSurface.Width));

							foreach (var control in pane.Controls)
							{
                                if (control != null)
                                {
                                    Add(control);
                                    control.Position = new Point(1, activeRow);
                                    activeRow += pane.Redraw(control) + control.Height;
                                }
                                else
                                    activeRow++;
							}

							activeRow += 1;
						}
						else
							Print(1, activeRow++, closed + " " + pane.Title);
					}
                }
            }
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            foreach (var tool in ToolsPanel.ToolsListBox.Items.Cast<ITool>())
            {
                foreach (var key in info.KeysPressed)
                {
                    if (key.Character == tool.Hotkey)
                    {
                        ToolsPanel.ToolsListBox.SelectedItem = tool;
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool ProcessMouse(SadConsole.Input.MouseInfo info)
        {
            base.ProcessMouse(info);

            if (_isMouseOver)
            {
                if (info.ScrollWheelValueChange != 0)
                {
                    EditorConsoleManager.Instance.ScrollToolbox(info.ScrollWheelValueChange);
                    return true;
                }

                foreach (var item in _hotSpots)
                {
                    if (item.Item2 == info.ConsoleLocation.Y)
                    {
                        if (info.LeftClicked)
                        {
                            item.Item1.IsCollapsed = !item.Item1.IsCollapsed;
                            RefreshControls();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
