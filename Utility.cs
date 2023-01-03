using System;
using System.Collections.Generic;
using System.IO;
using PixelCrushers.DialogueSystem;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogueSystemTools
{
    class Utility
    {
        public static string GetScriptPath([System.Runtime.CompilerServices.CallerFilePath] string filename = null)
        {
            return Path.GetDirectoryName(filename[(Application.dataPath.Length - "Assets".Length)..]);
        }

        // Add guid field entry to list (Conversation or DialogueEntry, whichever the list represents)
        public static string AddGuidField(List<Field> fields)
        {
            var field = fields.Find(x => x.title == "GUID");
            if (field == null)
            {
                field = new Field("GUID", Guid.NewGuid().ToString("N"), FieldType.Text);
                fields.Add(field);
                return field.value;
            }
            else if (field.type == FieldType.Text)
            {
                if (field.value == "") field.value = Guid.NewGuid().ToString("N");
                return field.value;
            }
            else
            {
                Debug.LogWarning("DialogueSystemTools: 'GUID' field exist on entry but type is not 'FieldType.Text'");
                return null;
            }
        }

        public static Conversation FindConversation(List<Conversation> conversations, string guid)
        {
            if (guid == null || guid == "") return null;
            return conversations.Find(x => x.fields.Find(x => x.title == "GUID" && x.value == guid) != null);
        }

        public static DialogueEntry FindDialogueEntry(Conversation conversation, string guid)
        {
            if (guid == null || guid == "" || conversation == null) return null;
            return conversation.dialogueEntries.Find(x => x.fields.Find(x => x.title == "GUID" && x.value == guid) != null);
        }

        // find index of conversation entry with matching guid or return -1
        public static int FindConversationIndex(List<Conversation> conversations, string guid)
        {
            if (guid == null || guid == "") return -1;
            return conversations.FindIndex(x => x.fields.Find(x => x.title == "GUID" && x.value == guid) != null);
        }

        // Find index of dialogue entry with matching guid or return -1
        public static int FindDialogueEntryIndex(Conversation conversation, string guid)
        {
            if (guid == null || guid == "" || conversation == null) return -1;
            return conversation.dialogueEntries.FindIndex(x => x.fields.Find(x => x.title == "GUID" && x.value == guid) != null);
        }

#if UNITY_EDITOR
        public static void ConversationSelectorGUI(SerializedObject serializedObject, SerializedProperty conversationGuid, SerializedProperty dialogueEntryGuid)
        {
            serializedObject.Update();
            {
                using var changeScope = new EditorGUI.ChangeCheckScope();

                // Grab conversations
                var conversations = DialogueManager.instance.initialDatabase.conversations;
                int prev; // used to check if an update is needed
                int i = FindConversationIndex(conversations, conversationGuid.stringValue);
                int j = i != -1 ? FindDialogueEntryIndex(conversations[i], dialogueEntryGuid.stringValue) : -1;

                // Draw conversation selector field
                var titles = conversations.ConvertAll(x => new string("[" + x.id + "] " + x.Title)).ToArray();
                i = EditorGUILayout.Popup("Conversation", prev = i, titles);

                // If conversation selected
                if (i != -1 && i < conversations.Count)
                {
                    // Update conversation guid if changed (add guid if doesn't exist)
                    var conversation = conversations[i];
                    if (prev != i)
                    {
                        conversationGuid.stringValue = AddGuidField(conversation.fields);
                        EditorUtility.SetDirty(DialogueManager.instance.initialDatabase);
                    }
                    // Grab dialogue entries
                    var texts = conversation.dialogueEntries.ConvertAll(x => new string("[" + x.id + "] " + x.DialogueText)).ToArray();

                    // Compute style and draw dialogue entry selector for potential multi-line dialogue entry dropdown
                    GUIStyle style = new(EditorStyles.popup);
                    if (j != -1 && j < conversation.dialogueEntries.Count)
                        style.fixedHeight = EditorGUIUtility.singleLineHeight * texts[j].Split("\n").Length;
                    j = EditorGUILayout.Popup("DialogueEntry", prev = j, texts, style);

                    // Fill in space needed for multiple line dialogue
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(style.fixedHeight - EditorGUIUtility.singleLineHeight);
                    EditorGUILayout.EndVertical();

                    // If dialogue entry selected
                    if (j != -1 && j < conversation.dialogueEntries.Count)
                    {
                        // Update dialogue entry guid if changed (add guid if doesn't exist)
                        var dialogueEntry = conversation.dialogueEntries[j];
                        if (prev != j)
                        {
                            dialogueEntryGuid.stringValue = AddGuidField(dialogueEntry.fields);
                            EditorUtility.SetDirty(DialogueManager.instance.initialDatabase);
                        }
                    }
                }

                // apply changes
                if (changeScope.changed) serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
