using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    private int currentScene = 0;
    private int scenarioState = 0;

    // (F)(M)ood Cluster
    private void StartSceneOne () {
        MainScreen.CurrentDateTime = new DateTime(2035, 1, 5, 15, 30, 0);

        OOCSILink.Subscribe("testchannel");
    }

    // Energy Cluster
    private void StartSceneTwo () {
        MainScreen.CurrentDateTime = new DateTime(2035, 12, 17, 15, 30, 0);
    }

    // Service Cluster
    private void StartSceneThree () {
        MainScreen.CurrentDateTime = new DateTime(2035, 12, 17, 15, 30, 0);
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            StartSceneOne();
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            StartSceneTwo();
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            StartSceneThree();
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            scenarioState++;
        }

        switch (currentScene) {
            case 1:
                switch (scenarioState) {
                    default:
                        break;
                }
                break;
            case 2:
                switch (scenarioState) {
                    default:
                        break;
                }
                break;
            case 3:
                switch (scenarioState) {
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

}
