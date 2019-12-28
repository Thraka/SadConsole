using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.UI.Controls;
using SadConsole.Input;
using SadRogue.Primitives;
using SadConsole;
using SadConsoleEditor.Windows;

namespace SadConsoleEditor.Panels
{
    class SelectionToolAltPanel : CustomPanel
    {
        private CheckBox skipEmptyColor;
        private CheckBox altEmptyColorCheck;
        private Controls.ColorPresenter altEmptyColor;


        public bool SkipEmptyCells { get { return skipEmptyColor.IsSelected; } }
        public bool UseAltEmptyColor { get { return altEmptyColorCheck.IsSelected; } }
        public Color AltEmptyColor { get { return altEmptyColor.SelectedColor; } }

        public SelectionToolAltPanel()
        {
            Title = "Sel.Rect Options";

            skipEmptyColor = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            skipEmptyColor.Text = "Skip Empty";

            altEmptyColorCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            altEmptyColorCheck.Text = "Use Alt. Empty";

            altEmptyColor = new Controls.ColorPresenter("Alt. Empty Clr", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            altEmptyColor.SelectedColor = Color.Black;

            Controls = new ControlBase[] { skipEmptyColor, altEmptyColorCheck, altEmptyColor };
        }

        public override void ProcessMouse(MouseScreenObjectState info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            if (control == skipEmptyColor)
                return 1;

            return 0;
        }

        public override void Loaded()
        {
        }
    }

    class SelectionToolPanel : CustomPanel
    {
        private CloneState state;
        private Button reset;
        private Button loadBrush;
        private Button saveBrush;
        private Button clone;
        private Button clear;
        private Button move;
        private Button stash;
        private Button restore;
        private Button createText;

        private string lastFolder = null;

        private Func<SadConsole.CellSurface> saveBrushHandler;
        private Action<SadConsole.CellSurface> loadBrushHandler;
        private Action stashBrushHandler;
        private Action restoreBrushHandler;
        private Action<CellSurface> createTextResultHandler;


        private TextMakerPopup textMaker;
        
        private int _currentStepChar = 175;


        public CloneState State
        {
            get { return state; }
            set
            {
                state = value;

                saveBrush.IsEnabled = value == CloneState.Selected;
                clone.IsEnabled = value == CloneState.Selected;
                clear.IsEnabled = value == CloneState.Selected;
                move.IsEnabled = value == CloneState.Selected;

                if (StateChangedHandler != null)
                    StateChangedHandler(value);
            } 
        }

        public Action<CloneState> StateChangedHandler;

        public enum CloneState
        {
            SelectingPoint1,
            Selected,
            Stamp,
            Clear,
            Move
        }



        public SelectionToolPanel(Action<CellSurface> loadBrushHandler, Func<CellSurface> saveBrushHandler, Action stashHandler, Action restoreHandler, Action<CellSurface> createTextResultHandler)
        {
            reset = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            reset.Text = "Reset Steps";
            reset.Click += (o, e) => State = CloneState.SelectingPoint1;

            loadBrush = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            loadBrush.Text = "Import Brush";
            loadBrush.Click += _loadBrush_Click;

            saveBrush = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            saveBrush.Text = "Export Brush";
            saveBrush.Click += _saveBrush_Click;

            clone = new Button(Consoles.ToolPane.PanelWidthControls);
            clone.Text = "Clone";
            clone.Click += clone_Click;

            clear = new Button(Consoles.ToolPane.PanelWidthControls);
            clear.Text = "Clear";
            clear.Click += clear_Click;

            move = new Button(Consoles.ToolPane.PanelWidthControls);
            move.Text = "Move";
            move.Click += move_Click;

            stash = new Button(Consoles.ToolPane.PanelWidthControls);
            stash.Text = "Stash";
            stash.Click += stash_Click;

            restore = new Button(Consoles.ToolPane.PanelWidthControls);
            restore.Text = "Restore";
            restore.Click += restore_Click;

            createText = new Button(Consoles.ToolPane.PanelWidthControls);
            createText.Text = "Import Text";
            createText.Click += createText_Click;

            textMaker = new TextMakerPopup();
            textMaker.Closed += (s, e) => { if (textMaker.DialogResult) this.createTextResultHandler(textMaker.SurfaceResult); };

            Controls = new ControlBase[] { reset, loadBrush, saveBrush, null, clone, clear, move, null, createText, null, stash, restore };

            this.loadBrushHandler = loadBrushHandler;
            this.saveBrushHandler = saveBrushHandler;
            this.stashBrushHandler = stashHandler;
            this.restoreBrushHandler = restoreHandler;
            this.createTextResultHandler = createTextResultHandler;

            Title = "Clone";
            State = CloneState.SelectingPoint1;
        }

        private void createText_Click(object sender, EventArgs e)
        {
            textMaker.Show(true);
        }


        private void stash_Click(object sender, EventArgs e)
        {
            stashBrushHandler();
        }

        private void restore_Click(object sender, EventArgs e)
        {
            restoreBrushHandler();
        }

        private void move_Click(object sender, EventArgs e)
        {
            State = CloneState.Move;
        }

        private void clear_Click(object sender, EventArgs e)
        {
            State = CloneState.Clear;
        }

        private void clone_Click(object sender, EventArgs e)
        {
            State = CloneState.Stamp;
        }

        private void _saveBrush_Click(object sender, EventArgs e)
        {
            var popup = new SelectFilePopup(new[] { "SURFACE", "TEXT" });
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    popup.SelectedLoader.Save(saveBrushHandler(), popup.SelectedFile);
                }
            };
            popup.CurrentFolder = lastFolder ?? Environment.CurrentDirectory;
            popup.SelectButtonText = "Save";
            popup.SkipFileExistCheck = true;
            popup.Show(true);
            popup.Center();
        }

        private void _loadBrush_Click(object sender, EventArgs e)
        {
            var popup = new SelectFilePopup(new[] { "SURFACE", "TEXT", "ANSI" });
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    if (System.IO.File.Exists(popup.SelectedFile))
                    {
                        lastFolder = System.IO.Path.GetDirectoryName(popup.SelectedFile);

                        //if (System.IO.Path.GetExtension(popup.SelectedFile) == ".ans")
                        //{
                        //    using (var ansi = new SadConsole.Ansi.Document(popup.SelectedFile))
                        //    {
                        //        var console = new SadConsole.Console(80, 1);
                        //        console.TextSurface.ResizeOnShift = true;
                        //        SadConsole.Ansi.AnsiWriter writer = new SadConsole.Ansi.AnsiWriter(ansi, console);
                        //        writer.ReadEntireDocument();
                        //        _loadBrushHandler(console.CellData);
                        //    }

                        //}
                        //else
                        loadBrushHandler((SadConsole.CellSurface)popup.SelectedLoader.Load(popup.SelectedFile));
                    }
                }
            };
            popup.CurrentFolder = lastFolder ?? Environment.CurrentDirectory;
            popup.Show(true);
            popup.Center();
        }

        public override void ProcessMouse(MouseScreenObjectState info)
        {

        }

        public override int Redraw(ControlBase control)
        {
            return 0;
        }

        public override void Loaded()
        {
        }
    }
}
