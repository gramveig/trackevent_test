using System;
using System.Collections;
using System.Collections.Generic;
using Alex.trackevent_service.Data;
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
        private bool _isBusy;
        private string _savePath;
        private EventsData _eventsData;

        private void Start()
        {
            _savePath = Application.persistentDataPath + "/" + "SavedEvents";
            Debug.Log("Persistent data path is: " + _savePath);
            RestoreSavedEvents();
        }

        private void Update()
        {
            if (_events.Count == 0 || _isUploading || _isBusy)
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

        private void OnDestroy()
        {
            if (_events.Count == 0 || _isUploading)
            {
                return;
            }

            SaveEvents();
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
            _eventsData = GetEventsData();
            SaveEventsBeforeUpload(_eventsData);
        }

        private void UploadAfterEventsSaved()
        {
            string jsonStr = JsonUtility.ToJson(_eventsData);

            _isUploading = true;
            _cooldownTimeCounter = 0;
            Debug.Log("Attempting to upload " + _events.Count + " event(s) to the server...");
            StartCoroutine(Upload(jsonStr));
        }
        
        private EventsData GetEventsData()
        {
            var eventsData = new EventsData
            {
                events = _events.ToArray()
            };

            return eventsData;
        }

        private IEnumerator Upload(string jsonString)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(_serverUrl, jsonString);
            webRequest.SetRequestHeader("Accept", "application/json");
            yield return webRequest.SendWebRequest();
     
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error during uploading events to server: \n" + webRequest.error);
                Debug.Log("Waiting " + _cooldownBeforeSend + " seconds before next attempt.");
            }
            else
            {
                Debug.Log(_events.Count + " event(s) uploaded successfully.");
                SaveManager.Instance.ClearFIle(_savePath);
                _events.Clear();
            }

            _isUploading = false;
        }

        private void RestoreSavedEvents()
        {
            _isBusy = true;
            SaveManager.Instance.Load<EventsData>(_savePath, OnEventsLoaded, true);
        }

        private void SaveEvents()
        {
            if (_events.Count == 0)
            {
                return;
            }

            Debug.Log("Saving " + _events.Count + " unsent event(s) before exiting application...");
            _isBusy = true;
            var eventsData = GetEventsData();
            SaveManager.Instance.Save(eventsData, _savePath, OnEventsSaved, true);
        }
        
        private void SaveEventsBeforeUpload(EventsData eventsData)
        {
            Debug.Log("Saving " + eventsData.events.Length + " event(s) before uploading them to server...");
            SaveManager.Instance.Save(eventsData, _savePath, OnEventsSavedBeforeUpload, true);
        }

        private void OnEventsLoaded(EventsData eventsData, SaveResult result, string message)
        {
            _isBusy = false;

            if (result != SaveResult.Success)
            {
                Debug.Log($"No saved events retrieved.\nresult: {result}, message: {message}");
                return;
            }

            if (eventsData.events.Length == 0)
            {
                Debug.Log("Zero events retrieved.");
                return;
            }

            _events = new List<EventData>(eventsData.events);
            Debug.Log("Successfully retrieved " + _events.Count + " event(s) from Save.");
            SaveManager.Instance.ClearFIle(_savePath);
        }

        private void OnEventsSaved(SaveResult result, string message)
        {
            _isBusy = false;
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving events.\nresult: {result}, message: {message}");
                return;
            }

            Debug.Log("Events saved successfully.");
            _events.Clear();
        }
        
        private void OnEventsSavedBeforeUpload(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving events.\nresult: {result}, message: {message}");
            }
            else
            {
                Debug.Log("Events saved successfully.");
            }

            UploadAfterEventsSaved();
        }
    }
}