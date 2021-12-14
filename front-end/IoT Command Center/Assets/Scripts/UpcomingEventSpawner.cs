using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpcomingEventSpawner : MonoBehaviour {

    public GameObject EventPanelPrefab;
    public Transform ContentTransform;

    public void AddUpcomingEvent ( string title, string subtitle, string time ) {
        GameObject go = Instantiate(EventPanelPrefab, ContentTransform);
        go.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        go.transform.Find("Subtitle").GetComponent<TextMeshProUGUI>().text = subtitle;
        go.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = time;
    }

}
