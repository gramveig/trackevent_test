using System;

namespace Alex.trackevent_service.Data
{
    [Serializable]
    public class EventData
    {
        public string type;
        public string data;

        public EventData(string type, string data)
        {
            this.type = type;
            this.data = data;
        }
    }
}