using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class MarbleGameSocket : MonoBehaviour
{
    public Dictionary<string, Marble> players = new Dictionary<string, Marble>();

    [SerializeField] Marble marblePrefab;

    WebSocket ws;
    string lastMsg;

    void Start()
    {
        ws = new WebSocket("ws://goldenrod-majestic-board.glitch.me/ws");
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
                    var playerId = tokens[1];
                    var x = float.Parse(tokens[2]);
                    var z = -float.Parse(tokens[3]);
                    float time = int.Parse(tokens[4]);
                    if (players[playerId])
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
                    var playerId = tokens[1];
                
                    var player = Instantiate(marblePrefab);
                    players[playerId] = player;
                
                    player.SetNameplate(tokens[2]);
                
                    ws.Send("player_added|"+playerId);
                    break;
                }
                case "del":
                {
                    print("here");
                    var playerId = tokens[1];

                    if (players[playerId])
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
