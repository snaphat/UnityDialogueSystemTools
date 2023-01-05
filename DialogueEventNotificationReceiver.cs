using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace DialogueSystemTools
{
    public class DialogueEventNotificationReceiver : MonoBehaviour, INotificationReceiver
    {
        bool timelineStopped = false;
        public void StartConversation(Playable playable, string title, int entryID, bool pauseTimeline)
        {


            IEnumerator StartConversation()
            {
                // One frame delay is needed before the conversation starts to avoid buggy dialogue fade-in 
                yield return null;

                IEnumerator Pause()
                {
                    if (pauseTimeline)
                    {
                        var coroutinePausedTimeline = false; // whether this coroutine paused the timeline
                        var speed = playable.GetSpeed(); // cache speed
                        while (DialogueManager.IsConversationActive)
                        {
                            if (!timelineStopped)
                            {
                                coroutinePausedTimeline = timelineStopped = true;
                                playable.SetSpeed(0);
                            }
                            yield return null;
                        }
                        if (coroutinePausedTimeline) // if this couroutine paused the timeline restore the speed
                        {
                            timelineStopped = false; // mark the timeline as no longer stopped
                            playable.SetSpeed(speed);
                        }
                    }
                }

                yield return Pause(); // Pause if pausing enabled

                DialogueManager.StartConversation(title, transform, null, entryID); // Start conversation

                yield return Pause(); // Pause if pausing enabled

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
