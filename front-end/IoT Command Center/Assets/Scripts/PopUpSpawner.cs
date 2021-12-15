using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSpawner : MonoBehaviour {

    public static PopUpSpawner INSTANCE = null;

    public GameObject PopUpPrefab;

    private void Awake () {
        INSTANCE = this;
    }

    public void CreatePopUp ( string randomId, string title, string time, string subtitle, int iconId, string description, PopUpAction? leftAction, PopUpAction? rightAction, int timeout, PopUpImportance importance ) {
        // Dismiss all other active popups.
        Array.ForEach(FindObjectsOfType<PopUp>(), item => { item.GetComponent<Animator>().SetTrigger("dismiss"); });
        
        GameObject go = Instantiate(PopUpPrefab, this.transform);
        go.name = randomId;

        PopUp popup = go.GetComponent<PopUp>();
        popup.Importance = importance;
        popup.Title = title;
        popup.Time = time;
        popup.Subtitle = subtitle;
        popup.IconId = iconId;
        popup.Description = description;
        popup.LeftButton = leftAction;
        popup.RightButton = rightAction;
        popup.Timeout = timeout;
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.P)) {
            string randomId = "popup_" + Guid.NewGuid().ToString();
            this.CreatePopUp(
                randomId,
                "Low on Coffee Beans!",
                MainScreen.CurrentDateTime.ToString("t"),
                "From 'Morning Coffee' Event, Tomorrow at 07:30",
                0,
                "Consider getting more 'Coffee Beans' to continue supporting your 'Morning Coffee' event.",
                null,
                new PopUpAction("OK", () => { GameObject.Find(randomId).GetComponent<Animator>().SetTrigger("dismiss"); }),
                120,
                PopUpImportance.LOW
            );
        }
    }

}
