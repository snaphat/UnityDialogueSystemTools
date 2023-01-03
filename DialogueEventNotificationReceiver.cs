using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace DialogueSystemTools
{
    public class DialogueEventNotificationReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            //An INotificationReceiver will receive all the triggered notifications. We need to 
            //have a filter to use only the notifications that we can process.
            var message = notification as DialogueEventNotification;
            if (message == null) return;

            var conversation = message.conversation;
            var dialogueEntry = message.dialogueEntry;

            DialogueManager.StartConversation(conversation.Title, transform, null, dialogueEntry.id);
        }
    }
}
