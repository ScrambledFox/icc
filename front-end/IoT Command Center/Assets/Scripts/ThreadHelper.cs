using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ThreadHelper : MonoBehaviour {

    public static ThreadHelper INSTANCE = null;

    private static Queue<Action> queuedActions = new Queue<Action>();

    public static void AddToQueue ( Action action ) {
        if (action == null)
            return;

        lock (queuedActions) {
            queuedActions.Enqueue(action);
        }
    }

    private void Update () {
        if (queuedActions.Count > 0) {
            Action action;
            lock (queuedActions) {
                action = queuedActions.Dequeue();
            }

            action.Invoke();
        }
    }

}
