using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleMovement : MonoBehaviour {

    Vector2 position;
    Vector2 dragHandleOffset;

    Vector3 point;

    new Rigidbody rigidbody;
    Module module;

    public Vector2 Position { get => position; }

    private void Awake () {
        position = this.transform.position;
        rigidbody = this.transform.GetComponent<Rigidbody>();
        module = this.transform.GetComponent<Module>();
    }

    private void OnMouseDown () {
        dragHandleOffset = this.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));

        Camera.main.GetComponent<CameraControl>().ResetOrbitPosition();
    }

    private void OnMouseDrag () {
        point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));

        // Try to disconnect if distance is too great.
        if (module.IsConnected) {
            rigidbody.MovePosition(module.ModulePosition.GlobalPosition);

            if (Vector3.Distance(point, module.ModulePosition.GlobalPosition - new Vector3(dragHandleOffset.x, dragHandleOffset.y, 0)) > 0.7f) {
                module.Disconnect();
            }

            return;
        }

        position = dragHandleOffset + new Vector2(point.x, point.y);
        Vector3 targetPosition = new Vector3(position.x, position.y, 0);

        // Check if we can connect to any...
        foreach (var pos in MainScreen.INSTANCE.grid) {
            if (this.module.CanConnect(pos)) {
                targetPosition = pos.GlobalPosition;
                this.GetComponent<Module>().Connect(pos);
                break;
            }
        }

        rigidbody.MovePosition(targetPosition);
    }

    private void Update () {
        if (module.IsConnected) {
            rigidbody.MovePosition(module.ModulePosition.GlobalPosition);
        }

        // Clamp Position
        this.transform.position = Vector3.ClampMagnitude(this.transform.position, 10);
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        if (module.ModulePosition != null) Gizmos.DrawSphere(module.ModulePosition.GlobalPosition, 0.5f);

        Gizmos.color = Color.blue;
        if (module.ModulePosition != null) Gizmos.DrawSphere(module.ModulePosition.GlobalPosition - new Vector3(dragHandleOffset.x, dragHandleOffset.y, 0), 0.5f);
        
        Gizmos.color = Color.white;
        if (point != null) Gizmos.DrawSphere(point, 0.5f);
    }

}
