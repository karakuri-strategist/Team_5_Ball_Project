using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        var facing = target.GetComponent<BallMovement>().facing;

        if (facing.magnitude > 0.0001)
            transform.forward = facing;
    }
}
