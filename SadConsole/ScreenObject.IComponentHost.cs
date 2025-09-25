using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using SadConsole.Components;

namespace SadConsole;

public partial class ScreenObject : IScreenObject
{
    /// <summary>
    /// A filtered list from <see cref="IComponentHost.SadComponents"/> where <see cref="IComponent.IsUpdate"/> is <see langword="true"/>.
    /// </summary>
    protected List<IComponent> ComponentsUpdate;

    /// <summary>
    /// A filtered list from <see cref="IComponentHost.SadComponents"/> where <see cref="IComponent.IsRender"/> is <see langword="true"/>.
    /// </summary>
    protected List<IComponent> ComponentsRender;

    /// <summary>
    /// A filtered list from <see cref="IComponentHost.SadComponents"/> where <see cref="IComponent.IsMouse"/> is <see langword="true"/>.
    /// </summary>
    protected List<IComponent> ComponentsMouse;

    /// <summary>
    /// A filtered list from <see cref="IComponentHost.SadComponents"/> where <see cref="IComponent.IsKeyboard"/> is <see langword="true"/>.
    /// </summary>
    protected List<IComponent> ComponentsKeyboard;

    /// <summary>
    /// A filtered list from <see cref="IComponentHost.SadComponents"/> that is not set for update, render, mouse, or keyboard.
    /// </summary>
    protected List<IComponent> ComponentsEmpty;

    /// <inheritdoc/>
    public ObservableCollection<IComponent> SadComponents { get; protected set; }

    /// <inheritdoc/>
    public IEnumerable<TComponent> GetSadComponents<TComponent>()
        where TComponent : class, IComponent
    {
        foreach (IComponent component in SadComponents)
        {
            if (component is TComponent)
                yield return (TComponent)component;
        }
    }

    /// <inheritdoc/>
    public TComponent? GetSadComponent<TComponent>()
        where TComponent : class, IComponent
    {
        foreach (IComponent component in SadComponents)
        {
            if (component is TComponent)
                return (TComponent)component;
        }

        return null;
    }

    /// <inheritdoc/>
    public bool HasSadComponent<TComponent>([NotNullWhen(true)] out TComponent? component)
        where TComponent : class, IComponent =>

        (component = GetSadComponent<TComponent>()) == null ? false : true;

