using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WebConnection : MonoBehaviour 
{
    private string webPageUrl = "https://calm-concise-longhorn.ngrok.app/sensorData";
    public TMP_Text txt;
    public GameObject ballFab;

    void Start()
    {
        StartCoroutine(GetSensorData());
    }

// Simple class that matches JSON
public class DeviceData
{
    public string username;
    public float fb_tilt;    
    public float lr_tilt;
}

// Holds all server data
public static ConcurrentDictionary<int, DeviceData> unityServerDict = new ConcurrentDictionary<int, DeviceData>();
 
// Updated coroutine
IEnumerator GetSensorData() 
{
    while (true)
    {
        UnityWebRequest request = UnityWebRequest.Get(webPageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            try
            {
                DeviceData data = JsonUtility.FromJson<DeviceData>(rawResponse);
                // Convert json dictionary to C#
                Dictionary<int, DeviceData> dataMap = JsonConvert.DeserializeObject<Dictionary<int, DeviceData>>(rawResponse);
                ConvertToThreadSafeDict(dataMap);
                //txt.text = $"Rotation: {data.rotation}\nFront/Back Tilt: {data.fb_tilt}\nLeft/Right Tilt: {data.lr_tilt}";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing JSON: {e.Message}");
            }
        }
        else
        {
            Debug.Log("FAIL");
        }

        yield return new WaitForSeconds(0.01f);
    }
}

void ConvertToThreadSafeDict(Dictionary<int, DeviceData> localServerDict)
{
    // Get keys in Unity dict that are missing in the updated 
    // server (those that were deleted). Delete them from unity.
    var DeletedKeys = unityServerDict.Keys.Except(localServerDict.Keys);
    foreach(var key in DeletedKeys)
    {
        unityServerDict.TryRemove(key, out _);
    }

    // First update dictionary and track which ones are new
    foreach (KeyValuePair<int, DeviceData> pair in localServerDict)
    {
        bool isNew = !unityServerDict.ContainsKey(pair.Key);
        
        unityServerDict.AddOrUpdate
        (
            pair.Key,           
            pair.Value,         
            (key, oldValue) => pair.Value 
        );

        // If this was a new key, create a ball
        if (isNew)
        {
            GameObject newBall = Instantiate(ballFab);
            newBall.GetComponent<Ball>().Init(pair.Key);
        }
    }
}

void Update()
{
    foreach (var pair in unityServerDict)
{
    Debug.Log($"ID: {pair.Key}, Username: {pair.Value.username} FB Tilt: {pair.Value.fb_tilt}, LR Tilt: {pair.Value.lr_tilt}");
}
}

}
