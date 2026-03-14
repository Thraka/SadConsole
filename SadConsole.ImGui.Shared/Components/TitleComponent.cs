using System;
using SadConsole.ImGuiSystem;
using SadConsole.Input;

namespace SadConsole.Components
{

    /// <summary>
    /// A component that holds a title for a screen object.
    /// </summary>
    public class TitleComponent : IComponent, ITitle
    {
        /// <summary>
        /// The title of the screen object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Creates a new instance of this component.
        /// </summary>
        /// <param name="title">The title of the screen object.</param>
        public TitleComponent(string title) =>
            Title = title;

        #region IComponent
        uint IComponent.SortOrder => 0;

        bool IComponent.IsUpdate => false;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => false;

        bool IComponent.IsKeyboard => false;

        void IComponent.OnAdded(IScreenObject host)
        {

        }

        void IComponent.OnRemoved(IScreenObject host)
        {
        }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = false;
        }

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;
        }

        void IComponent.Render(IScreenObject host, TimeSpan delta)
        {
        }

        void IComponent.Update(IScreenObject host, TimeSpan delta)
        {
        }
        #endregion
    }
}

namespace SadConsole
{
    /// <summary>
    /// Extensions for working with the <see cref="Components.TitleComponent"/> component.
    /// </summary>
    public static partial class ScreenObjectExtensionsDebug
    {
        /// <summary>
        /// Gets the title component of the screen object.
        /// </summary>
        /// <param name="screenObject">The screen object to get the title from.</param>
        /// <returns>The title component if found; otherwise, <c>null</c>.</returns>
        public static string? GetTitle(this IScreenObject screenObject) =>
            screenObject.GetSadComponent<Components.TitleComponent>()?.Title;

        /// <summary>
        /// Sets the title of the screen object, adding a title component if one does not already exist.
        /// </summary>
        /// <param name="screenObject">The screen object to set the title for.</param>
        /// <param name="title">The title.</param>
        public static void SetTitle(this IScreenObject screenObject, string title)
        {
            Components.TitleComponent? titleComponent = screenObject.GetSadComponent<Components.TitleComponent>();
            if (titleComponent is null)
            {
                titleComponent = new Components.TitleComponent(title);
                screenObject.SadComponents.Add(titleComponent);
            }
            else
            {
                titleComponent.Title = title;
            }
        }

        /// <summary>
        /// Removes the title component from the screen object if it exists.
        /// </summary>
        /// <param name="screenObject">The screen object to remove the title from.</param>
        public static void RemoveTitle(this IScreenObject screenObject)
        {
            Components.TitleComponent? titleComponent = screenObject.GetSadComponent<Components.TitleComponent>();

            if (titleComponent is not null)
                screenObject.SadComponents.Remove(titleComponent);
        }
    }
}