    private void Components_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                    foreach (object item in e.NewItems)
                    {
                        Components_FilterAddItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnAdded(this);
                        OnSadComponentAdded((IComponent)item);
                    }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                    foreach (object item in e.OldItems)
                    {
                        Components_FilterRemoveItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnRemoved(this);
                        OnSadComponentRemoved((IComponent)item);
                    }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems != null)
                    foreach (object item in e.NewItems)
                    {
                        Components_FilterAddItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnAdded(this);
                        OnSadComponentAdded((IComponent)item);
                    }
                if (e.OldItems != null)
                    foreach (object item in e.OldItems)
                    {
                        Components_FilterRemoveItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnRemoved(this);
                        OnSadComponentRemoved((IComponent)item);
                    }
                break;
            case NotifyCollectionChangedAction.Move:
                SortComponents();
                break;
            case NotifyCollectionChangedAction.Reset:
                List<IComponent> items = new(ComponentsRender.Count + ComponentsUpdate.Count + ComponentsKeyboard.Count + ComponentsMouse.Count);

                while (ComponentsRender.Count != 0)
                {
                    ComponentsRender[0].OnRemoved(this);

                    if (!items.Contains(ComponentsRender[0]))
                        items.Add(ComponentsRender[0]);

                    Components_FilterRemoveItem(ComponentsRender[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                }
                while (ComponentsUpdate.Count != 0)
                {
                    ComponentsUpdate[0].OnRemoved(this);

                    if (!items.Contains(ComponentsUpdate[0]))
                        items.Add(ComponentsUpdate[0]);

                    Components_FilterRemoveItem(ComponentsUpdate[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                }
                while (ComponentsKeyboard.Count != 0)
                {
                    ComponentsKeyboard[0].OnRemoved(this);

                    if (!items.Contains(ComponentsKeyboard[0]))
                        items.Add(ComponentsKeyboard[0]);

                    Components_FilterRemoveItem(ComponentsKeyboard[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                }
                while (ComponentsMouse.Count != 0)
                {
                    ComponentsMouse[0].OnRemoved(this);

                    if (!items.Contains(ComponentsMouse[0]))
                        items.Add(ComponentsMouse[0]);

                    Components_FilterRemoveItem(ComponentsMouse[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                }
                while (ComponentsEmpty.Count != 0)
                {
                    ComponentsEmpty[0].OnRemoved(this);

                    if (!items.Contains(ComponentsEmpty[0]))
                        items.Add(ComponentsEmpty[0]);

                    Components_FilterRemoveItem(ComponentsEmpty[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                }

                foreach (IComponent item in items)
                    OnSadComponentRemoved(item);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Called when a component is added to the <see cref="IComponentHost.SadComponents"/> collection.
    /// </summary>
    /// <param name="component">The component added.</param>
    protected virtual void OnSadComponentAdded(IComponent component) { }

    /// <summary>
    /// Called when a component is removed from the <see cref="IComponentHost.SadComponents"/> collection.
    /// </summary>
    /// <param name="component">The component removed.</param>
    protected virtual void OnSadComponentRemoved(IComponent component) { }

    /// <summary>
    /// Sorts the components based on the <see cref="IComponent.SortOrder"/> value.
    /// </summary>
    public void SortComponents()
    {
        ComponentsRender.Sort(IComponentHost.CompareComponent);
        ComponentsUpdate.Sort(IComponentHost.CompareComponent);
        ComponentsKeyboard.Sort(IComponentHost.CompareComponent);
        ComponentsMouse.Sort(IComponentHost.CompareComponent);
        ComponentsEmpty.Sort(IComponentHost.CompareComponent);
    }

    /// <summary>
    /// Adds a component to the provided collections, based on its configuration.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <param name="componentsRender">The render collection.</param>
    /// <param name="componentsUpdate">The update collection.</param>
    /// <param name="componentsKeyboard">The keyboard collection.</param>
    /// <param name="componentsMouse">The mouse collection.</param>
    /// <param name="componentsEmpty">The empty collection.</param>
    public static void Components_FilterAddItem(IComponent component,
                                                List<IComponent> componentsRender,
                                                List<IComponent> componentsUpdate,
                                                List<IComponent> componentsKeyboard,
                                                List<IComponent> componentsMouse,
                                                List<IComponent> componentsEmpty)
    {
        if (component.IsRender)
            if (!componentsRender.Contains(component))
                componentsRender.Add(component);

        if (component.IsUpdate)
            if (!componentsUpdate.Contains(component))
                componentsUpdate.Add(component);

        if (component.IsKeyboard)
            if (!componentsKeyboard.Contains(component))
                componentsKeyboard.Add(component);

        if (component.IsMouse)
            if (!componentsMouse.Contains(component))
                componentsMouse.Add(component);

        if (!component.IsRender && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
            if (!componentsEmpty.Contains(component))
                componentsEmpty.Add(component);

        componentsRender.Sort(IComponentHost.CompareComponent);
        componentsUpdate.Sort(IComponentHost.CompareComponent);
        componentsKeyboard.Sort(IComponentHost.CompareComponent);
        componentsMouse.Sort(IComponentHost.CompareComponent);
        componentsEmpty.Sort(IComponentHost.CompareComponent);
    }

    /// <summary>
    /// Removes a component to the provided collections, based on its configuration.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <param name="componentsRender">The render collection.</param>
    /// <param name="componentsUpdate">The update collection.</param>
    /// <param name="componentsKeyboard">The keyboard collection.</param>
    /// <param name="componentsMouse">The mouse collection.</param>
    /// <param name="componentsEmpty">The empty collection.</param>
    public static void Components_FilterRemoveItem(IComponent component,
                                                    List<IComponent> componentsRender,
                                                    List<IComponent> componentsUpdate,
                                                    List<IComponent> componentsKeyboard,
                                                    List<IComponent> componentsMouse,
                                                    List<IComponent> componentsEmpty)
    {
        if (component.IsRender)
            if (componentsRender.Contains(component))
                componentsRender.Remove(component);

        if (component.IsUpdate)
            if (componentsUpdate.Contains(component))
                componentsUpdate.Remove(component);

        if (component.IsKeyboard)
            if (componentsKeyboard.Contains(component))
                componentsKeyboard.Remove(component);

        if (component.IsMouse)
            if (componentsMouse.Contains(component))
                componentsMouse.Remove(component);

        if (!component.IsRender && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
            if (!componentsEmpty.Contains(component))
                componentsEmpty.Remove(component);

        componentsRender.Sort(IComponentHost.CompareComponent);
        componentsUpdate.Sort(IComponentHost.CompareComponent);
        componentsKeyboard.Sort(IComponentHost.CompareComponent);
        componentsMouse.Sort(IComponentHost.CompareComponent);
        componentsEmpty.Sort(IComponentHost.CompareComponent);
    }

    /// <summary>
    /// Helper to sort the components in the split collections.
    /// </summary>
    /// <param name="componentsRender">The render collection.</param>
    /// <param name="componentsUpdate">The update collection.</param>
    /// <param name="componentsKeyboard">The keyboard collection.</param>
    /// <param name="componentsMouse">The mouse collection.</param>
    /// <param name="componentsEmpty">The empty collection.</param>
    public static void Components_Sort(List<IComponent> componentsRender,
                                        List<IComponent> componentsUpdate,
                                        List<IComponent> componentsKeyboard,
                                        List<IComponent> componentsMouse,
                                        List<IComponent> componentsEmpty)
    {
        componentsRender.Sort(IComponentHost.CompareComponent);
        componentsUpdate.Sort(IComponentHost.CompareComponent);
        componentsKeyboard.Sort(IComponentHost.CompareComponent);
        componentsMouse.Sort(IComponentHost.CompareComponent);
        componentsEmpty.Sort(IComponentHost.CompareComponent);
    }
}
