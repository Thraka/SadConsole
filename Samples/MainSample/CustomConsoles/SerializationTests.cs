using System;
using System.Text;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace FeatureDemo.CustomConsoles
{
    internal class SerializationTests : Console
    {
        private readonly SadConsole.UI.ControlsConsole _controlsConsole;
        private readonly Console masterView;
        private readonly SadConsole.Console loadedView;

        // Controls
        private readonly RadioButton optionButtonSurface;
        private readonly RadioButton optionButtonView;
        private readonly RadioButton optionButtonLayered;
        private readonly RadioButton optionButtonAnimated;
        private readonly Console basicSurface;
        private readonly AnimatedScreenSurface animatedSurface;
        private readonly Console viewSurface;
        private LayeredScreenSurface layeredSurface;
        private readonly Console emptySurface;

        public SerializationTests() : base(80, 23)
        {
            //Settings.SerializationIsCompressed = true;
            _controlsConsole = new ControlsConsole(80, 4);

            masterView = new Console(34, 15);
            loadedView = new Console(34, 15);

            masterView.Fill(Color.White, Color.Red, 0);
            loadedView.Fill(Color.White, Color.Blue, 0);

            UseMouse = true;

            // Add the consoles to the list.
            Children.Add(_controlsConsole);
            Children.Add(masterView);
            Children.Add(loadedView);

            // Setup main view
            masterView.Position = new Point(3, 6);

            // Setup sub view
            loadedView.Position = new Point(80 - 37, 6);


            // Setup controls
            _controlsConsole.Position = new Point(0, 0);

            optionButtonSurface = new SadConsole.UI.Controls.RadioButton(18, 1)
            {
                Text = "Surface",
                Position = new Point(1, 1),
            };
            optionButtonSurface.IsSelectedChanged += OptionButton_IsSelectedChanged;
            _controlsConsole.ControlHostComponent.Add(optionButtonSurface);

            optionButtonView = new SadConsole.UI.Controls.RadioButton(18, 1)
            {
                Text = "Surface View",
                Position = new Point(1, 2)
            };
            optionButtonView.IsSelectedChanged += OptionButton_IsSelectedChanged;
            _controlsConsole.ControlHostComponent.Add(optionButtonView);

            optionButtonLayered = new SadConsole.UI.Controls.RadioButton(21, 1)
            {
                Text = "Layered Surface",
                Position = new Point(optionButtonSurface.Bounds.MaxExtentX + 1, 1)
            };
            optionButtonLayered.IsSelectedChanged += OptionButton_IsSelectedChanged;
            _controlsConsole.ControlHostComponent.Add(optionButtonLayered);

            optionButtonAnimated = new SadConsole.UI.Controls.RadioButton(21, 1)
            {
                Text = "Animated Surface",
                Position = new Point(optionButtonSurface.Bounds.MaxExtentX + 1, 2)
            };
            optionButtonAnimated.IsSelectedChanged += OptionButton_IsSelectedChanged;
            _controlsConsole.ControlHostComponent.Add(optionButtonAnimated);

            var buttonSave = new SadConsole.UI.Controls.Button(17, 1)
            {
                Text = "Save and Load",
                Position = new Point(_controlsConsole.Width - 19, 1)
            };
            buttonSave.Click += ButtonSave_Click;
            _controlsConsole.ControlHostComponent.Add(buttonSave);

            basicSurface = new SadConsole.Console(34, 15);

            animatedSurface = SadConsole.AnimatedScreenSurface.CreateStatic(34, 15, 15, 0.3d);
            viewSurface = new Console(1, 1);
            viewSurface.SetSurface(basicSurface, new Rectangle(5, 2, 34 - 10, 15 - 4));
            //emptySurface = (SadConsole.Surfaces.BasicSurface)loadedView.TextSurface;

            MakeBasicSurface();
            MakeLayeredSurface();

            optionButtonSurface.IsSelected = true;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            loadedView.Children.Clear();

            if (optionButtonSurface.IsSelected)
            {
                SadConsole.Serializer.Save<Console>(basicSurface, "basicsurface.surface", false);
                loadedView.Children.Add(SadConsole.Serializer.Load<Console>("basicsurface.surface", false));
            }
            else if (optionButtonView.IsSelected)
            {
                SadConsole.Serializer.Save<Console>(viewSurface, "viewsurface.view", false);
                var loaded = SadConsole.Serializer.Load<Console>("viewsurface.view", false);
                loaded.SetSurface(basicSurface, new Rectangle(5, 2, 34 - 10, 15 - 4));
                basicSurface.IsDirty = true;
                viewSurface.IsDirty = true;
                loadedView.Children.Add(loaded);
                loaded.Fill(new Rectangle(1, 1, loaded.Width - 2, 3), Color.White, Color.DarkBlue, 0, Mirror.None);
                loaded.Print(2, 2, "Loaded view");
                //loadedView.Children.Add(SadConsole.Surfaces.Basic.Load("viewsurface.view", basicSurface);
            }
            else if (optionButtonLayered.IsSelected)
            {
                SadConsole.Serializer.Save(layeredSurface, "layeredObject.layered", false);
                var layers = SadConsole.Serializer.Load<LayeredScreenSurface>("layeredObject.layered", false);
                loadedView.Children.Add(layers);
            }
            else if (optionButtonAnimated.IsSelected)
            {
                SadConsole.Serializer.Save(animatedSurface, "animatedsurface.animation", false);
                var animation = SadConsole.Serializer.Load<AnimatedScreenSurface>("animatedsurface.animation", false);
                animation.Start();
                loadedView.Children.Add(animation);
            }
        }

        private void OptionButton_IsSelectedChanged(object sender, EventArgs e)
        {
            loadedView.Children.Clear();
            masterView.Children.Clear();

            if (optionButtonSurface.IsSelected)
            {
                masterView.Children.Add(basicSurface);
            }
            else if (optionButtonView.IsSelected)
            {
                masterView.Children.Add(viewSurface);
            }
            else if (optionButtonLayered.IsSelected)
            {
                masterView.Children.Add(layeredSurface);
            }
            else if (optionButtonAnimated.IsSelected)
            {
                masterView.Children.Add(animatedSurface);
            }
        }

        public override void Update(TimeSpan delta)
        {
            if (optionButtonAnimated.IsSelected)
            {
                //animatedSurface.Update(delta);
                //(loadedView as SadConsole.Surfaces.Animated)?.Update(delta);

            }

            base.Update(delta);
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            // Process mouse for each console
            var childState = new MouseScreenObjectState(_controlsConsole, state.Mouse);

            if (childState.IsOnScreenObject)
            {
                return _controlsConsole.ProcessMouse(childState);
            }

            return false;
        }

        public override bool ProcessKeyboard(Keyboard info) => _controlsConsole.ProcessKeyboard(info);


        private void MakeBasicSurface()
        {
            basicSurface.Print(0, 0, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".Repeat(9));
            basicSurface.SetGlyph(0, 0, 7);
            basicSurface.SetGlyph(1, 0, 8);
            basicSurface.SetGlyph(2, 0, 9);
            basicSurface.SetGlyph(3, 0, 10);
            ColorGradient gradient = new ColorGradient(SadConsole.UI.Themes.Library.Default.Colors.Blue, SadConsole.UI.Themes.Library.Default.Colors.Yellow);
            for (int i = 0; i < 510; i += 10)
            {
                Point point = Point.FromIndex(i, basicSurface.Width);
                basicSurface.Print(point.X, point.Y, gradient.ToColoredString(basicSurface.GetString(i, 10)));
            }

            // Mirror 1
            int startSet1 = new Point(0, 3).ToIndex(34);
            int startSet2 = new Point(0, 6).ToIndex(34);
            int startSet3 = new Point(0, 9).ToIndex(34);

            for (int i = 0; i < 34 * 3; i++)
            {
                basicSurface.Cells[startSet1 + i].Mirror = Mirror.Vertical;
                basicSurface.Cells[startSet1 + i].Background = SadConsole.UI.Themes.Library.Default.Colors.PurpleDark;

                basicSurface.Cells[startSet2 + i].Mirror = Mirror.Horizontal;
                basicSurface.Cells[startSet2 + i].Background = SadConsole.UI.Themes.Library.Default.Colors.OrangeDark;

                basicSurface.Cells[startSet3 + i].Mirror = Mirror.Horizontal | Mirror.Vertical;
                basicSurface.Cells[startSet3 + i].Background = SadConsole.UI.Themes.Library.Default.Colors.GreenDark;
            }
        }

        private void MakeLayeredSurface()
        {
            int width = 34;
            int height = 15;

            var layers = new List<LayeredScreenSurface.Layer>();

            var layer = new LayeredScreenSurface.Layer("1", width, height);
            layer.Surface.Fill(SadConsole.UI.Themes.Library.Default.Colors.Gray, SadConsole.UI.Themes.Library.Default.Colors.GrayDark, 0);
            layer.Surface.Print(14, 4, "Layer 0");
            layers.Add(layer);

            layer = new LayeredScreenSurface.Layer("2", width, height);
            layer.Surface.Fill(SadConsole.UI.Themes.Library.Default.Colors.Green, SadConsole.UI.Themes.Library.Default.Colors.GreenDark, 0);
            layer.Surface.Fill(new Rectangle(10, 4, 34 - 20, 15 - 8), Color.White, Color.Transparent, 0);
            layer.Surface.Print(14, 2, "Layer 1");
            layers.Add(layer);

            layer = new LayeredScreenSurface.Layer("3", width, height);
            layer.Surface.Fill(SadConsole.UI.Themes.Library.Default.Colors.Brown, SadConsole.UI.Themes.Library.Default.Colors.BrownDark, 0);
            layer.Surface.Fill(new Rectangle(5, 2, 34 - 10, 15 - 4), Color.White, Color.Transparent, 0);
            layer.Surface.Print(14, 0, "Layer 2");
            layers.Add(layer);

            layer = new LayeredScreenSurface.Layer("oob", width + 20, height + 20);
            layer.Surface.Fill(SadConsole.UI.Themes.Library.Default.Colors.Purple, SadConsole.UI.Themes.Library.Default.Colors.PurpleDark, 0);
            layer.Surface.Fill(new Rectangle(1, 1, width, height), Color.White, Color.Transparent, 0);
            layer.Surface.Print(14, 0, "Layer OOB");
            layer.Position = (-1, -1);
            layers.Add(layer);

            layeredSurface = new LayeredScreenSurface(layers, true);
        }
    }

    internal static class StringHelpers
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

