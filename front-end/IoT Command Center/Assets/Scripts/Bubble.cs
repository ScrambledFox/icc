using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
    
    private Vector3 source;
    private Vector3 target;

    private bool active = false;
    private float startSize = 1.0f;
    private float endSize = 1.0f;
    private float speed = 10.0f;

    private float maxDistance;
    private float time = 0;

    public void Go (Vector3 source, Vector3 target, float minSize, float maxSize, float speed) {
        this.source = source;
        this.target = target;

        this.startSize = Random.Range(minSize, maxSize);
        this.endSize = minSize;
        this.speed = speed;

        maxDistance = Vector3.Distance(source, target);

        this.active = true;
    }

    void Update() {
        if (active) {
            time += Time.deltaTime / maxDistance * speed;
            
            Vector3 newPos = Vector3.Lerp(source, target, time);
            Vector3 newScale = Vector3.Lerp(Vector3.one * startSize, Vector3.one * endSize, time);
            
            transform.position = newPos;
            transform.localScale = newScale;

            if (time >= 1) {
                Destroy(this.gameObject);
            }
        }
    }
}
