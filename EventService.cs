using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Firelight.Events
{
    public delegate void EventHandler<in T>(T eventTrigger) where T : class, IEvent;
    
    /// <summary>
    /// The main class for all event handling. This class is responsible for all event coordination. 
    /// </summary>
    public static class EventService
    {
        /// <summary>
        /// A hook for mostly editor systems that provides a callback whenever an event is invoked by the system. DO NOT
        /// use this to subscribe to events. This is purely a debugging hook.
        /// </summary>
        public static Action<IEvent> OnEventTriggered;
        
        /// <summary>
        /// The collection of registered events that are not bound to an object
        /// </summary>
        private static readonly Dictionary<Type, Delegate> GlobalDelegates = new();
        
        /// <summary>
        /// The collection of registered events that are bound to an object
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<object, Delegate>> InstancedDelegates = new();

        /// <summary>
        /// Removes all registered events from the global and instanced event lookups
        /// </summary>
        public static void ClearAllEvents()
        {
            GlobalDelegates.Clear();
            InstancedDelegates.Clear();
        }

        /// <summary>
        /// Allows binding of callbacks to events. Each binding is responsible for managing itself; all events need to
        /// be unbound explicitly. If the event is only needed once, <see cref="SubscribeOnce{T}"/> might be a better option.
        /// This method returns an <see cref="Action"/> that can be invoked to unsubscribe the event automatically. This
        /// is the recommended method since it minimizes the possibility for a failed unsubscription.
        /// </summary>
        public static Action Subscribe<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            var key = typeof(T);
            if (instance != null)
            {
                AddInstanced(key, listener, instance);
            }
            else
            {
                AddGlobal(key, listener);
            }

            return () =>
            {
                Unsubscribe(listener, instance);
            };
        }
        
        /// <summary>
        /// Functions like <see cref="Subscribe{T}"/> but unbinds the delegate after the event is invoked.
        /// </summary>
        public static void SubscribeOnce<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            void ListenerWrapper(T value)
            {
                Unsubscribe<T>(ListenerWrapper);
                listener(value);
            }
            
            Subscribe<T>(ListenerWrapper, instance);
        }

        /// <summary>
        /// The method used to manually unsubscribe from an event stream. This can be useful when subscribing/unsubscribing
        /// blocks of events at a time. However, in most cases, it is better to use the <see cref="Action"/> returned by
        /// the <see cref="Subscribe{T}"/> event to handle unsubscription.
        /// </summary>
        public static void Unsubscribe<T>(EventHandler<T> listener, object instance = null) where T : class, IEvent
        {
            var key = typeof(T);
            if (instance != null)
            {
                RemoveInstanced(key, listener, instance);
            }
            else
            {
                RemoveGlobal(key, listener);
            }
        }

        /// <summary>
        /// Invokes the given <see cref="IEvent"/> implementation as the given event type
        /// </summary>
        public static void Trigger<T>(T eventTrigger, object instance = null) where T : class, IEvent
        {
            FireEvent(new EventInvoker<T>(eventTrigger, instance));
        }

        private static void FireEvent(IEventInvoker eventInvoker)
        {
            LogEvent(eventInvoker);
            eventInvoker.Fire(GlobalDelegates);
            if (eventInvoker.Instance != null)
            {
                eventInvoker.Fire(InstancedDelegates);
            }
        }

        #region GLOBAL DELEGATES
        private static void AddGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
        {
            if (GlobalDelegates.ContainsKey(key))
            {
                GlobalDelegates[key] = (GlobalDelegates[key] as EventHandler<T>) + listener;
            }
            else
            {
                GlobalDelegates.Add(key, listener);
            }
        }

        private static void RemoveGlobal<T>(Type key, EventHandler<T> listener) where T : class, IEvent
        {
            if (GlobalDelegates.ContainsKey(key))
            {
                GlobalDelegates[key] = (GlobalDelegates[key] as EventHandler<T>) - listener;
            }
        }
        #endregion

        #region INSTANCED DELEGATES
        private static void AddInstanced<T>(Type key, EventHandler<T> listener, object instance) where T : class, IEvent
        {
            if (!InstancedDelegates.ContainsKey(key))
            {
                InstancedDelegates.Add(key, new Dictionary<object, Delegate>());
            }

            if (InstancedDelegates[key].ContainsKey(instance))
            {
                InstancedDelegates[key][instance] = (InstancedDelegates[key][instance] as EventHandler<T>) + listener;
            }
            else
            {
                InstancedDelegates[key].Add(instance, listener);
            }
        }

        private static void RemoveInstanced<T>(Type key, EventHandler<T> listener, object instance) where T : class, IEvent
        {
            if (InstancedDelegates.ContainsKey(key) && InstancedDelegates[key].ContainsKey(instance))
            {
                InstancedDelegates[key][instance] = (InstancedDelegates[key][instance] as EventHandler<T>) - listener;
            }
        }
        #endregion

        /// <summary>
        /// A conditional method that is used to invoke the <see cref="OnEventTriggered"/> debug hook. This is called before
        /// the method is invoked to allow for logging to be done before the event is dispatched.
        /// </summary>
        [Conditional("DEBUG")]
        private static void LogEvent(IEventInvoker eventInvoker)
        {
            OnEventTriggered?.Invoke(eventInvoker.EventObject);
        }
    }
}