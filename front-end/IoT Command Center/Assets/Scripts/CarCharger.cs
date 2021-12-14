using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarCharger : MonoBehaviour {

    public enum ChargeState {
        DISCHARGE = -1,
        CHARGE = 1,
        FAST_CHARGE = 2
    }

    private TextMeshProUGUI text;

    private float percentToRangeModifier = 6.14f;

    public float Charged { get; set; } = 100;

    private void Awake () {
        text = transform.Find("ChargePercent").GetComponent<TextMeshProUGUI>();
    }

    private void Update () {
        text.text = Charged + "%" + Environment.NewLine + Charged * percentToRangeModifier + "km";
    }

}
