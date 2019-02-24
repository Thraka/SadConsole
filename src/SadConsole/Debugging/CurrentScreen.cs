#if XNA
using Microsoft.Xna.Framework;
#endif

using SadConsole.Controls;
using System.Reflection;
using System.Linq;

namespace SadConsole.Debug
{
    public static class CurrentScreen
    {
        private class DebugWindow : Window
        {
            public DebugWindow() : base (80, 23)
            {
                Title = "Global.CurrentScreen";
                IsModalDefault = true;
                CloseOnEscKey = true;

                ListBox listbox = new ListBox(20, 15) { Position = new Point(2, 2) };
                Add(listbox);

                Console screenObject = Global.CurrentScreen;

                //foreach (var child in screenObject.Children)
                //    AddConsoleToList(child);
                AddConsoleToList(screenObject);
                void AddConsoleToList(Console console, int indent = 0)
                {
                    var debugger = console.GetType().GetTypeInfo().GetCustomAttributes<System.Diagnostics.DebuggerDisplayAttribute>().FirstOrDefault();
                    var text = debugger != null ? debugger.Value : console.ToString();

                    listbox.Items.Add(new string('-', indent) + text);

                    foreach (var child in console.Children)
                        AddConsoleToList(child, indent + 1);
                }
            }

            public override string ToString()
            {
                return "Debug Window";
            }
        }
        
        public static void Show()
        {
            DebugWindow window = new DebugWindow();
            window.Show();
        }

        //private class DebugSurface : Console
        //{
        //    public override void Draw(TimeSpan timeElapsed)
        //    {
        //        base.Draw(timeElapsed);
        //    }
        //}

        //public static void Show()
        //{

        //}
    }
}
