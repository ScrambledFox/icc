using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class Module : MonoBehaviour {

    private ModulePosition modPos = null;
    private ModuleMovement movement;
    private AudioSource audioSource;

    private bool isConnected = false;
    private bool hasAlert = false;
    private string label;
    
    public AudioClip attachSound;
    public AudioClip detachSound;

    public string Label { get => label; set => label = value; }
    public bool IsConnected { get => isConnected; }
    public bool HasAlert { get => hasAlert; }
    public ModulePosition ModulePosition { get => modPos; }

    private void Awake () {
        this.movement = this.GetComponent<ModuleMovement>();
        this.audioSource = this.GetComponent<AudioSource>();

        this.label = "Module_" + this.gameObject.GetInstanceID();
    }

    public void Connect ( ModulePosition modPos ) {
        modPos.HasModule = true;
        this.isConnected = true;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(attachSound);

        this.modPos = modPos;

        var modulesDictionary = MainScreen.INSTANCE.modulesDictionary;
        if (!modulesDictionary.ContainsKey(this.modPos))
            modulesDictionary.Add(this.modPos, this);
        else {
            modulesDictionary[this.modPos] = this;
        }

        Debug.Log($"Module {this.GetInstanceID()} locked.");

        MainScreen.INSTANCE.CheckModules(true);
    }

    public ModulePosition CanConnectAny () {
        foreach (var pos in MainScreen.INSTANCE.grid) {
            if (this.CanConnect(pos))
                return pos;

        }

        return null;
    }

    public bool CanConnect ( ModulePosition modPos ) {
        if (modPos.Locked || modPos.HasModule) return false;
        return Vector3.Distance(movement.Position, modPos.GlobalPosition) < 0.5f;
    }

    public void Disconnect () {
        var modulesDictionary = MainScreen.INSTANCE.modulesDictionary;

        if (modulesDictionary.ContainsKey(this.modPos)) {
            this.modPos.HasModule = false;
            modulesDictionary.Remove(this.modPos);
        }

        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(detachSound);

        this.isConnected = false;
        this.modPos = null;

        Debug.Log($"Module {this.GetInstanceID()} unlocked.");

        MainScreen.INSTANCE.CheckModules();
    }

    private void OnDrawGizmos () {
        Gizmos.color = this.isConnected ? Color.green : Color.red;
        Gizmos.DrawSphere(this.transform.position + new Vector3(0.5f, 0.5f, 0), 0.25f);
    }
}

public class ModulePosition {

    public enum SpecialState {
        NONE,
        ALWAYS_LOCKED,
        ALWAYS_UNLOCKED,
    }

    private bool locked;
    private bool hasModule;
    private Vector2Int gridPos;
    private Vector3 globalPosition;
    private SpecialState specialState;
    private Connection connectionGraphic;

    public bool Locked { get => locked; }
    public bool HasModule {
        get => hasModule; set {
            this.hasModule = value;
            if (value) {
                this.connectionGraphic?.BackgroundGraphic.GetComponent<Animator>().SetTrigger("moduleConnected");
            } else {
                this.connectionGraphic?.BackgroundGraphic.GetComponent<Animator>().SetTrigger("moduleDisconnected");
            }
        }
    }
    public Vector2Int GridPosition { get => gridPos; }
    public Vector3 GlobalPosition { get => globalPosition; }
    public SpecialState State { get => specialState; }

    public ModulePosition ( Vector2Int gridPos, Vector3 position, Connection connectionGraphic = null, SpecialState specialState = SpecialState.NONE ) {
        this.gridPos = gridPos;
        this.globalPosition = position;
        this.hasModule = false;
        this.specialState = specialState;
        this.connectionGraphic = connectionGraphic;

        this.locked = specialState == SpecialState.ALWAYS_UNLOCKED ? false : true;
    }

    public bool Lock () {
        if (specialState == SpecialState.ALWAYS_UNLOCKED) {
            return false;
        } else {
            return this.locked = true;
        }
    }

    public bool Unlock () {
        if (specialState == SpecialState.ALWAYS_LOCKED) {
            return false;
        } else {
            return this.locked = false;
        }
    }

