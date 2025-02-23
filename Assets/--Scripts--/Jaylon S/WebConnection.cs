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
    private string nonLocalUrl = "https://calm-concise-longhorn.ngrok.app/sensorData";
    private string localUrl = "http://localhost:8080/sensorData";
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

// Connects ID to player's marble gameobject. Seperate because concurrent dict is constantly being updated with new data.
public static Dictionary<int, GameObject> playerObjDict = new Dictionary<int, GameObject>();
 
// Gets all user data from server as dictionary, converts to unity dictionary.
IEnumerator GetSensorData() 
{
    while (true)
    {
        UnityWebRequest request = UnityWebRequest.Get(localUrl);
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

        playerObjDict.Remove(key);
        //Debug.Log(playerObjDict + "DELETED");
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
            newBall.transform.position = new Vector3(0,20f,0);
            newBall.GetComponent<Ball>().Init(pair.Key);
            
            newBall.GetComponent<PlayerTeam>().SetTeam(GameSetup.Instance.GameStarted);

            playerObjDict[pair.Key] = newBall;
            //Debug.Log(playerObjDict.Keys);
        }
    }
}

public class ScoreboardData
{
    public string userID;
    public string username;    
    public bool isAlive;
}

private IEnumerator PostToScoreboard(string userID, string username, bool isAlive)
{
    ScoreboardData data = new ScoreboardData
    {
        userID = userID,
        username = username,
        isAlive = isAlive
    };

    string jsonData = JsonUtility.ToJson(data);
    
    UnityWebRequest request = new UnityWebRequest("http://localhost:8080/scoreboard", "POST");
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Error: {request.error}");
    }
    else
    {
        Debug.Log("Scoreboard updated successfully!");
    }
}

void Update()
{
    foreach (var pair in unityServerDict)
{
    //Debug.Log($"ID: {pair.Key}, Username: {pair.Value.username} FB Tilt: {pair.Value.fb_tilt}, LR Tilt: {pair.Value.lr_tilt}");
}
}

}
