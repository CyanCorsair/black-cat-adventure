using System;
using System.Collections.Generic;
using BlackCatAdventure.Interfaces;

namespace BlackCatAdventure.Services;

public class BasicEventBus: IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<T>(T @event) where T : class
    {
        var eventType = typeof(T);
        if (_handlers.ContainsKey(eventType))
        {
            foreach (var handler in _handlers[eventType])
            {
                ((Action<T>)handler)(@event);
            }
        }
    }

    public void Subscribe<T>(Action<T> handler) where T : class
    {
        var eventType = typeof(T);
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Delegate>();
        }
        _handlers[eventType].Add(handler);
    }
}