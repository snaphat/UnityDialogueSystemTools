using System.ComponentModel;
using UnityEngine.Timeline;

namespace DialogueSystemTools
{
    [TrackColor(0.0f, 0.74f, 0.82f)]
    [TrackBindingType(typeof(DialogueEventNotificationReceiver)), DisplayName("Dialogue Event Track")]
    public class DialogueEventTrack : MarkerTrack
    {
    }
}
