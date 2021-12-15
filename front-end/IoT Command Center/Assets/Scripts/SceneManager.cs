using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SceneManager : MonoBehaviour {

    private int currentScenario = 0;
    //private int currentState = 0;

    private CarCharger carCharger;
    private void Awake () {
        carCharger = FindObjectOfType<CarCharger>();
    }

    // (F)(M)ood Cluster
    private int brusselSprouts = 0;
    private int peopleEatingDinner = 0;
    private bool sentRequest = false;

    private void StartSceneOne () {
        currentScenario = 1;
        MainScreen.CurrentDateTime = new DateTime(2035, 1, 5, 15, 30, 0);

        UpcomingEventSpawner.INSTANCE.Clear();

        OOCSILink.Subscribe("Inventory", OnInventoryUpdate);
        OOCSILink.Subscribe("Mood", OnMoodUpdate);
        OOCSILink.Subscribe("Location", OnLocationUpdate);
        OOCSILink.Subscribe("Planning", OnPlanningUpdate);

        sentRequest = false;
        brusselSprouts = 0;
        peopleEatingDinner = 4;

        if (carCharger != null)
            carCharger.ReactToOOCSI = true;
    }

    // Energy Cluster
    private void StartSceneTwo () {
        currentScenario = 2;
        MainScreen.CurrentDateTime = new DateTime(2035, 12, 17, 15, 30, 0);

        UpcomingEventSpawner.INSTANCE.Clear();

        if (carCharger != null) { 
            carCharger.State = CarChargerState.DISCHARGING;
            carCharger.Charge = 0.79f;
            carCharger.CarConnected = true;
            carCharger.ReactToOOCSI = true;
        }
    }

    // Service Cluster
    private void StartSceneThree () {
        currentScenario = 3;
        MainScreen.CurrentDateTime = new DateTime(2035, 08, 10, 10, 45, 0);

        UpcomingEventSpawner.INSTANCE.Clear();
        UpcomingEventSpawner.INSTANCE.AddUpcomingEvent("Washing Cycle", "White clothes", "11:15");
        UpcomingEventSpawner.INSTANCE.AddUpcomingEvent("Lunch", "2 People joining", "12:30");

        if (carCharger != null) {
            carCharger.ReactToOOCSI = false;
            carCharger.CarConnected = true;
            carCharger.State = CarChargerState.FAST_CHARGING;
            carCharger.Charge = 0.2f;
        } else {
            Debug.LogError("Couldn't find car charger.");
        }
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            StartSceneOne();
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            StartSceneTwo();
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            StartSceneThree();
        } else if (Input.GetKeyDown(KeyCode.Space)) {

            string randomId = "popup_" + Guid.NewGuid().ToString();
            switch (currentScenario) {
                case 1:
                    PopUpSpawner.INSTANCE.CreatePopUp(
                        randomId,
                        "Glass object dropped!",
                        MainScreen.CurrentDateTime.ToString("t"),
                        "Source: 'audioSensorKitchen'",
                        1,
                        "Detected broken glass in the kitchen. Setting the lights to bright now.",
                        null,
                        new PopUpAction("OK", () => { GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss"); }),
                        30,
                        PopUpImportance.VERY_HIGH
                        );

                    JObject data = new JObject(
                        new JProperty("forceLightSetting", "bright")
                        );
                    OOCSILink.Send("dodeca", data);

                    break;
                case 2:
                    PopUpSpawner.INSTANCE.CreatePopUp(
                        randomId,
                        "Energy planning mismatch!",
                        MainScreen.CurrentDateTime.ToString("t"),
                        "Expected high consumption, but current consumption is low.",
                        1,
                        "The energy planner had planned to charge the car at the current time, however, the charger decided not to due to high energy prices. How do you want to handle this?",
                        new PopUpAction("Update Planning (Manual)", () => GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss")),
                        new PopUpAction("Charge Communal Battery (Manual)", () => { GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss"); }),
                        -1,
                        PopUpImportance.LOW
                        );
                    break;
                case 3:
                    PopUpSpawner.INSTANCE.CreatePopUp(
                        randomId,
                        "High Humidity detected!",
                        MainScreen.CurrentDateTime.ToString("t"),
                        "Source: 'humidity_sensor_garage', 'water_pressure_sensor', 'car_charge_status'",
                        3,
                        "Unusual humidity levels in garage. High risk of hardware malfuction. Turned off car fast charging to prevent damage." +
                        Environment.NewLine +
                        "Next Actions: Closing off water mains and calling maintance company.",
                        new PopUpAction("Cancel Planned Actions", () => GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss")),
                        new PopUpAction("Take Actions Now", () => { SceneThreeTakeActions(); GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss"); }),
                        120,
                        PopUpImportance.HIGH
                        );

                    // Disable car charger.
                    if (carCharger != null) {
                        carCharger.State = CarChargerState.IDLE;
                    }
                    break;
                default:
                    break;
            }

        }


        switch (this.currentScenario) {
            case 1:
                if (brusselSprouts > 10 && peopleEatingDinner < 4 && !sentRequest) {
                    // Send fermentationrequest.
                    JObject data =
                        new JObject(
                            new JProperty("food", "brusselSprouts")
                            );

                    OOCSILink.Send("fermentation", data);

                    string randomId = "popup_" + Guid.NewGuid().ToString();
                    PopUpSpawner.INSTANCE.CreatePopUp(
                        randomId,
                        "Fermentation Request Sent",
                        MainScreen.CurrentDateTime.ToString("t"),
                        "Source: 'FoodInventory', 'peopleAtDinner'",
                        2,
                        "The yield of brussel sprouts is more than required. Automatically planned a fermentation request for this evening to ferment excess yield.",
                        null,
                        new PopUpAction("ok", () => { GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss"); }),
                        30,
                        PopUpImportance.VERY_LOW
                        );

                    UpcomingEventSpawner.INSTANCE.AddUpcomingEvent("Ferment Sprouts", "Automatic action", "19:00");

                    sentRequest = true;
                }
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

    }

    private void SceneThreeTakeActions () {
        UpcomingEventSpawner.INSTANCE.AddUpcomingEvent("House Service", "HighHumidityEvent", "12:35");
    }

    private void OnInventoryUpdate ( LinkMessage msg ) {
        msg.data.TryGetValue("brusselSprouts", out JToken sprouts);

        if (sprouts != null) {
            if (sprouts.Type == JTokenType.Integer) {
                this.brusselSprouts = sprouts.ToObject<int>();
            }
        }
    }

    private void OnMoodUpdate ( LinkMessage msg ) {

    }

    private void OnLocationUpdate ( LinkMessage msg ) {

    }

    private void OnPlanningUpdate ( LinkMessage msg ) {
        msg.data.TryGetValue("peopleDinner", out JToken peopleDinner);

        if (peopleDinner != null) {
            if (peopleDinner.Type == JTokenType.Integer) {
                this.peopleEatingDinner = peopleDinner.ToObject<int>();
            }
        }
    }

}

