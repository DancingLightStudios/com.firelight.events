using System.Collections.Generic;
using UnityEditor;

namespace Firelight.Events.Editor
{
    public static class EventRecorder
    {
        private const string EditorPrefIsRecording = "events.recorder.recording";
        private const string EditorPrefClearOnPlay = "events.recorder.clearOnPlay";
        
        public static List<RecordedEvent> EventHistory { get; }= new();

        public static bool IsRecording
        {
            get => EditorPrefs.GetBool(EditorPrefIsRecording, false);
            set => EditorPrefs.SetBool(EditorPrefIsRecording, value);
        }

        public static bool ClearOnPlay
        {
            get => EditorPrefs.GetBool(EditorPrefClearOnPlay, false);
            set => EditorPrefs.SetBool(EditorPrefClearOnPlay, value);
        }

        [InitializeOnLoadMethod]
        private static void BindEvent()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            EventService.OnEventTriggered += OnEventTriggered;
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredPlayMode && ClearOnPlay)
            {
                Clear();
            }
        }

        private static void OnEventTriggered(IEvent newEvent)
        {
            if (!IsRecording)
            {
                return;
            }
            
            EventHistory.Add(new RecordedEvent(newEvent));
        }

        public static void Clear()
        {
            EventHistory.Clear();
        }
    }
}