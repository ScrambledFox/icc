using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using WebSocketSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OOCSILink : MonoBehaviour {

    WebSocket ws;

    private void Start () {
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();

        ws.OnOpen += (client, e) => {
            Debug.Log("Connected.");
        };

        ws.OnMessage += (client, e) => {
            Debug.Log($"LINK: RECEIVED: {e.Data}");

            JObject dataObj = JObject.Parse(e.Data);

            string recipient = dataObj.GetValue("recipient").ToString();
            JObject data = dataObj.GetValue("data").ToObject<JObject>();
            string sender = dataObj.GetValue("sender").ToString();
            ulong unixTimestamp = ulong.Parse(dataObj.GetValue("timestamp").ToString());
            
            LinkMessage linkMessage = new LinkMessage(recipient, data, sender, unixTimestamp);
            Debug.Log($"MSG: {linkMessage.recipient} with {linkMessage.data.ToString()}");
        };
    }

    private void SendLinkMessage ( string channel, JObject message ) {
        LinkMessage linkMessage = new LinkMessage(channel, message, "icc", (ulong)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds));
        string json = JsonConvert.SerializeObject(linkMessage);
        ws.Send(json);
    }

    private void Update () {
        if (ws == null) {
            return;
        }

        if (ws.ReadyState == WebSocketState.Closed) {
            if ((int)Time.realtimeSinceStartup % 5 == 0) {
                Debug.Log("Trying to reconnect...");
                ws.ConnectAsync(); 
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            JObject obj = new JObject(
                new JProperty("temperature", 21.2)
            );

            this.SendLinkMessage("icc-test-channel", obj);
        }
    }

    private void OnDestroy () {
        ws.Close();
    }

}

public class LinkMessage {

    public string recipient;
    public ulong timestamp;
    public string sender;
    public JObject data;

    public LinkMessage ( string recipient, JObject data, string sender, ulong timestamp ) {
        this.recipient = recipient;
        this.timestamp = timestamp;
        this.sender = sender;
        this.data = data;
    }

}