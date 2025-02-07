using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class MarbleGameSocket : MonoBehaviour
{
    public Dictionary<string, Marble> players = new Dictionary<string, Marble>();

    [SerializeField] Marble marblePrefab;

    WebSocket ws;
    string lastMsg;
    [SerializeField] int lobbyCode = 0;
    
    public TMP_Text lobbyCodeText;
    
    [SerializeField] private string websocketURL;

    void Start()
    {
        ws = new WebSocket(websocketURL);
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        
        ws.OnMessage += Ws_OnMessage;
        ws.OnOpen += Ws_OnOpen;
        // Add error handling
        ws.OnError += (sender, e) => Debug.LogError(string.Format("WebSocket Error: {0}", e.Message));
        ws.Connect();
        // REMOVE THE IMMEDIATE ws.Send() HERE
    }

    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("Web Socket is now connected");
        // Send the message only after the connection is confirmed open
        ws.Send("Unity is now connected");
        lobbyCode = Random.Range(10000, 99999);
        lobbyCodeText.text = "Lobby Code:\n" + lobbyCode;
    }
    
    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        lastMsg = e.Data;
    }

    private void OnDestroy()
    {
        if (ws != null) ws.Close();
    }

    void Update ()
    {
        if (lastMsg != null)
        {
            var tokens = lastMsg.Split('|');
            switch (tokens[0])
            {
                case "move":
                {
                    int lobbyCodeToken = int.Parse(tokens[5]);
                    if (lobbyCodeToken != lobbyCode)
                        return;
                    
                    var playerId = tokens[1];
                    var x = float.Parse(tokens[2]);
                    var z = -float.Parse(tokens[3]);
                    float time = int.Parse(tokens[4]);
                    if (players.ContainsKey(playerId))
                    {
                        players[playerId].Launch(x, z, time);
                    }
                    else
                    {
                        Debug.Log("Player: " + playerId + " is no longer connected");
                    }

                    break;
                }
                case "add":
                {
                    int lobbyCodeToken = int.Parse(tokens[3]);
                    if (lobbyCodeToken != lobbyCode)
                        return;
                    
                    var playerId = tokens[1];
                
                    var player = Instantiate(marblePrefab);
                    players[playerId] = player;
                
                    player.SetNameplate(tokens[2]);
                
                    ws.Send("player_added|"+playerId);
                    break;
                }
                case "del":
                {
                    int lobbyCodeToken = int.Parse(tokens[2]);
                    if (lobbyCodeToken != lobbyCode)
                        return;                    
                    
                    print("here");
                    var playerId = tokens[1];

                    if (players.ContainsKey(playerId))
                    {
                        Destroy(players[playerId].gameObject);
                        players.Remove(playerId);
                    }
                    else
                    {
                        Debug.Log("Player: " + playerId + " is no longer connected");
                    }
                    break;
                }
            }
            lastMsg = null;
        }
    }
}
