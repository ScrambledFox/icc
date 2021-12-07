"use strict";

var OOCSI = require("OOCSI");

OOCSI.connect("wss://oocsi.id.tue.nl/ws", "icc_" + Date.now().toString());

// OOCSI.subscribe("unixtime", (msg) => {
//   console.log("RECEIVED:" + msg);
// });

// setInterval(() => {
//   let msg = { time: Date.now().toString() };
//   OOCSI.send("unixtime", msg);
//   console.log("SENT: " + msg.time);
// }, 1000);
