using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class WebConnectionUpgrade : MonoBehaviour 
{
    private ClientWebSocket webSocket;
    // Use when you are hosting the game server locally.
    private string webSocketLocalUrl = "ws://localhost:8080/ws/unity";
    // Use when you are NOT hosting the game server.
    private string webSocketNonLocalUrl = "wss://calm-concise-longhorn.ngrok.app/ws/unity";
    private CancellationTokenSource cancellationToken;
    public GameObject ballFab;

    // Simple class that matches JSON
    public class DeviceData
    {
        public string username;
        public float fb_tilt;    
        public float lr_tilt;
        public int last_update;
    }

    // Holds all server data
    public static ConcurrentDictionary<int, DeviceData> unityServerDict = new ConcurrentDictionary<int, DeviceData>();

    // Connects ID to player's marble gameobject. Separate because concurrent dict is constantly being updated with new data.
    public static Dictionary<int, GameObject> playerObjDict = new Dictionary<int, GameObject>();
 
    async void Start()
    {
        webSocket = new ClientWebSocket();
        cancellationToken = new CancellationTokenSource();
        try 
        {
            await webSocket.ConnectAsync(new System.Uri(webSocketLocalUrl), cancellationToken.Token);
            Debug.Log("Connected to WebSocket server!");
            ReceiveServerData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebSocket connection error: {e.Message}");
        }
    }

private async void ReceiveServerData()
{
    // Estimate to allow for around ~40 users (4KB) worth of JSON dictionary
    byte[] buffer = new byte[4096]; 

    // Continously checks if web socket open, and listens for messages.
    while (webSocket.State == WebSocketState.Open)
    {
        try
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                buffer,
                cancellationToken.Token);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                try 
                {
                    Debug.Log($"Received JSON: {message}");
                    // Deserialize the JSON message where keys are strings
                    Dictionary<int, DeviceData> tempDataMap = JsonConvert.DeserializeObject<Dictionary<int, DeviceData>>(message);

                    Debug.Log("DESERIALIZE SUCCESS" + message);
                    // Convert to thread-safe dict and update Unity data
                    ConvertToThreadSafeDict(tempDataMap);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing JSON: {e.Message}");
                    Debug.LogError($"Message received: {message}");
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Debug.Log("WebSocket closed by server");
                break;
            }

            // 3ms delay between requests
            await Task.Delay(3, cancellationToken.Token); 
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error receiving message: {e.Message}");
            break;
        }

    }

}

    void ConvertToThreadSafeDict(Dictionary<int, DeviceData> localServerDict)
    {
        //Get keys in Unity dict that are missing in the updated 
        //server (those that were deleted). Delete them from unity.
        var DeletedKeys = unityServerDict.Keys.Except(localServerDict.Keys);
        foreach(var key in DeletedKeys)
        {
            unityServerDict.TryRemove(key, out _);

            if (playerObjDict.ContainsKey(key))
            {
                Destroy(playerObjDict[key]);
                playerObjDict.Remove(key);
            }
        }

        // First update dictionary and track which ones are new
        foreach (KeyValuePair<int, DeviceData> pair in localServerDict)
        {
            bool isNew = !unityServerDict.ContainsKey(pair.Key);
            
            unityServerDict.AddOrUpdate(
                pair.Key,           
                pair.Value,         
                (key, oldValue) => pair.Value 
            );

        // If this was a new key, create a ball
        if (isNew)
        {
            GameObject newBall = Instantiate(ballFab);
            newBall.transform.position = new Vector3(0, 20f, 0);
            newBall.GetComponent<Ball>().Init(pair.Key);
            playerObjDict[pair.Key] = newBall;
        }
        }
    }

    void OnDestroy()
    {
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
        }
        
        if (webSocket != null && 
            (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.Connecting))
        {
            webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application closing", 
                CancellationToken.None).Wait();
        }
    }
}