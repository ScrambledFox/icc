"use strict";
import { WebSocketServer } from "ws";
import OOCSI from "OOCSI";

const OOCSI_ID = "icc_" + Date.now().toString();
const ws = new WebSocketServer({ port: 8080 });

OOCSI.connect("wss://oocsi.id.tue.nl/ws", OOCSI_ID);

ws.on("connection", (ws) => {
  // Say hello to unity.
  ws.send("welcome");

  // On link message
  ws.on("message", (data) => {
    console.log("LINK: RECEIVED: " + data);
    const obj = JSON.parse(data);

    // Subscribe to channel we get a link message from.
    OOCSI.subscribe(obj.recipient, (e) => {
      console.log("OOCSI: RECEIVED: " + e);
      ws.send(JSON.stringify(e));
    });

    OOCSI.send(obj.recipient, obj.data);
    console.log("OOCSI: SENT:", obj.data, "to", obj.recipient);
  });
});
