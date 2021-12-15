using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Newtonsoft.Json.Linq;

public class CarCharger : MonoBehaviour {

    private TextMeshProUGUI chargeStatusText;
    private Animator animator;

    private uint maxRange = 614;

    private bool carConnected = false;
    private CarChargerState wantedState = CarChargerState.DISABLED;
    private float currentCharge = 1.0f;

    public bool CarConnected { get => carConnected; set { carConnected = value; UpdateCarCharger(); } }
    public CarChargerState State {
        get { return carConnected ? wantedState : CarChargerState.DISABLED; }
        set { wantedState = value; UpdateCarCharger(); }
    }
    public float Charge {
        get { return currentCharge; }
        set { currentCharge = value; }
    }
    public bool ReactToOOCSI { get; set; } = true;

    private void Start () {
        chargeStatusText = transform.Find("ChargePercent").GetComponent<TextMeshProUGUI>();
        animator = GetComponentInChildren<Animator>();

        OOCSILink.Subscribe("carcharger", OnCarChargeUpdate);
    }

    private void OnCarChargeUpdate ( LinkMessage message ) {
        if (!ReactToOOCSI) return;

        message.data.TryGetValue("vehicleConnected", out JToken vehicleConnected);
        message.data.TryGetValue("chargeStatus", out JToken chargeStatus);
        message.data.TryGetValue("chargePercent", out JToken chargePercent);

        if (vehicleConnected != null) {
            if (vehicleConnected.Type == JTokenType.Boolean) {
                if (vehicleConnected.ToObject<bool>()) {
                    this.carConnected = true;
                } else {
                    this.carConnected = false;
                }
            }
        }

        if (chargeStatus != null) {
            if (chargeStatus.Type == JTokenType.Integer) {
                switch (chargeStatus.ToObject<int>()) {
                    case -1:
                        this.wantedState = CarChargerState.DISCHARGING;
                        break;
                    case 0:
                        this.wantedState = CarChargerState.IDLE;
                        break;
                    case 1:
                        this.wantedState = CarChargerState.CHARGING;
                        break;
                    case 2:
                        this.wantedState = CarChargerState.FAST_CHARGING;
                        break;
                }
            }
        }

        if (chargePercent != null) {
            if (chargePercent.Type == JTokenType.Float) {
                currentCharge = Mathf.Clamp01((float)chargePercent);
            }
        }
        
        UpdateCarCharger();
    }

    private void UpdateCarCharger () {
        switch (State) {
            case CarChargerState.DISABLED:
                animator.SetBool("carPresent", false);
                break;
            case CarChargerState.IDLE:
                animator.SetBool("carPresent", true);
                animator.SetInteger("charge", 0);
                break;
            case CarChargerState.CHARGING:
                animator.SetBool("carPresent", true);
                animator.SetInteger("charge", 1);
                break;
            case CarChargerState.FAST_CHARGING:
                animator.SetBool("carPresent", true);
                animator.SetInteger("charge", 2);
                break;
            case CarChargerState.DISCHARGING:
                animator.SetBool("carPresent", true);
                animator.SetInteger("charge", -1);
                break;
            default:
                break;
        }

        return;
    }

    private void Update () {
        currentCharge = Mathf.Clamp01(currentCharge);
        chargeStatusText.text = (int)(currentCharge * 100) + "%" + Environment.NewLine + (int)(currentCharge * maxRange) + "km";
    }

}

public enum CarChargerState {
    DISABLED, IDLE, CHARGING, FAST_CHARGING, DISCHARGING
}
