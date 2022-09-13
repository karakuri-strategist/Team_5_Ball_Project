using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Tooltip("Adjusts how quickly the camera moves to the direction the player is looking.")]
    private float cameraDamper = 100;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        var amountMoved = (lastPosition - target.position).magnitude;
        var facing = target.GetComponent<BallMovement>().facing;

        if (facing.magnitude > 0.0001)
        {
            transform.forward = Vector3.Lerp(transform.forward, facing, amountMoved * Time.deltaTime * 100);
        }
        lastPosition = target.position;
    }
}
