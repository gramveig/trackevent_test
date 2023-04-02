using System.Collections;
using System.Collections.Generic;
using Alex.trackevent_service.Data;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace Alex.trackevent_service
{
    public class TrackEventService : MonoBehaviour
    {
        [SerializeField]
        private string _serverUrl;

        [SerializeField]
        private float _cooldownBeforeSend;
        
        private List<EventData> _events = new List<EventData>();
        
        public void TrackEvent(string type, string data)
        {
            if (string.IsNullOrEmpty(_serverUrl))
            {
                Debug.LogError("Server URL is empty");
                return;
            }

            _events.Add(new EventData(type, data));
            EventsData eventsData = new EventsData
            {
                events = _events.ToArray()
            };
            string jsonStr = JsonUtility.ToJson(eventsData);

            StartCoroutine(Upload(jsonStr));
        }
        
        IEnumerator Upload(string jsonString)
        {
            WWWForm form = new WWWForm();
            form.AddField("events", jsonString);
     
            UnityWebRequest webRequest = UnityWebRequest.Post(_serverUrl, form);
            webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return webRequest.SendWebRequest();
     
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}