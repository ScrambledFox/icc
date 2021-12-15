using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour {

    [HideInInspector]
    public PopUpImportance Importance;

    public AudioClip[] PopUpSounds;
    public AudioClip DismissSound;

    public Sprite[] iconSprites;

    private Button[] buttons;

    private PopUpAction? leftAction;
    private PopUpAction? rightAction;
    private float timeout = -1;

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
    public int IconId {
        set {
            transform.Find("BigIcon").GetComponent<Image>().sprite = this.iconSprites[value];
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
    public PopUpAction? LeftButton {
        set {
            leftAction = value;

            if (value == null)
                transform.Find("LeftButton").gameObject.SetActive(false);
            else
                InitButton(0, value.GetValueOrDefault());
        }
        get {
            return leftAction;
        }
    }
    public PopUpAction? RightButton {
        set {
            rightAction = value;

            if (value == null)
                transform.Find("RightButton").gameObject.SetActive(false);
            else
                InitButton(1, value.GetValueOrDefault());
        }
        get {
            return rightAction;
        }
    }
    public float Timeout {
        set {
            timeout = value;
        }
        get {
            return timeout;
        }
    }

    private void Awake () {
        buttons = gameObject.GetComponentsInChildren<Button>();
    }

    private void InitButton ( int button, PopUpAction value ) {
        Button btn = buttons[button];
        btn.GetComponentInChildren<TextMeshProUGUI>().text = value.title;

        if (value.action != null) {
            btn.onClick.AddListener(() => { value.action(); });
        } else {
            btn.interactable = false;
        }
    }

    private void Update () {
        if (timeout > 0) {
            timeout -= UnityEngine.Time.deltaTime * MainScreen.INSTANCE.TimeAcceleration;

            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = rightAction.Value.title + $" ({Mathf.FloorToInt(timeout) + 1})";

            if (timeout <= 0)
                rightAction.Value.action?.Invoke();
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

public struct PopUpAction {

    public string title;
    public Action action;

    public PopUpAction ( string title = "action", Action action = null ) {
        this.title = title;
        this.action = action;
    }
}

public enum PopUpImportance {
    VERY_LOW, LOW, MEDIUM, HIGH, VERY_HIGH,
}
