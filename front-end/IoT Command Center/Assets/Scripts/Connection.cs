using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour {

    //private Module module;

    public GameObject BackgroundGraphic;
    public GameObject AlertGraphic;

    private void Awake () {
        AlertGraphic.SetActive(false);
    }

    //public void SetModule (Module module) {
    //    this.module = module;
    //}

    public void SetAlert ( string title ) {
        this.AlertGraphic.SetActive(true);
    }

}
