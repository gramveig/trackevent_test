using UnityEngine;

namespace Alex.trackevent_service
{
    public class TestService : MonoBehaviour
    {
        [SerializeField]
        private TrackEventService _trackEventService;

        private void Start()
        {
            _trackEventService.TrackEvent("test", "123");
        }
    }
}