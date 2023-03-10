using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystemTools
{
    public enum ListenerMethod
    {
        // Listeners for MonoBehaviours 
        Awake,
        Start,
        OnEnable,
        OnDisable,

        // Listeners for Dialogue System for Unity (PixelCrushers) Messages
        OnUse,
        OnBarkStart,
        OnBarkEnd,
        OnConversationStart,
        OnConversationEnd,
        OnSequenceStart,
        OnSequenceEnd,

        // Listeners for Unity Collider Messages
        OnTriggerStay,
        OnTriggerEnter,
        OnTriggerExit,
        OnCollisionStay,
        OnCollisionEnter,
        OnCollisionExit,

        // Listeners for Unity Collider2D Messages
        OnTriggerStay2D,
        OnTriggerEnter2D,
        OnTriggerExit2D,
        OnCollisionStay2D,
        OnCollisionEnter2D,
        OnCollisionExit2D,

        // Listeners for Unity PlayableDirector Events
        OnPlayed, // play event
        OnPaused, // paused event
        OnStopped // stopped event
    }

    class DialogueEventListener : MonoBehaviour
    {
        public string conversationGuid = "";
        public string dialogueEntryGuid = "";

        Conversation conversation;
        DialogueEntry dialogueEntry;

        public ListenerMethod listener; // listener method
        public string tagMatch = "";    // Tag Match check

        [Serializable]
        public class QuestCondition
        {
            [QuestPopup]
            public string name;

            [QuestState]
            public QuestState state;
        };

        public QuestCondition[] questConditions;

        // Check logic for each listener and tag combination
        public void CheckMatch(ListenerMethod listener, Transform actor)
        {
            if (this.listener == listener)
                if (tagMatch == "" || (actor != null && tagMatch == actor.tag))
                {
                    foreach (var quest in questConditions)
                        if (QuestLog.GetQuestState(quest.name) != quest.state)
                            return;
                    StartConversation(actor);
                }
        }

        // Add Event listener for PlayableDirector Events
        public void AddListener()
        {
            var director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                RemoveListener();
                if (listener == ListenerMethod.OnPlayed)
                    director.played += OnPlayed;
                else if (listener == ListenerMethod.OnPaused)
                    director.paused += OnPaused;
                else if (listener == ListenerMethod.OnStopped)
                    director.stopped += OnStopped;
            }
        }

        // Remove Event listener for PlayableDirector Events
        public void RemoveListener()
        {
            var director = GetComponent<PlayableDirector>();
            if (director != null)
            {
                director.played -= OnPlayed;
                director.paused -= OnPaused;
                director.stopped -= OnStopped;
            }
        }

        // Message Listeners for MonoBehaviours
        public void Awake()
        {
            AddListener();

            conversation = Utility.FindConversation(DialogueManager.instance.initialDatabase.conversations, conversationGuid);
            dialogueEntry = Utility.FindDialogueEntry(conversation, dialogueEntryGuid);

            // PlayOnAwake/Play workaround bc played events are trigger before we registered ours if it woke up before us
            if (listener == ListenerMethod.OnPlayed)
            {
                // if the playablegraph is playing and it just started then we need to invoke our callbacks
                var director = GetComponent<PlayableDirector>();
                if (director != null && director.playableGraph.IsValid() && director.playableGraph.IsPlaying()
                    && director.time == director.initialTime)
                {
                    // Wait one frame to make sure all scene objects are initialized before invoking callbacks 
                    IEnumerator DelayInvokeCallbacks()
                    {
                        yield return null;
                        OnPlayed(director); // Manually call OnPlayed callback
                        yield break;
                    };
                    StartCoroutine(DelayInvokeCallbacks());
                }
            }

            CheckMatch(ListenerMethod.Awake, null);
        }
        public void Start() { CheckMatch(ListenerMethod.Start, null); }
        public void OnEnable() { CheckMatch(ListenerMethod.OnEnable, null); }
        public void OnDisable() { CheckMatch(ListenerMethod.OnDisable, null); }

        // Message Listeners for Dialogue System for Unity (PixelCrushers)
        public void OnUse(Transform actor) { CheckMatch(ListenerMethod.OnUse, actor); }
        public void OnBarkStart(Transform actor) { CheckMatch(ListenerMethod.OnBarkStart, actor); }
        public void OnBarkEnd(Transform actor) { CheckMatch(ListenerMethod.OnBarkEnd, actor); }
        public void OnConversationStart(Transform actor) { CheckMatch(ListenerMethod.OnConversationStart, actor); }
        public void OnConversationEnd(Transform actor) { CheckMatch(ListenerMethod.OnConversationEnd, actor); }
        public void OnSequenceStart(Transform actor) { CheckMatch(ListenerMethod.OnSequenceStart, actor); }
        public void OnSequenceEnd(Transform actor) { CheckMatch(ListenerMethod.OnSequenceEnd, actor); }

        // Message Listeners for Unity Collider
        public void OnTriggerStay(Collider other) { CheckMatch(ListenerMethod.OnTriggerStay, other.transform); }
        public void OnTriggerEnter(Collider other) { CheckMatch(ListenerMethod.OnTriggerEnter, other.transform); }
        public void OnTriggerExit(Collider other) { CheckMatch(ListenerMethod.OnTriggerExit, other.transform); }
        public void OnCollisionStay(Collision collision) { CheckMatch(ListenerMethod.OnCollisionStay, collision.transform); }
        public void OnCollisionEnter(Collision collision) { CheckMatch(ListenerMethod.OnCollisionEnter, collision.transform); }
        public void OnCollisionExit(Collision collision) { CheckMatch(ListenerMethod.OnCollisionExit, collision.transform); }

        // Message Listeners for Unity Collider2D
        public void OnTriggerStay2D(Collider2D other) { CheckMatch(ListenerMethod.OnTriggerStay2D, other.transform); }
        public void OnTriggerEnter2D(Collider2D other) { CheckMatch(ListenerMethod.OnTriggerEnter2D, other.transform); }
        public void OnTriggerExit2D(Collider2D other) { CheckMatch(ListenerMethod.OnTriggerExit2D, other.transform); }
        public void OnCollisionStay2D(Collision2D collision) { CheckMatch(ListenerMethod.OnCollisionStay2D, collision.transform); }
        public void OnCollisionEnter2D(Collision2D collision) { CheckMatch(ListenerMethod.OnCollisionEnter2D, collision.transform); }
        public void OnCollisionExit2D(Collision2D collision) { CheckMatch(ListenerMethod.OnCollisionExit2D, collision.transform); }

        // Event Listeners for Unity PlayableDirector
        public void OnPlayed(PlayableDirector director) { CheckMatch(ListenerMethod.OnPlayed, director.transform); }
        public void OnPaused(PlayableDirector director) { CheckMatch(ListenerMethod.OnPaused, director.transform); }
        public void OnStopped(PlayableDirector director) { CheckMatch(ListenerMethod.OnStopped, director.transform); }

        // Callback for starting a conversation
        public void StartConversation(Transform actor)
        {
            DialogueManager.StartConversation(conversation.Title, actor, transform, dialogueEntry.id);
        }
    }

