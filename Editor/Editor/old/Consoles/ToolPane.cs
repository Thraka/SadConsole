using System;
using SadConsole;
using Console = SadConsole.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Surfaces;
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
    class ToolPane : Screen
    {
        private ControlsConsole ToolsConsole;
        private ControlsConsole ScrollerConsole;

        public static int PanelWidth;
        public static int PanelWidthControls;

        public SadConsole.Controls.ScrollBar ToolsPaneScroller { get; private set; }

        private List<Tuple<CustomPanel, int>> _hotSpots;
        private Dictionary<string, ITool> _tools;

        public FilesPanel PanelFiles;
        //public ToolsPanel PanelTools;

        public int Width { get { return ToolsConsole.Width; } }
        
        public ToolPane()
        {
            ToolsConsole = new ControlsConsole(Settings.Config.ToolPaneWidth - 1, Settings.Config.WindowHeight * 3);
            ToolsConsole.MouseHandler = ProcessMouse;
            ToolsConsole.UseKeyboard = false;

            // Create scrollbar
            ToolsPaneScroller = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, Settings.Config.WindowHeight - 1);
            ToolsPaneScroller.Maximum = ToolsConsole.TextSurface.Height - Settings.Config.WindowHeight;
            ToolsPaneScroller.ValueChanged += (o, e) =>
            {
                ToolsConsole.TextSurface.RenderArea = new Rectangle(0, ToolsPaneScroller.Value, ToolsConsole.Width, Settings.Config.WindowHeight);
            };

            ScrollerConsole = new ControlsConsole(1, Settings.Config.WindowHeight - 1);
            ScrollerConsole.Add(ToolsPaneScroller);
            ScrollerConsole.Position = new Point(Width, 0);
            ScrollerConsole.IsVisible = true;
            ScrollerConsole.FocusOnMouseClick = false;

            

            PanelWidth = Settings.Config.ToolPaneWidth - 1;
            PanelWidthControls = PanelWidth - 2;

            _tools = new Dictionary<string, ITool>();

            ToolsConsole.TextSurface.DefaultBackground = Settings.Color_MenuBack;
            ToolsConsole.TextSurface.DefaultForeground = Settings.Color_TitleText;
            ToolsConsole.Clear();

            _hotSpots = new List<Tuple<CustomPanel, int>>();

            // Create tools
            _tools.Add(PaintTool.ID, new PaintTool());
            ToolsConsole.TextSurface.RenderArea = new Rectangle(0, 0, ToolsConsole.Width, Settings.Config.WindowHeight - 1);

            // Create panels
            PanelFiles = new FilesPanel();
            //PanelTools = new ToolsPanel();

            Children.Add(ToolsConsole);
            Children.Add(ScrollerConsole);
        }


        public void RedrawPanels()
        {
            int activeRow = 0;
            ToolsConsole.Clear();
            ToolsConsole.RemoveAll();
            _hotSpots.Clear();

            char open = (char)31;
            char closed = (char)16;

            List<CustomPanel> allPanels = new List<CustomPanel>() { PanelFiles };

            // Custom panels from the selected editor
            if (MainScreen.Instance.ActiveEditor != null)
                if (MainScreen.Instance.ActiveEditor.Panels != null && MainScreen.Instance.ActiveEditor.Panels.Length != 0)
                    allPanels.AddRange(MainScreen.Instance.ActiveEditor.Panels);

            // Custom panels from the selected tool
            //if (SelectedTool.ControlPanels != null && SelectedTool.ControlPanels.Length != 0)
            //    allPanels.AddRange(SelectedTool.ControlPanels);

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
                            ToolsConsole.Print(1, activeRow++, open + " " + pane.Title);
                            ToolsConsole.Print(0, activeRow++, new string((char)196, ToolsConsole.TextSurface.Width));

                            foreach (var control in pane.Controls)
                            {
                                if (control != null)
                                {
                                    if (control.IsVisible)
                                    {
                                        ToolsConsole.Add(control);
                                        control.Position = new Point(1, activeRow);
                                        activeRow += pane.Redraw(control) + control.Height;
                                    }
                                }
                                else
                                    activeRow++;
                            }

                            activeRow += 1;
                        }
                        else
                            ToolsConsole.Print(1, activeRow++, closed + " " + pane.Title);
                    }
                }
            }

            int scrollAbility = activeRow + 1 - Settings.Config.WindowHeight;

            if (scrollAbility <= 0)
            {
                ToolsPaneScroller.IsEnabled = false;
                ToolsPaneScroller.Maximum = 0;
            }
            else
            {
                ToolsPaneScroller.Maximum = scrollAbility;
                ToolsPaneScroller.IsEnabled = true;
            }
        }

        

        public bool ProcessMouse(IConsole console, SadConsole.Input.MouseConsoleState info)
        {
            if (info.IsOnConsole)
            {
                if (MainScreen.Instance.Brush != null)
                    MainScreen.Instance.Brush.IsVisible = false;

                if (info.Mouse.ScrollWheelValueChange != 0)
                {
                    if (ToolsPaneScroller.IsEnabled)
                        ToolsPaneScroller.Value += info.Mouse.ScrollWheelValueChange / 20;
                    return true;
                }

                foreach (var item in _hotSpots)
                {
                    if (item.Item2 == info.CellPosition.Y)
                    {
                        if (info.Mouse.LeftClicked)
                        {
                            item.Item1.IsCollapsed = !item.Item1.IsCollapsed;
                            RedrawPanels();
                            return true;
                        }
                    }
                }

                return ToolsConsole.ProcessMouseNonHandler(info);
            }

            return false;
        }

        public bool ProcessKeyboard(Keyboard info)
        {
            return false;
        }
    }
}
