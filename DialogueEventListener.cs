using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystemTools
{
    public enum ListenerMethod
    {
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

        public void Awake()
        {
            AddListener();

            conversation = Utility.FindConversation(DialogueManager.instance.initialDatabase.conversations, conversationGuid);
            dialogueEntry = Utility.FindDialogueEntry(conversation, dialogueEntryGuid);

            // PlayOnAwake/Play workaround bc played events are trigger before we registered ours if it woke up before
            // us
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

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnUse(Transform actor)
        {
            if (listener == ListenerMethod.OnUse)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnBarkStart(Transform actor)
        {
            if (listener == ListenerMethod.OnBarkStart)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnBarkEnd(Transform actor)
        {
            if (listener == ListenerMethod.OnBarkEnd)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);

        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnConversationStart(Transform actor)
        {
            if (listener == ListenerMethod.OnConversationStart)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnConversationEnd(Transform actor)
        {
            if (listener == ListenerMethod.OnConversationEnd)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnSequenceStart(Transform actor)
        {
            if (listener == ListenerMethod.OnSequenceStart)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Dialogue System for Unity (PixelCrushers)
        public void OnSequenceEnd(Transform actor)
        {
            if (listener == ListenerMethod.OnSequenceEnd)
                if (tagMatch == "" || actor.tag == tagMatch)
                    StartConversation(actor);
        }

        // Message Listener for Unity Collider
        public void OnTriggerStay(Collider other)
        {
            if (listener == ListenerMethod.OnTriggerStay)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider
        public void OnTriggerEnter(Collider other)
        {
            if (listener == ListenerMethod.OnTriggerEnter)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider
        public void OnTriggerExit(Collider other)
        {
            if (listener == ListenerMethod.OnTriggerExit)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider
        public void OnCollisionStay(Collision collision)
        {
            if (listener == ListenerMethod.OnCollisionStay)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Message Listener for Unity Collider
        public void OnCollisionEnter(Collision collision)
        {
            if (listener == ListenerMethod.OnCollisionEnter)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Message Listener for Unity Collider
        public void OnCollisionExit(Collision collision)
        {
            if (listener == ListenerMethod.OnCollisionExit)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnTriggerStay2D(Collider2D other)
        {
            if (listener == ListenerMethod.OnTriggerStay2D)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (listener == ListenerMethod.OnTriggerEnter2D)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnTriggerExit2D(Collider2D other)
        {
            if (listener == ListenerMethod.OnTriggerExit2D)
                if (tagMatch == "" || other.tag == tagMatch)
                    StartConversation(other.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnCollisionStay2D(Collision2D collision)
        {
            if (listener == ListenerMethod.OnCollisionStay2D)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (listener == ListenerMethod.OnCollisionEnter2D)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Message Listener for Unity Collider2D
        public void OnCollisionExit2D(Collision2D collision)
        {
            if (listener == ListenerMethod.OnCollisionExit2D)
                if (tagMatch == "" || collision.gameObject.tag == tagMatch)
                    StartConversation(collision.gameObject.transform);
        }

        // Event Listener for Unity PlayableDirector
        public void OnPlayed(PlayableDirector director)
        {
            if (listener == ListenerMethod.OnPlayed)
                if (tagMatch == "" || director.tag == tagMatch)
                    StartConversation(director.transform);
        }

        // Event Listener for Unity PlayableDirector
        public void OnPaused(PlayableDirector director)
        {
            if (listener == ListenerMethod.OnPaused)
                if (tagMatch == "" || director.tag == tagMatch)
                    StartConversation(director.transform);
        }

        // Event Listener for Unity PlayableDirector
        public void OnStopped(PlayableDirector director)
        {
            if (listener == ListenerMethod.OnStopped)
                if (tagMatch == "" || director.tag == tagMatch)
                    StartConversation(director.transform);
        }

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
        SerializedProperty m_conversationGuid;
        SerializedProperty m_dialogueEntryGuid;

        // Get serialized object properties (for UI)
        public void OnEnable()
        {
            // Functional properties
            m_Listener = serializedObject.FindProperty("listener");
            m_TagMatch = serializedObject.FindProperty("tagMatch");
            m_conversationGuid = serializedObject.FindProperty("conversationGuid");
            m_dialogueEntryGuid = serializedObject.FindProperty("dialogueEntryGuid");
        }

        // Draw inspector GUI
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                using var changeScope = new EditorGUI.ChangeCheckScope();

                // Draw listener field
                EditorGUILayout.PropertyField(m_Listener);

                // Draw tag selector field
                if (m_TagMatch.stringValue == "") m_TagMatch.stringValue = "Untagged";
                m_TagMatch.stringValue = EditorGUILayout.TagField("Tag", m_TagMatch.stringValue);
                if (m_TagMatch.stringValue == "Untagged") m_TagMatch.stringValue = "";

                // apply changes
                if (changeScope.changed) serializedObject.ApplyModifiedProperties();
            }

            Utility.ConversationSelectorGUI(serializedObject, m_conversationGuid, m_dialogueEntryGuid);
        }
    }
#endif
}
