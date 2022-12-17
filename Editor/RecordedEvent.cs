using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Firelight.Events.Editor
{
    [Serializable]
    public class RecordedEvent
    {
        public Type type;
        public float time;
        public string timeString;
        public List<(FieldInfo, object)> fields = new();
        
        private RecordedEvent() {}

        public RecordedEvent(IEvent targetEvent)
        {
            type = targetEvent.GetType();
            time = Time.time;
            timeString = $"{time.ToString("F")}s";
            var fieldInfos = type.GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                fields.Add((fieldInfo, fieldInfo.GetValue(targetEvent)));
            }
        }
    }
}