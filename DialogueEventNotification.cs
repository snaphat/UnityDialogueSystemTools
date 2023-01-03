using System;
using System.ComponentModel;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace DialogueSystemTools
{

    [CustomStyle("DialogueMarkerStyle")]
    [Serializable, DisplayName("Dialogue Marker")]
    public class DialogueEventNotification : Marker, INotification
    {
        public string conversationGuid = "";
        public string dialogueEntryGuid = "";
        public Transform conversant;

        public Conversation conversation;
        public DialogueEntry dialogueEntry;

        public void OnEnable()
        {
            conversation = Utility.FindConversation(DialogueManager.instance.initialDatabase.conversations, conversationGuid);
            dialogueEntry = Utility.FindDialogueEntry(conversation, dialogueEntryGuid);
        }

        public override void OnInitialize(TrackAsset asset)
        {
            Debug.Log("init");
        }

        PropertyName INotification.id { get { return new PropertyName(); } }
    }

#if UNITY_EDITOR
    // Custom Inspector for creating Event Marker editor
    [CustomEditor(typeof(DialogueEventNotification)), CanEditMultipleObjects]
    public class DialogueEventNotificationEditor : Editor
    {
        SerializedProperty m_Time;
        SerializedProperty m_conversant;
        SerializedProperty m_conversationGuid;
        SerializedProperty m_dialogueEntryGuid;
        Marker marker;

        // Get serialized object properties (for UI)
        public void OnEnable()
        {
            // Functional properties
            m_Time = serializedObject.FindProperty("m_Time");
            m_conversant = serializedObject.FindProperty("conversant");
            m_conversationGuid = serializedObject.FindProperty("conversationGuid");
            m_dialogueEntryGuid = serializedObject.FindProperty("dialogueEntryGuid");
        }

        // Draw inspector GUI
        public override void OnInspectorGUI()
        {
            var newMarker = target as Marker;

            // Make sure there is an instance of all objects before attempting to show anything in inspector
            if (newMarker == null || newMarker.parent == null || TimelineEditor.inspectedDirector == null) return;

            if (newMarker != marker && newMarker && newMarker.parent is not DialogueEventTrack)
                Debug.LogWarning("<color=red>DialogueSystemTools: Add Dialogue Event Marker to a Dialogue Event Track</color>");

            marker = newMarker;

            using var changeScope = new EditorGUI.ChangeCheckScope();
            EditorGUILayout.PropertyField(m_Time);
            EditorGUILayout.Space();

            //
            EditorGUILayout.PropertyField(m_conversant);

            // apply changes
            if (changeScope.changed) serializedObject.ApplyModifiedProperties();

            Utility.ConversationSelectorGUI(serializedObject, m_conversationGuid, m_dialogueEntryGuid);
            
            // Warning -- dialogue  event markers should only be used in event marker tracks for correct timeline preview behaviour

        }
    }

    // Editor used by the Timeline window to customize the appearance of a marker
    [CustomTimelineEditor(typeof(DialogueEventNotification))]
    public class DialogueEventOverlay : MarkerEditor
    {
        static readonly Texture2D iconTexture;
        static readonly Texture2D selectedTexture;

        static DialogueEventOverlay()
        {
            iconTexture = Resources.Load<Texture2D>("Icon");
            selectedTexture = Resources.Load<Texture2D>("Selected");
        }

        // Draws a vertical line on top of the Timeline window's contents.
        public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
        {
            // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
            var annotation = marker as DialogueEventNotification;
            if (annotation == null) return;

            if (uiState.HasFlag(MarkerUIStates.Selected))
            {
                var rect = new Rect(region.markerRegion);
                rect.width += 4;
                rect.x -= 2;
                GUI.DrawTexture(rect, selectedTexture);
            }
        }

        // Sets the marker's tooltip based on its title.
        public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            // The `marker argument needs to be cast as the appropriate type, usually the one specified in the `CustomTimelineEditor` attribute
            var eventMarker = marker as DialogueEventNotification;
            if (eventMarker == null) return base.GetMarkerOptions(marker);

            // Set marker icon
            EditorGUIUtility.SetIconForObject(eventMarker, iconTexture);

            // Create tooltip
            string tooltip = "";

            tooltip = tooltip.Length == 0 ? "No method" : tooltip.TrimEnd();
            return new MarkerDrawOptions { tooltip = tooltip };
        }
    }
#endif
}
