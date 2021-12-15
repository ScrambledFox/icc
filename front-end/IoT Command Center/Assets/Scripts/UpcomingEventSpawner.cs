using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpcomingEventSpawner : MonoBehaviour {

    public static UpcomingEventSpawner INSTANCE = null;

    public GameObject EventPanelPrefab;
    public Transform ContentTransform;

    public List<GameObject> EventObjects = new List<GameObject>();

    private void Awake () {
        INSTANCE = this;
    }

    public void AddUpcomingEvent ( string title, string subtitle, string time ) {
        GameObject go = Instantiate(EventPanelPrefab, ContentTransform);
        go.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        go.transform.Find("Subtitle").GetComponent<TextMeshProUGUI>().text = subtitle;
        go.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = time;
        EventObjects.Add(go);
    }

    public void Clear () {
        foreach (var go in EventObjects) {
            Destroy(go);
        }

        EventObjects.Clear();
    }

}
