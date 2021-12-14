using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSpawner : MonoBehaviour {

    public static PopUpSpawner INSTANCE = null;

    public GameObject PopUpPrefab;

    private void Awake () {
        INSTANCE = this;
    }

    public void PopUp ( string title, string time, string subtitle, string description, string leftButtonText, string rightButtonText, PopUpImportance importance ) {
        Array.ForEach(FindObjectsOfType<PopUp>(), item => { item.GetComponent<Animator>().SetTrigger("dismiss"); });
        
        GameObject go = Instantiate(PopUpPrefab, this.transform);
        PopUp popup = go.GetComponent<PopUp>();
        popup.Importance = importance;
        popup.Title = title;
        popup.Time = time;
        popup.Subtitle = subtitle;
        popup.Description = description;
        popup.LeftButtonText = leftButtonText;
        popup.RightButtonText = rightButtonText;
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.P)) {
            this.PopUp("Test PopUp", MainScreen.CurrentDateTime.ToString("t"), "Test Event", "This is a test event.", "Action 1", "Action 2", (PopUpImportance)UnityEngine.Random.Range(0, 5));
        }
    }

}