    public bool HasNeighbourWithModule () {
        var grid = MainScreen.INSTANCE.grid;
        var gridSize = MainScreen.INSTANCE.gridSize;

        if (grid[gridPos.x, gridPos.y].specialState == SpecialState.ALWAYS_UNLOCKED) {
            return true;
        }

        if (gridPos.x - 1 >= 0) {
            if (grid[gridPos.x - 1, gridPos.y].hasModule) return true;
        }
        if (gridPos.x + 1 < gridSize.x) {
            if (grid[gridPos.x + 1, gridPos.y].hasModule) return true;
        }
        if (gridPos.y - 1 >= 0) {
            if (grid[gridPos.x, gridPos.y - 1].hasModule) return true;
        }
        if (gridPos.y + 1 < gridSize.y) {
            if (grid[gridPos.x, gridPos.y + 1].hasModule) return true;
        }

        return false;
    }

    public bool HasConnectionToMainScreen () {
        List<ModulePosition> checkedPositions = new List<ModulePosition>();

        Queue<ModulePosition> queue = new Queue<ModulePosition>();
        queue.Enqueue(this);
        checkedPositions.Add(this);

        var grid = MainScreen.INSTANCE.grid;
        var gridSize = MainScreen.INSTANCE.gridSize;

        while (queue.Count > 0) {
            ModulePosition current = queue.Dequeue();

            // Add and check neighbours with modules for checking.
            if (current.gridPos.x - 1 > 0) {
                if (grid[current.gridPos.x - 1, current.gridPos.y].specialState == SpecialState.ALWAYS_LOCKED) return true;
                if (grid[current.gridPos.x - 1, current.gridPos.y].hasModule && !checkedPositions.Contains(grid[current.gridPos.x - 1, current.gridPos.y])) {
                    queue.Enqueue(grid[current.gridPos.x - 1, current.gridPos.y]);
                    checkedPositions.Add(grid[current.gridPos.x - 1, current.gridPos.y]);
                }
            }
            if (current.gridPos.x + 1 < gridSize.x) {
                if (grid[current.gridPos.x + 1, current.gridPos.y].specialState == SpecialState.ALWAYS_LOCKED) return true;
                if (grid[current.gridPos.x + 1, current.gridPos.y].hasModule && !checkedPositions.Contains(grid[current.gridPos.x + 1, current.gridPos.y])) {
                    queue.Enqueue(grid[current.gridPos.x + 1, current.gridPos.y]);
                    checkedPositions.Add(grid[current.gridPos.x + 1, current.gridPos.y]);
                }
            }
            if (current.gridPos.y - 1 > 0) {
                if (grid[current.gridPos.x, current.gridPos.y - 1].specialState == SpecialState.ALWAYS_LOCKED) return true;
                if (grid[current.gridPos.x, current.gridPos.y - 1].hasModule && !checkedPositions.Contains(grid[current.gridPos.x, current.gridPos.y - 1])) {
                    queue.Enqueue(grid[current.gridPos.x, current.gridPos.y - 1]);
                    checkedPositions.Add(grid[current.gridPos.x, current.gridPos.y - 1]);
                }
            }
            if (current.gridPos.y + 1 < gridSize.y) {
                if (grid[current.gridPos.x, current.gridPos.y + 1].specialState == SpecialState.ALWAYS_LOCKED) return true;
                if (grid[current.gridPos.x, current.gridPos.y + 1].hasModule && !checkedPositions.Contains(grid[current.gridPos.x, current.gridPos.y + 1])) {
                    queue.Enqueue(grid[current.gridPos.x, current.gridPos.y + 1]);
                    checkedPositions.Add(grid[current.gridPos.x, current.gridPos.y + 1]);
                }
            }
        }

        return false;
    }

    public static Vector2Int GlobalToGridPosition ( Vector3 position ) {
        int x = (int)((position.x + MainScreen.INSTANCE.gridSize.x) / 2f);
        int y = (int)((position.y + MainScreen.INSTANCE.gridSize.y) / 2f);
        return new Vector2Int(x, y);
    }

    public static Vector3 GridToGlobalPosition ( Vector2Int gridPosition ) {
        return new Vector3(gridPosition.x * 2f + 1, gridPosition.y * 2f + 1, 0) -
            new Vector3(MainScreen.INSTANCE.gridSize.x, MainScreen.INSTANCE.gridSize.y, 0);
    }
}