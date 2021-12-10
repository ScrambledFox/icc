using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour {

    private Vector3 source;
    private Vector3 target;

    private bool active = false;
    private float speed = 15.0f;

    public float Size = 1f;
    public float SizeVariation = 0.25f;
    public float Frequency = 1.5f;
    public float FrequencyVariation = 0.5f;

    private float timer;

    private bool lastOffsetSide = false;

    public Transform targetTransform;
    public GameObject Bubble;

    public float SideOffset = 7.5f;
    public float MaxRandomOffset = 3.0f;

    private void Awake () {
        this.source = transform.parent.position;
        this.target = targetTransform.position;
    }

    public void SetActive ( bool state ) {
        this.active = state;
        
        if (state) {
            timer = 0;
        } else {
            timer = -1;
        }
    }

    private void Update () {
        if (Frequency == 0) {
            return;
        }

        if (active) {
            if (timer > 0) {
                timer -= Time.deltaTime;
            } else {
                Vector3 startSource = source + (source - target).normalized * 15f;
                Vector3 sideways = Vector3.Cross(transform.forward, startSource);

                float offset = SideOffset * (lastOffsetSide ? 1 : -1);
                float randomOffset = Random.Range(-MaxRandomOffset, MaxRandomOffset);
                startSource += sideways.normalized * (offset + randomOffset);
                lastOffsetSide = !lastOffsetSide;

                GameObject bubble = Instantiate(Bubble, startSource, Quaternion.identity, this.transform);
                bubble.GetComponent<Bubble>().Go(startSource, target + sideways.normalized * (offset * 0.25f), Mathf.Max(0, Size - SizeVariation), Mathf.Max(0, Size + SizeVariation), speed);
                timer = 1 / Mathf.Max(0.01f, (Frequency + Random.Range(-FrequencyVariation, FrequencyVariation)));
            }
        }
    }



}
