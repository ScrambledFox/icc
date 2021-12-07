using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleMovement : MonoBehaviour {

    Vector2 position;
    Vector2 dragHandleOffset;

    Vector3 point;

    new Rigidbody rigidbody;
    Module module;

    private void Awake () {
        position = this.transform.position;
        rigidbody = this.transform.GetComponent<Rigidbody>();
        module = this.transform.GetComponent<Module>();
    }

    private void OnMouseDown () {
        dragHandleOffset = this.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }

    private void OnMouseDrag () {
        point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));

        if (module.isLocked) {
            rigidbody.MovePosition(module.pos.position);

            if (Vector3.Distance(point, module.pos.position - new Vector3(dragHandleOffset.x, dragHandleOffset.y, 0)) > 0.7f) {
                MainScreen.INSTANCE.CheckNeighbours();
                MainScreen.INSTANCE.UnlockModule(module);
            }

            return;
        }

        position = dragHandleOffset + new Vector2(point.x, point.y);
        Vector3 targetPosition = new Vector3(position.x, position.y, 0);

        foreach (var pos in MainScreen.INSTANCE.modulePositions) {
            if (this.module.pos == pos) {
                targetPosition = pos.position;
                break;
            }

            if (pos.locked || pos.hasModule) continue;

            if (Vector3.Distance(targetPosition, pos.position) < 0.5f) {
                targetPosition = pos.position;
                MainScreen.INSTANCE.LockModule(this.GetComponent<Module>(), pos);
                MainScreen.INSTANCE.CheckNeighbours();
                break;
            }
        }

        rigidbody.MovePosition(targetPosition);
    }

    private void Update () {
        // Clamp Position
        this.transform.position = Vector3.ClampMagnitude(this.transform.position, 10);
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.blue;

        if (module.pos != null) Gizmos.DrawSphere(module.pos.position - new Vector3(dragHandleOffset.x, dragHandleOffset.y, 0), 0.5f);
        
        Gizmos.color = Color.white;
        if (point != null) Gizmos.DrawSphere(point, 0.5f);
    }

}
