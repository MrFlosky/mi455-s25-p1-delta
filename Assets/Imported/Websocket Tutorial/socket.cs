using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class socket : MonoBehaviour
{
    public Cannon cannon;

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
            float x = float.Parse(tokens[0]);
            float y = -float.Parse(tokens[1]);
            float time = int.Parse(tokens[2]);
            cannon.Fire(x, y, time);
            lastMsg = null;
        }
    }
}
