using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Entities;
using SadConsole;
using SadRogue.Primitives;
using GoRogue;
using Game.Screens;

namespace Game
{
    class GameObject: Entity, GoRogue.IHasComponents
    {
        private GoRogue.ComponentContainer _components = new ComponentContainer();

        public GameObject() : this(Color.White, Color.Black, 0) { }
        public GameObject(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
            Animation.CurrentFrame.SetGlyph(0, 0, glyph, foreground, background);
        }

        public void ProcessTickComponents(WorldPlay world)
        {
            foreach (var item in _components.GetComponents<ObjectComponents.ITick>())
            {
                item.Action(world.GameBoard, this);
            }
        }

        #region GoRogue Components
        public void AddComponent(object component)
        {
            if (!(component is ObjectComponents.IGameObjectComponent)) throw new Exception($"Must add a {nameof(ObjectComponents.IGameObjectComponent)} to this object");
            ((IHasComponents)_components).AddComponent(component);

            ((ObjectComponents.IGameObjectComponent)component).Added(this);
        }

        public T GetComponent<T>()
        {
            return ((IHasComponents)_components).GetComponent<T>();
        }

        public IEnumerable<T> GetComponents<T>()
        {
            return ((IHasComponents)_components).GetComponents<T>();
        }

        public bool HasComponent(Type componentType)
        {
            return ((IHasComponents)_components).HasComponent(componentType);
        }

        public bool HasComponent<T>()
        {
            return ((IHasComponents)_components).HasComponent<T>();
        }

        public bool HasComponents(params Type[] componentTypes)
        {
            return ((IHasComponents)_components).HasComponents(componentTypes);
        }

        public void RemoveComponent(object component)
        {
            ((IHasComponents)_components).RemoveComponent(component);
            ((ObjectComponents.IGameObjectComponent)component).Removed(this);
        }

        public void RemoveComponents(params object[] components)
        {
            ((IHasComponents)_components).RemoveComponents(components);

            foreach (var item in components)
                ((ObjectComponents.IGameObjectComponent)item).Removed(this);
        }
        #endregion
    }
}
