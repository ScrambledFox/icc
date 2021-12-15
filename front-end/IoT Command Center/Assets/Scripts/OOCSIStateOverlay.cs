using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOCSIStateOverlay : MonoBehaviour {

    public Transform connectionText;

    private void Update () {
        connectionText.gameObject.SetActive(OOCSILink.GetOOCILinkState() == WebSocketSharp.WebSocketState.Open ? false : true);
    }

}
