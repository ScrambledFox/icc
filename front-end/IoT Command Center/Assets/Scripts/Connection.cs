using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour {

    Animator backgroundGraphicAnimator;
    Animator alertGraphicAnimator;

    private bool hasAlert = false;

    public GameObject BackgroundGraphic;
    public GameObject AlertGraphic;

    public bool HasAlert { get => hasAlert; }

    public Flow Flow;

    private void Awake () {
        backgroundGraphicAnimator = this.BackgroundGraphic.GetComponent<Animator>();
        alertGraphicAnimator = this.AlertGraphic.GetComponent<Animator>();

        this.Flow = this.GetComponentInChildren<Flow>();
    }

    public void SetModule ( bool connected ) {
        backgroundGraphicAnimator.SetBool("hasModule", connected);
        if (connected) {
            backgroundGraphicAnimator.SetTrigger("moduleConnected");
        } else {
            backgroundGraphicAnimator.SetTrigger("moduleDisconnected");
        }
    }

    public void SetAlert ( string title ) {
        backgroundGraphicAnimator.ResetTrigger("moduleConnected");
        backgroundGraphicAnimator.ResetTrigger("moduleDisconnected");

        hasAlert = true;

        backgroundGraphicAnimator.SetBool("hasAlert", true);
        alertGraphicAnimator.SetBool("hasAlert", true);
    }

    public void ResetAlert () {
        backgroundGraphicAnimator.SetBool("hasAlert", false);
        alertGraphicAnimator.SetBool("hasAlert", false);

        hasAlert = false;
    }

    public void SetFlow ( bool state ) {
        this.Flow.SetActive(state);
    }

}
