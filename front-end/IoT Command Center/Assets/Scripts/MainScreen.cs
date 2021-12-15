using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainScreen : MonoBehaviour {

    public static MainScreen INSTANCE = null;

    private Transform timeDateText;
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI dateText;

    public Transform modulePrefab;
    public ModulePosition[,] grid;
    public Dictionary<ModulePosition, Module> modulesDictionary = new Dictionary<ModulePosition, Module>();
    public GameObject[] ConnectionPositions;
    public Connection[] Connections;

    public bool FollowCurrentTime = true;
    public int Year, Month, Day, Hour, Minute, Second;
    public float TimeAcceleration = 1.0f;
    public static DateTime CurrentDateTime = DateTime.Now;

    public Vector2Int gridSize = new Vector2Int(11, 12);

    private void Awake () {
        INSTANCE = this;

        CurrentDateTime = FollowCurrentTime ? DateTime.Now : new DateTime(Year, Month, Day, Hour, Minute, Second);

        timeDateText = GameObject.Find("TimeDate").transform;
        timeText = timeDateText.Find("Time").GetComponent<TextMeshProUGUI>();
        dateText = timeDateText.Find("Date").GetComponent<TextMeshProUGUI>();

        grid = new ModulePosition[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++) {
            for (int x = 0; x < gridSize.x; x++) {
                Vector3 pos = new Vector3(x * 2f + 1, y * 2f + 1, 0) - new Vector3(gridSize.x, gridSize.y, 0);
                ModulePosition.SpecialState state = ModulePosition.SpecialState.NONE;
                Connection connection = null;

                if ((Mathf.Abs(pos.x) == 5 && Mathf.Abs(pos.y) < 4) || (Mathf.Abs(pos.y) == 4 && Mathf.Abs(pos.x) < 5)) {
                    state = ModulePosition.SpecialState.ALWAYS_UNLOCKED;
                    connection = GetClosestConnectionGraphic( pos );
                } else if (Mathf.Abs(pos.x) < 5 && Mathf.Abs(pos.y) < 4) {
                    state = ModulePosition.SpecialState.ALWAYS_LOCKED;
                }

                grid[x, y] = new ModulePosition(new Vector2Int(x, y), pos, connection, state);
            }
        }

        UpcomingEventSpawner eventSpawner = FindObjectOfType<UpcomingEventSpawner>();
    }

    private Connection GetClosestConnectionGraphic ( Vector3 pos ) {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < ConnectionPositions.Length; i++) {
            float distance = Vector3.Distance(pos, ConnectionPositions[i].transform.position);
            if ( distance < closestDistance ) {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        if (closestIndex == -1) return null;
        return this.Connections[closestIndex];
    }

    public void CheckModules ( bool connected = false ) {
        Debug.Log("Checking module connections...");

        // Update positions.
        for (int y = 0; y < gridSize.y; y++) {
            for (int x = 0; x < gridSize.x; x++) {

                // If current gridpos has a neighbour with an active module...
                if (grid[x, y].HasNeighbourWithModule() && grid[x, y].HasConnectionToMainScreen()) {
                    // unlock the position.
                    grid[x, y].Unlock();
                } else {
                    // If we don't have a neighbour with an active module.
                    // Check if we have a module here and disconnect it.
                    if (grid[x, y].HasModule) { 
                        modulesDictionary[grid[x, y]].Disconnect();

                    // Lock the gridpos.
                    } else {
                        grid[x, y].Lock();
                    }
                }
            }
        }

        if (connected) {
            // Update module connections..
            Module[] mods = GameObject.FindObjectsOfType<Module>();
            foreach (var mod in mods) {
                ModulePosition pos = mod.CanConnectAny();
                if (pos != null) mod.Connect(pos);
            }
        }
    }

    private void OnDrawGizmos () {
        if (grid == null) return;

        foreach (var modulePos in grid) {
            if (modulePos == null) continue;

            if (modulePos.Locked) {
                Gizmos.color = Color.red;
            } else if (modulePos.HasModule) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawWireCube(modulePos.GlobalPosition, new Vector3(1.95f, 1.95f, 0.2f));
            Gizmos.DrawSphere(modulePos.GlobalPosition, 0.1f);
        }
    }

    private void Update () {
        // Global Time
        if (FollowCurrentTime) {
            CurrentDateTime = DateTime.Now;
        } else {
            CurrentDateTime = CurrentDateTime.AddSeconds(Time.deltaTime * TimeAcceleration);
        }

        // Create modules.
        if (Input.GetKeyDown(KeyCode.M)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            pos.z = 0;
            Instantiate(modulePrefab, pos, Quaternion.Euler(-90, 0, 0));
        }

        // Deleting modules.
        if (Input.GetKeyDown(KeyCode.Delete)) {
            Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            GameObject[] modules = GameObject.FindGameObjectsWithTag("Module");

            float closestDistance = 1f;
            int closestIndex = -1;
            for (int i = 0; i < modules.Length; i++) {
                float distance = Vector3.Distance(mouseLocation, modules[i].transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex == -1) return;

            Module module = modules[closestIndex].GetComponent<Module>();
            if (module.IsConnected) module.Disconnect();
            Destroy(modules[closestIndex]);
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            GameObject.Find("EventBG").transform.localPosition = new Vector3(0, 0, 0);

            foreach (var modPair in modulesDictionary) {
                if (modPair.Key.Connection != null) {
                    modPair.Key.Connection.SetFlow(true);
                }
            }
        }

        // Update Time
        timeText.text = CurrentDateTime.ToString("t");
        dateText.text = CurrentDateTime.ToString("m") + Environment.NewLine + CurrentDateTime.Year.ToString();
    }

}
