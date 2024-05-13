using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraplingGhostEffect : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject ghost;
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(ghostDelaySeconds > 0)
        {
            ghostDelaySeconds -= Time.deltaTime;
        }
        else
        {
            GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
            ghostDelaySeconds = ghostDelay;
        }
    }
}
