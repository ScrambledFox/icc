using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    int zoomLevel = 0;
    Vector2 position = Vector2.zero;
    Vector2 orbitPosition = Vector2.zero;

    Vector3 mousePosBeforeDrag;
    Vector2 positionBeforeDrag;
    Vector2 orbitPosBeforeDrag;

    public float zoomInterval = 0.5f;
    public float maxZoom = 8f;

    public float mouseSensitivity = 0.1f;
    public bool xInverted, yInverted = false;

    public float positionSmoothing = 20f;
    public float rotationSmoothing = 20f;

    public float positionLimit = 10f;
    public float orbitLimit = 2f;

    private void Update () {
        // Panning
        if (Input.GetMouseButtonDown(1)) {
            mousePosBeforeDrag = Input.mousePosition;
            positionBeforeDrag = position;
        }
        if (Input.GetMouseButton(1)) {
            Vector3 delta = mousePosBeforeDrag - Input.mousePosition;
            position = positionBeforeDrag + new Vector2(delta.x * mouseSensitivity * (xInverted ? -1 : 1), delta.y * mouseSensitivity * (yInverted ? -1 : 1));
        }
        position = Vector2.ClampMagnitude(position, positionLimit);

        // Limited Orbitting 
        if (Input.GetMouseButtonDown(2)) {
            mousePosBeforeDrag = Input.mousePosition;
            orbitPosBeforeDrag = orbitPosition;
        }
        if (Input.GetMouseButton(2)) {
            Vector3 delta = mousePosBeforeDrag - Input.mousePosition;
            orbitPosition = orbitPosBeforeDrag + new Vector2(delta.x * mouseSensitivity * (xInverted ? -1 : 1), delta.y * mouseSensitivity * (yInverted ? -1 : 1));
        }
        orbitPosition = Vector2.ClampMagnitude(orbitPosition, orbitLimit);

        // Zooming
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            zoomLevel++;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            zoomLevel--;
        }
        zoomLevel = Mathf.Clamp(zoomLevel, 0, (int)(maxZoom / zoomInterval));

        // Applying
        Vector3 targetPosition = new Vector3(position.x, position.y, 10 - zoomLevel * zoomInterval) + new Vector3(orbitPosition.x, orbitPosition.y, 0);

        Vector3 forwardVector = new Vector3(position.x, position.y, 0) - targetPosition;
        Quaternion targetRotation = Quaternion.LookRotation(forwardVector, Vector3.up);

        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, positionSmoothing * Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }

    public void ResetOrbitPosition () {
        this.orbitPosition = Vector2.zero;
    }


}
