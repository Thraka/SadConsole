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
    class GameObject: Entity, GoRogue.IHasComponents, SadConsole.Factory.IFactoryObject
    {
        private GoRogue.ComponentContainer _components = new ComponentContainer();
        private GoRogue.Messaging.MessageBus _messages = new GoRogue.Messaging.MessageBus();

        public string DefinitionId { get; set; }

        /// <summary>
        /// <see langword="true"/> when the object is still on the map.
        /// </summary>
        public bool IsAlive { get; set; }

        public GameObject() : this(Color.White, Color.Black, 0) { }
        public GameObject(Color foreground, Color background, int glyph) : base(foreground, background, glyph, 0)
        {
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

        #region GoRogue Messages
        public void RegisterSubscriber<TMessage>(GoRogue.Messaging.ISubscriber<TMessage> instance) =>
            _messages.RegisterSubscriber(instance);

        public void UnregisterSubscriber<TMessage>(GoRogue.Messaging.ISubscriber<TMessage> instance) =>
            _messages.UnregisterSubscriber(instance);

        public void SendMessage<TMessage>(TMessage message) =>
            _messages.Send(message);
        #endregion
    }
}