#if UNITY_EDITOR
    // Custom Inspector for creating EventListener
    [CustomEditor(typeof(DialogueEventListener)), CanEditMultipleObjects]
    public class DialogueEventListenerEditor : Editor
    {
        SerializedProperty m_Listener;
        SerializedProperty m_TagMatch;
        SerializedProperty m_QuestConditions;
        SerializedProperty m_ConversationGuid;
        SerializedProperty m_DialogueEntryGuid;

        // Get serialized object properties (for UI)
        public void OnEnable()
        {
            // Functional properties
            m_Listener = serializedObject.FindProperty("listener");
            m_TagMatch = serializedObject.FindProperty("tagMatch");
            m_QuestConditions = serializedObject.FindProperty("questConditions");
            m_ConversationGuid = serializedObject.FindProperty("conversationGuid");
            m_DialogueEntryGuid = serializedObject.FindProperty("dialogueEntryGuid");
        }

        // Draw inspector GUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                using var changeScope = new EditorGUI.ChangeCheckScope();

                // Draw listener field
                EditorGUILayout.PropertyField(m_Listener);
                EditorGUILayout.PropertyField(m_QuestConditions);

                // Draw tag selector field
                if (m_TagMatch.stringValue == "") m_TagMatch.stringValue = "Untagged";
                m_TagMatch.stringValue = EditorGUILayout.TagField("Tag Condition", m_TagMatch.stringValue);
                if (m_TagMatch.stringValue == "Untagged") m_TagMatch.stringValue = "";

                // apply changes
                if (changeScope.changed) serializedObject.ApplyModifiedProperties();
            }

            Utility.ConversationSelectorGUI(serializedObject, m_ConversationGuid, m_DialogueEntryGuid);
        }
    }
#endif
}
