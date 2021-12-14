using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour {

    [HideInInspector]
    public PopUpImportance Importance;

    public AudioClip[] PopUpSounds;
    public AudioClip DismissSound;

    public string Title { 
        set { 
            transform.Find("Title").GetComponentInChildren<TextMeshProUGUI>().text = value; 
        } get {
            return transform.Find("Title").GetComponentInChildren<TextMeshProUGUI>().text;
        } 
    }
    public string Time {
        set {
            transform.Find("Time").GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
        get {
            return transform.Find("Time").GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }
    public string Subtitle {
        set {
            transform.Find("Subtitle").GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
        get {
            return transform.Find("Subtitle").GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }
    public string Description {
        set {
            transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
        get {
            return transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }
    public string LeftButtonText {
        set {
            transform.Find("LeftButton").GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
        get {
            return transform.Find("LeftButton").GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }
    public string RightButtonText {
        set {
            transform.Find("RightButton").GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
        get {
            return transform.Find("RightButton").GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }

    public void PlayPopUpSound () {
        MainScreen.INSTANCE.GetComponent<AudioSource>().PlayOneShot(PopUpSounds[(int)Importance]);
    }

    public void PlayDismissSound () {
        MainScreen.INSTANCE.GetComponent<AudioSource>().PlayOneShot(DismissSound);
    }

    public void DestroyMe () {
        Destroy(this.gameObject);
    }

}

public enum PopUpImportance {
    VERY_LOW, LOW, MEDIUM, HIGH, VERY_HIGH,
}
