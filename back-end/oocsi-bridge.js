"use strict";
import { WebSocketServer } from "ws";
import OOCSI from "OOCSI";

const OOCSI_ID = "icc_" + Date.now().toString();
const ws = new WebSocketServer({ port: 8080 });

OOCSI.connect("wss://oocsi.id.tue.nl/ws", OOCSI_ID);

ws.on("connection", (ws, req) => {
  // Say hello to unity.
  console.log("AtHome connected.");
  ws.send("welcome");

  // On link message
  ws.on("message", (data) => {
    console.log("LINK: RECEIVED: " + data);
    const obj = JSON.parse(data);

    // Subscribe to channel we get a link message from.
    console.log("Subscribing to", obj.recipient);
    OOCSI.subscribe(obj.recipient, (e) => {
      console.log("OOCSI: RECEIVED: " + e);
      ws.send(JSON.stringify(e));
    });

    OOCSI.send(obj.recipient, obj.data);
    console.log("OOCSI: SENT:", obj.data, "to", obj.recipient);
  });

  ws.on("close", () => {
    console.log("Server closed for some reason.");

    OOCSI.unsubscribe();
  });

  ws.on("error", (err) => {
    console.log(err);
  });
});
