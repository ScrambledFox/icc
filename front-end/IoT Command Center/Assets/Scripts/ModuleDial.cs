using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDial : MonoBehaviour {

    private Transform dialBody;

    private string state = "idle";

    private void Awake () {
        dialBody = this.transform.parent;
    }

    void Update() {
        
    }

    private void OnMouseEnter () {
        this.state = "hover";
    }

    private void OnMouseExit () {
        this.state = "idle";
    }

    private void OnMouseDrag () {
        this.state = "drag";
    }

    private void OnMouseUp () {
        if (this.state == "drag") {
            this.state = "idle";
        }
    }
}
