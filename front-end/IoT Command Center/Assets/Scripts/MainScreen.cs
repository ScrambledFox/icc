using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainScreen : MonoBehaviour {

    public static MainScreen INSTANCE = null;

    public Transform modulePrefab;
    public ModulePosition[,] modulePositions;
    public Dictionary<ModulePosition, Module> modules = new Dictionary<ModulePosition, Module>();

    public Vector2Int gridSize = new Vector2Int(11, 12);

    private void Awake () {
        INSTANCE = this;

        modulePositions = new ModulePosition[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++) {
            for (int x = 0; x < gridSize.x; x++) {
                Vector3 pos = new Vector3(x * 2f, y * 2f, 0) - new Vector3(gridSize.x, gridSize.y, 0);
                bool locked = true;

                if ((Mathf.Abs(pos.x) == 5 && Mathf.Abs(pos.y) < 4) || (Mathf.Abs(pos.y) == 4 && Mathf.Abs(pos.x) < 5))
                    locked = false;

                modulePositions[x, y] = new ModulePosition(new Vector2Int(x, y), pos, locked);
            }
        }
    }

    public void LockModule ( Module module, ModulePosition pos ) {
        pos.hasModule = true;
        
        module.isLocked = true;
        module.pos = pos;

        if (!modules.ContainsKey(pos))
            modules.Add(pos, module);
        else {
            modules[pos] = module;
        }
    }

    public void UnlockModule ( Module module ) {
        if (modules.ContainsKey(module.pos)) {
            module.pos.hasModule = false;
            modules.Remove(module.pos);
        }

        module.isLocked = false;
        module.pos = null;
    }

    //public void UnlockNeighbours ( ModulePosition modPos ) {
    //    modulePositions[modPos.gridPos.x - 1, modPos.gridPos.y].locked = false;
    //    modulePositions[modPos.gridPos.x + 1, modPos.gridPos.y].locked = false;
    //    modulePositions[modPos.gridPos.x, modPos.gridPos.y - 1].locked = false;
    //    modulePositions[modPos.gridPos.x, modPos.gridPos.y + 1].locked = false;

    //    foreach (var pos in modulePositions) {
    //        if (Mathf.Abs(pos.position.x) < 5f && Mathf.Abs(pos.position.y) < 4f) {
    //            pos.locked = true;
    //        }
    //    }
    //}

    //public void LockNeighbours ( ModulePosition modPos ) {
    //    modulePositions[modPos.gridPos.x - 1, modPos.gridPos.y].locked = true;
    //    modulePositions[modPos.gridPos.x + 1, modPos.gridPos.y].locked = true;
    //    modulePositions[modPos.gridPos.x, modPos.gridPos.y - 1].locked = true;
    //    modulePositions[modPos.gridPos.x, modPos.gridPos.y + 1].locked = true;

    //    foreach (var pos in modulePositions) {
    //        if ((Mathf.Abs(pos.position.x) == 5 && Mathf.Abs(pos.position.y) < 4) || (Mathf.Abs(pos.position.y) == 4 && Mathf.Abs(pos.position.x) < 5))
    //            pos.locked = false;
    //    }

    //}

    public void CheckNeighbours () {
        Debug.Log("Updating neighbours...");
    }

    private void OnDrawGizmos () {
        if (modulePositions == null) return;

        foreach (var modulePos in modulePositions) {
            if (modulePos == null) continue;

            if (modulePos.locked) {
                Gizmos.color = Color.red;
            } else if (modulePos.hasModule) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawWireCube(modulePos.position, new Vector3(1.95f, 1.95f, 0.2f));
            Gizmos.DrawSphere(modulePos.position, 0.1f);
        }
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.M)) {
            Instantiate(modulePrefab);
        }
    }

}

public class ModulePosition {

    public Vector2Int gridPos = Vector2Int.zero;
    public Vector3 position = Vector3.zero;
    public bool hasModule = false;
    public bool locked = false;

    public ModulePosition ( Vector2Int gridPos, Vector3 position, bool locked = false ) {
        this.gridPos = gridPos;
        this.position = position;
        this.locked = locked;
    }
}
