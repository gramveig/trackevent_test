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
        private float _cooldownTimeCounter;
        private bool _isUploading;

        private void Update()
        {
            if (_events.Count == 0 || _isUploading)
            {
                return;
            }

            if (_cooldownTimeCounter >= _cooldownBeforeSend)
            {
                UploadEvents();
                return;
            }

            _cooldownTimeCounter += Time.deltaTime;
        }

        public void TrackEvent(string type, string data)
        {
            if (string.IsNullOrEmpty(_serverUrl))
            {
                Debug.LogError("Server URL is empty");
                return;
            }

            _events.Add(new EventData(type, data));
            Debug.Log("Adding an event to upload. Current number of events: " + _events.Count);
        }

        private void UploadEvents()
        {
            EventsData eventsData = new EventsData
            {
                events = _events.ToArray()
            };
            string jsonStr = JsonUtility.ToJson(eventsData);

            _isUploading = true;
            _cooldownTimeCounter = 0;
            Debug.Log("Attempting to upload " + _events.Count + " event(s) to the server...");
            StartCoroutine(Upload(jsonStr));
        }

        private IEnumerator Upload(string jsonString)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(_serverUrl, jsonString);
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return webRequest.SendWebRequest();
     
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error during uploading events to server: \n" + webRequest.error);
                Debug.Log("Waiting " + _cooldownBeforeSend + " seconds before next attempt.");
            }
            else
            {
                Debug.Log(_events.Count + " event(s) uploaded successfully.");
                _events.Clear();
            }

            _isUploading = false;
        }
    }
}