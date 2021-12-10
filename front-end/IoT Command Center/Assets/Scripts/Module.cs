using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

public class Module : MonoBehaviour {

    private ModulePosition modPos = null;
    private ModuleMovement movement;
    private AudioSource audioSource;
    private ModuleDial dial;

    public Light ledPrefab;
    private Light[] leds;

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
        this.dial = this.GetComponentInChildren<ModuleDial>();

        this.label = "Module_" + this.gameObject.GetInstanceID();

        leds = new Light[30];
        for (int i = 0; i < leds.Length; i++) {
            float ratio = i / (float)leds.Length;
            float circleRatio = ratio * Mathf.PI * 2 + (Mathf.PI / 2f);
            Vector3 pos = this.transform.position + new Vector3(Mathf.Cos(circleRatio) * 0.75f, Mathf.Sin(circleRatio) * 0.75f , 0.3f);

            leds[i] = Instantiate(ledPrefab, pos, Quaternion.identity, transform).GetComponent<Light>();
            
            leds[i].color = ratio <= dial.Value ? new Color(ratio, 0, ratio) : Color.black;
        }
    }

    public void Connect ( ModulePosition modPos ) {
        this.modPos = modPos;

        modPos.HasModule = true;
        this.isConnected = true;

        //this.modPos.Connection?.SetAlert("notification");
        this.modPos.Connection?.SetFlow(true);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(attachSound);

        var modulesDictionary = MainScreen.INSTANCE.modulesDictionary;
        if (!modulesDictionary.ContainsKey(this.modPos))
            modulesDictionary.Add(this.modPos, this);
        else {
            modulesDictionary[this.modPos] = this;
        }

        Debug.Log($"Module {this.GetInstanceID()} connected.");

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

        this.modPos.Connection?.ResetAlert();
        this.modPos.Connection?.SetFlow(false);

        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(detachSound);

        this.isConnected = false;
        this.modPos = null;

        Debug.Log($"Module {this.GetInstanceID()} disconnected.");

        MainScreen.INSTANCE.CheckModules();
    }

    private void Update () {
        if (this.modPos != null) {
            this.modPos.Connection.Flow.Size = dial.Value * 1.5f;
            this.modPos.Connection.Flow.SizeVariation = dial.Value * 0.75f;
            this.modPos.Connection.Flow.Frequency = dial.Value * 2;
            this.modPos.Connection.Flow.FrequencyVariation = dial.Value;

            if (dial.PushValue < 0.1f) {
                if (!this.modPos.Connection.HasAlert) {
                    
                    this.modPos.Connection.SetAlert("pushvalue");
                }
            } else if (dial.PushValue > 0.9f) {
                if (this.modPos.Connection.HasAlert) {
                    this.modPos.Connection.ResetAlert();
                }
            }
        }

        for (int i = 0; i < leds.Length; i++) {
            float ratio = i / (float)leds.Length;
            leds[i].color = ratio <= dial.Value ? new Color(ratio, 0, ratio) : Color.black;
        }
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
    private Connection connection;

    public bool Locked { get => locked; }
    public bool HasModule {
        get => hasModule; set {
            this.hasModule = value;
            this.connection?.SetModule(value);
        }
    }
    public Vector2Int GridPosition { get => gridPos; }
    public Vector3 GlobalPosition { get => globalPosition; }
    public SpecialState State { get => specialState; }
    public Connection Connection { get => connection; }

    public ModulePosition ( Vector2Int gridPos, Vector3 position, Connection connectionGraphic = null, SpecialState specialState = SpecialState.NONE ) {
        this.gridPos = gridPos;
        this.globalPosition = position;
        this.hasModule = false;
        this.specialState = specialState;
        this.connection = connectionGraphic;

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