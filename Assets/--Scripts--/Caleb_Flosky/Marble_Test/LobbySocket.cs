using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class LobbySocket : MonoBehaviour
{
    public Dictionary<string, string> players = new Dictionary<string, string>();

    WebSocket ws;
    string lastMsg;
    int lobbyCode = Random.Range(10000, 99999);

    [SerializeField] private TMP_Text player_list;
    
    [SerializeField] private string websocketURL;

    void Start()
    {
        ws = new WebSocket(websocketURL);
        ws.OnMessage += Ws_OnMessage;
        ws.OnOpen += Ws_OnOpen;
        // Add error handling
        ws.OnError += (sender, e) => Debug.LogError(string.Format("WebSocket Error: {0}", e.Message));
        ws.Connect();
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
        player_list.text = string.Join("\n", players.Values);
        
        if (lastMsg != null)
        {
            var tokens = lastMsg.Split('|');
            switch (tokens[0])
            {
                case "add":
                {
                    var playerId = tokens[1];
                    var playerName = tokens[2];
                    int lobbyCodeToken = int.Parse(tokens[3]);
                    
                    if (lobbyCodeToken != lobbyCode)
                        return;
                    
                    players[playerId] = playerName;
                    
                    ws.Send("player_added|"+playerId);
                    break;
                }
                case "del":
                {
                    var playerId = tokens[1];
                    int lobbyCodeToken = int.Parse(tokens[2]);
                                        
                    if (lobbyCodeToken != lobbyCode)
                        return;

                    if (players.ContainsKey(playerId))
                    {
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
