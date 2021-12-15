using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using WebSocketSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OOCSILink : MonoBehaviour {

    static WebSocket ws;

    private static Dictionary<string, List<CallbackDelegate>> s_subscriptions = new Dictionary<string, List<CallbackDelegate>>();
    private static Queue<KeyValuePair<string, CallbackDelegate>> subscribeRequests = new Queue<KeyValuePair<string, CallbackDelegate>>();

    public delegate void CallbackDelegate ( LinkMessage msg );

    private void Awake () {
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();

        ws.OnOpen += ( client, e ) => {
            Debug.Log("Connected.");
        };

        ws.OnMessage += ( client, e ) => {
            //Debug.Log($"LINK: RECEIVED: {e.Data}");
            JObject dataObj = JObject.Parse(e.Data);

            string recipient = dataObj.GetValue("recipient").ToString();
            JObject data = dataObj.GetValue("data").ToObject<JObject>();
            string sender = dataObj.GetValue("sender").ToString();
            ulong unixTimestamp = ulong.Parse(dataObj.GetValue("timestamp").ToString());

            LinkMessage linkMessage = new LinkMessage(recipient, data, sender, unixTimestamp);

            Debug.Log($"MSG: From {linkMessage.sender} to {linkMessage.recipient} with {linkMessage.data}");

            if (s_subscriptions.ContainsKey(recipient)) {
                foreach (CallbackDelegate callback in s_subscriptions[recipient]) {
                    try {
                        ThreadHelper.AddToQueue(() => callback(linkMessage));
                    } catch (System.Exception error) {
                        Debug.LogError(error);
                    }
                }
            }
        };
    }

    public static void Subscribe ( string channel, CallbackDelegate cb = null ) {
        if (ws.ReadyState != WebSocketState.Open) {
            subscribeRequests.Enqueue(new KeyValuePair<string, CallbackDelegate>(channel, cb));
            Debug.Log($"Queued subscription to {channel} as OOCSILink is not connected.");
        } else {
            // Send empty object to node back-end, which automatically subscribes to it.
            Send(channel, new JObject());

            if (cb == null) { 
                cb = ( e ) => { };
                Debug.Log($"Subscribed to {channel} with no callback.");
            } else {
                Debug.Log($"Subscribed to {channel} with a callback to {cb.Method.Name}.");
            }

            if (s_subscriptions.ContainsKey(channel)) {
                s_subscriptions[channel].Add(cb);
            } else {
                s_subscriptions.Add(channel, new List<CallbackDelegate>() { cb });
            }
        }
    }

    public static void Send ( string channel, JObject data ) {
        LinkMessage linkMessage = new LinkMessage(channel, data, "icc", (ulong)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds));
        string json = JsonConvert.SerializeObject(linkMessage);

        if (ws.ReadyState == WebSocketState.Open)
            ws.Send(json);
        else {
            Debug.LogError("Could't send message as socket was not open.");
        }
    }

    public static WebSocketState GetOOCILinkState () => OOCSILink.ws.ReadyState;

    private void Update () {
        if (ws == null) {
            return;
        }

        if (ws.ReadyState == WebSocketState.Open) {
            if (subscribeRequests.Count > 0) {
                var req = subscribeRequests.Dequeue();
                Subscribe(req.Key, req.Value);
            }
        } else if (ws.ReadyState == WebSocketState.Closed) {
            if ((int)Time.realtimeSinceStartup % 5 == 0) {
                Debug.Log("Trying to reconnect...");
                ws.ConnectAsync(); 
            }
        }
    }

    private void OnDestroy () {
        ws.Close();
    }

}

public struct LinkMessage {
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
