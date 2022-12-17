using System;
using System.Collections.Generic;

namespace Firelight.Events
{
    /// <summary>
    /// A custom <see cref="IEventInvoker"/> that is responsible for dispatching events for the <see cref="EventService"/>
    /// </summary>
    public class EventInvoker<T> : IEventInvoker where T : class, IEvent
    {
        private readonly T _eventObject;
        public IEvent EventObject => _eventObject;
        
        public object Instance { get; }

        public EventInvoker(T eventObject, object reference)
        {
            _eventObject = eventObject;
            Instance = reference;
        }
        
        public void Fire(Dictionary<Type, Delegate> delegates)
        {
            var dispatchAs = EventObject.GetType();
            if (!delegates.TryGetValue(dispatchAs, out var dispatches) || dispatches == null)
            {
                return;
            }

            try
            {
                (dispatches as EventHandler<T>)?.Invoke(_eventObject);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public void Fire(Dictionary<Type, Dictionary<object, Delegate>> delegates)
        {
            if (!delegates.TryGetValue(EventObject.GetType(), out var objectPairs) ||
                !objectPairs.TryGetValue(Instance, out var dispatches) || dispatches == null)
            {
                return;
            }
                
            try
            {
                (dispatches as EventHandler<T>)?.Invoke(_eventObject);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}