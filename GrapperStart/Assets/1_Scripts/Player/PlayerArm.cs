using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{
    // Start is called before the first frame update

    Hooking hook;
    void Start()
    {
        hook = FindAnyObjectByType<Hooking>();
    }

    // Update is called once per frame
    public float armSpeed = 3.0f;
    void Update()
    {
        
    }

    public void rotationArm(Vector3 hookdir)
    {
        Vector3 playerdir = hookdir - transform.position;

        float angle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15.0f);
    }
}
