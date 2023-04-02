using UnityEngine;
using EasyButtons;

namespace Alex.trackevent_service
{
    public class TestService : MonoBehaviour
    {
        [SerializeField]
        private TrackEventService _trackEventService;

        [SerializeField]
        private string _type = "levelStart";
        [SerializeField]
        private string _data = "level:3";

        [Button]
        public void TrackEvent()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Start application to track events");
                return;
            }

           _trackEventService.TrackEvent(_type, _data);
        }
    }
}