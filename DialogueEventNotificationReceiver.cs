using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace DialogueSystemTools
{
    public class DialogueEventNotificationReceiver : MonoBehaviour, INotificationReceiver
    {
        public void StartConversation(Playable playable, string title, int entryID, bool pauseTimeline)
        {
            IEnumerator StartConversation()
            {
                if (pauseTimeline)
                {
                    var speed = playable.GetSpeed();
                    while (DialogueManager.Instance.IsConversationActive)
                    {
                        playable.SetSpeed(0);
                        yield return null;
                    }
                    playable.SetSpeed(speed);
                }

                DialogueManager.StartConversation(title, transform, null, entryID);

                if (pauseTimeline)
                {
                    var speed = playable.GetSpeed();
                    while (DialogueManager.Instance.IsConversationActive)
                    {
                        playable.SetSpeed(0);
                        yield return null;
                    }
                    playable.SetSpeed(speed);
                }

                yield break;
            }
            StartCoroutine(StartConversation());
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            //An INotificationReceiver will receive all the triggered notifications. We need to 
            //have a filter to use only the notifications that we can process.
            var message = notification as DialogueEventNotification;
            if (message == null) return;

            var pauseTimeline = message.pauseTimeline;
            var conversation = message.conversation;
            var dialogueEntry = message.dialogueEntry;

            conversation ??= Utility.FindConversation(DialogueManager.instance.initialDatabase.conversations, message.conversationGuid);
            dialogueEntry ??= Utility.FindDialogueEntry(conversation, message.dialogueEntryGuid);

            var root = origin.GetGraph().GetRootPlayable(0);

            StartConversation(root, conversation.Title, dialogueEntry.id, pauseTimeline);
        }
    }
}
