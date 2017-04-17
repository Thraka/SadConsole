using SadConsole;
using System;
using Console = SadConsole.Console;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class SerializationTests : ConsoleContainer, IConsoleMetadata
    {
        ControlsConsole controlsConsole;
        Console masterView;
        Console loadedView;

        // Controls
        SadConsole.Controls.RadioButton optionButtonSurface;
        SadConsole.Controls.RadioButton optionButtonView;
        SadConsole.Controls.RadioButton optionButtonLayered;
        SadConsole.Controls.RadioButton optionButtonAnimated;

        SadConsole.Surfaces.BasicSurface basicSurface;
        SadConsole.Surfaces.AnimatedSurface animatedSurface;
        SadConsole.Surfaces.SurfaceView viewSurface;
        SadConsole.Surfaces.LayeredSurface layeredSurface;
        SadConsole.Surfaces.BasicSurface emptySurface;


        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Serialization Tests", Summary = "Test serializing various types from SadConsole" };
            }
        }

        public SerializationTests()
        {
            controlsConsole = new ControlsConsole(80, 4);

            masterView = new Console(34, 15);
            loadedView = new Console(34, 15);

            masterView.Fill(Color.White, Color.Red, 0);
            loadedView.Fill(Color.White, Color.Blue, 0);

            UseMouse = true;

            // Add the consoles to the list.
            Children.Add(controlsConsole);
            Children.Add(masterView);
            Children.Add(loadedView);

            // Setup main view
            masterView.Position = new Point(3, 6);

            // Setup sub view
            loadedView.Position = new Point(80 - 37, 6);


            // Setup controls
            controlsConsole.Position = new Point(0, 0);

            optionButtonSurface = new SadConsole.Controls.RadioButton(18, 1)
            {
                Text = "Surface",
                Position = new Point(1, 1),
            };
            optionButtonSurface.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonSurface);

            optionButtonView = new SadConsole.Controls.RadioButton(18, 1)
            {
                Text = "Surface View",
                Position = new Point(1, 2)
            };
            optionButtonView.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonView);

            optionButtonLayered = new SadConsole.Controls.RadioButton(21, 1)
            {
                Text = "Layered Surface",
                Position = new Point(optionButtonSurface.Bounds.Right + 1, 1)
            };
            optionButtonLayered.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonLayered);

            optionButtonAnimated = new SadConsole.Controls.RadioButton(21, 1)
            {
                Text = "Animated Surface",
                Position = new Point(optionButtonSurface.Bounds.Right + 1, 2)
            };
            optionButtonAnimated.IsSelectedChanged += OptionButton_IsSelectedChanged;
            controlsConsole.Add(optionButtonAnimated);

            var buttonSave = new SadConsole.Controls.Button(17)
            {
                Text = "Save and Load",
                Position = new Point(controlsConsole.Width - 19, 1)
            };
            buttonSave.Click += ButtonSave_Click;
            controlsConsole.Add(buttonSave);

            basicSurface = new SadConsole.Surfaces.BasicSurface(34, 15);
            layeredSurface = new SadConsole.Surfaces.LayeredSurface(34, 15, 3);
            animatedSurface = SadConsole.GameHelpers.Animations.CreateStatic(34, 15, 15, 0.3d);
            viewSurface = new SadConsole.Surfaces.SurfaceView(basicSurface, new Rectangle(5, 2, 34 - 10, 15 - 4));
            emptySurface = (SadConsole.Surfaces.BasicSurface)loadedView.TextSurface;

            MakeBasicSurface();
            MakeLayeredSurface();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (optionButtonSurface.IsSelected)
            {
                basicSurface.Save("basicsurface.surface");
                loadedView.TextSurface = SadConsole.Surfaces.BasicSurface.Load("basicsurface.surface");
            }
            else if (optionButtonView.IsSelected)
            {
                viewSurface.Save("viewsurface.view");
                loadedView.TextSurface = SadConsole.Surfaces.SurfaceView.Load("viewsurface.view", basicSurface);
            }
            else if (optionButtonLayered.IsSelected)
            {
                layeredSurface.Save("layeredsurface.layers", typeof(SadConsole.Surfaces.LayerMetadata));
                loadedView.TextSurface = SadConsole.Surfaces.LayeredSurface.Load("layeredsurface.layers", typeof(SadConsole.Surfaces.LayerMetadata));
            }
            else if (optionButtonAnimated.IsSelected)
            {
                animatedSurface.Save("animatedsurface.animation");
                loadedView.TextSurface = SadConsole.Surfaces.AnimatedSurface.Load("animatedsurface.animation");
                ((SadConsole.Surfaces.AnimatedSurface)loadedView.TextSurface).Start();
            }
        }

        private void OptionButton_IsSelectedChanged(object sender, EventArgs e)
        {
            loadedView.TextSurface = emptySurface;

            if (optionButtonSurface.IsSelected)
            {
                masterView.TextSurface = basicSurface;
                masterView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
                loadedView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
            }
            else if (optionButtonView.IsSelected)
            {
                masterView.TextSurface = viewSurface;
                masterView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
                loadedView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
            }
            else if (optionButtonLayered.IsSelected)
            {
                masterView.TextSurface = layeredSurface;
                masterView.Renderer = new SadConsole.Renderers.LayeredSurfaceRenderer();
                loadedView.TextSurface = new SadConsole.Surfaces.LayeredSurface(34, 15, 1);
                loadedView.Renderer = new SadConsole.Renderers.LayeredSurfaceRenderer();
            }
            else if (optionButtonAnimated.IsSelected)
            {
                masterView.TextSurface = animatedSurface;
                masterView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
                loadedView.Renderer = new SadConsole.Renderers.SurfaceRenderer();
            }
        }

        public override void Update(TimeSpan delta)
        {
            if (optionButtonAnimated.IsSelected)
            {
                animatedSurface.Update();
                (loadedView.TextSurface as SadConsole.Surfaces.AnimatedSurface)?.Update();
                    
            }

            base.Update(delta);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Process mouse for each console
            var childState = new MouseConsoleState(controlsConsole, state.Mouse);

            if (childState.IsOnConsole)
                return controlsConsole.ProcessMouse(childState);

            return false;
        }

        private void MakeBasicSurface()
        {
            var editor = new SadConsole.Surfaces.SurfaceEditor(basicSurface);
            editor.Print(0, 0, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".Repeat(9));

            ColorGradient gradient = new ColorGradient(StarterProject.Theme.Blue, StarterProject.Theme.Yellow);
            for (int i = 0; i < 510; i += 10)
            {
                var point = editor.TextSurface.GetPointFromIndex(i);
                editor.Print(point.X, point.Y, gradient.ToColoredString(editor.GetString(i, 10)));
            }

            // Mirror 1
            int startSet1 = new Point(0, 3).ToIndex(34);
            int startSet2 = new Point(0, 6).ToIndex(34);
            int startSet3 = new Point(0, 9).ToIndex(34);

            for (int i = 0; i < 34 * 3; i++)
            {
                basicSurface.Cells[startSet1 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                basicSurface.Cells[startSet1 + i].Background = StarterProject.Theme.PurpleDark;

                basicSurface.Cells[startSet2 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                basicSurface.Cells[startSet2 + i].Background = StarterProject.Theme.OrangeDark;

                basicSurface.Cells[startSet3 + i].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally | Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                basicSurface.Cells[startSet3 + i].Background = StarterProject.Theme.GreenDark;
            }
        }

        private void MakeLayeredSurface()
        {
            var editor = new SadConsole.Surfaces.SurfaceEditor(layeredSurface);

            layeredSurface.SetActiveLayer(2);
            editor.Fill(StarterProject.Theme.Brown, StarterProject.Theme.BrownDark, 0);
            editor.Fill(new Rectangle(5, 2, 34 - 10, 15 - 4), Color.White, Color.Transparent, 0);
            editor.Print(14, 0, "Layer 2");

            layeredSurface.SetActiveLayer(1);
            editor.Fill(StarterProject.Theme.Green, StarterProject.Theme.GreenDark, 0);
            editor.Fill(new Rectangle(10, 4, 34 - 20, 15 - 8), Color.White, Color.Transparent, 0);
            editor.Print(14, 2, "Layer 1");

            layeredSurface.SetActiveLayer(0);
            editor.Fill(StarterProject.Theme.Gray, StarterProject.Theme.GrayDark, 0);
            editor.Print(14, 4, "Layer 0");
        }
    }
    static class StringHelpers
    {
        public static string Repeat(this string value, int times)
        {
            StringBuilder builder = new StringBuilder(value.Length * times);
            for (int i = 0; i != times; i++)
            {
                builder.Append(value);
            }
            return builder.ToString();
        }
    }


}

