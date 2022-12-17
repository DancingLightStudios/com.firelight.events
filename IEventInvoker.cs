using System;
using System.Collections.Generic;

namespace Firelight.Events
{
    /// <summary>
    /// The base interface for the event invoker for the <see cref="EventService"/>. A custom implementation can be used
    /// if needed. However, the default implementation, <see cref="EventInvoker{T}"/>, should handle most use cases.
    /// </summary>
    public interface IEventInvoker
    {
        /// <summary>
        /// The object that this event is bound to. If this is a global event, then this field should be null.
        /// </summary>
        object Instance { get; }
        
        /// <summary>
        /// The instance of the registered event
        /// </summary>
        IEvent EventObject { get; }

        /// <summary>
        /// Dispatch the event globally
        /// </summary>
        void Fire(Dictionary<Type, Delegate> delegates);
        
        /// <summary>
        /// Dispatch the event with all registered objects
        /// </summary>
        void Fire(Dictionary<Type, Dictionary<object, Delegate>> delegates);
    }
}