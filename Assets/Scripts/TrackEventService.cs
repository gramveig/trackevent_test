using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Alex.trackevent_service
{
    public class TrackEventService : MonoBehaviour
    {
        [SerializeField]
        private string _serverUrl;
        
        public void TrackEvent(string type, string data)
        {
            if (string.IsNullOrEmpty(_serverUrl))
            {
                Debug.LogError("Server URL is empty");
                return;
            }

            TextAsset jsonStr = Resources.Load<TextAsset>("json_example");

            StartCoroutine(Upload(jsonStr.text));
        }
        
        IEnumerator Upload(string jsonString)
        {
            WWWForm form = new WWWForm();
            form.AddField("events", jsonString);
     
            UnityWebRequest www = UnityWebRequest.Post(_serverUrl, form);
            yield return www.SendWebRequest();
     
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}