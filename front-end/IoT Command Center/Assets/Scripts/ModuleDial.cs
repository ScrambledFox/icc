using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDial : MonoBehaviour {

    private Transform dialBody;

    private Vector3 mousePosBeginDrag;
    private Vector3 mouseDelta;

    private float valueBeforeDrag;
    [Range(0.0f, 1.0f)]
    public float Value = 0.0f;

    private float pushValueBeforeDrag;
    [Range(0.0f, 1.0f)]
    public float PushValue = 0.5f;

    private bool joysticking = false;
    public Vector2 JoystickPos = Vector2.zero;

    private void Awake () {
        dialBody = this.transform.parent;
    }

    private void Update () {
        Value = Mathf.Clamp01(Value);
        PushValue = Mathf.Clamp01(PushValue);

        this.transform.localRotation = Quaternion.Euler(0, 0, -(20f + Value * 320f));
        this.transform.localPosition = new Vector3(0, 0, -0.25f - PushValue * 1.4f);
        this.dialBody.localPosition = new Vector3(-JoystickPos.x * 0.25f, this.dialBody.localPosition.y, JoystickPos.y * 0.25f);
    }

    private void OnMouseDrag () {
        mouseDelta = (mousePosBeginDrag - Input.mousePosition);

        if (Input.GetKey(KeyCode.LeftControl)) {
            JoystickPos = Vector2.ClampMagnitude(new Vector2(-mouseDelta.x, -mouseDelta.y) / 200f, 1f);
            joysticking = true;
        } else if (Input.GetKey(KeyCode.LeftAlt)) {
            PushValue = Mathf.Clamp01(pushValueBeforeDrag + mouseDelta.y / 200f);
        } else {
            Value = Mathf.Clamp01(valueBeforeDrag + mouseDelta.y / 200f);
        }
    }

    private void OnMouseUp () {
        if (joysticking) JoystickPos = Vector2.zero;
    }

    private void OnMouseDown () {
        mousePosBeginDrag = Input.mousePosition;
        valueBeforeDrag = Value;
        pushValueBeforeDrag = PushValue;
    }

}
