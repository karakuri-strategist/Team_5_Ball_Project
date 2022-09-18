using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Tooltip("Adjusts how quickly the camera moves to the direction the player is looking.")]
    private float cameraDamper = 0.1f;
    [SerializeField]
    private Camera camera;

    private float velocityDiff;
    private Vector3 lastVelocity;
    private Rigidbody playerBody;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var newVelocityDiff = (lastVelocity - playerBody.velocity).magnitude;
        if (newVelocityDiff > velocityDiff)
            velocityDiff = newVelocityDiff;
        else
            velocityDiff = Mathf.Lerp(velocityDiff, newVelocityDiff, Time.deltaTime);
        target.GetComponent<Rigidbody>();
        transform.position = target.position;
        var facing = target.GetComponent<BallMovement>().facing;

        if (facing.magnitude > 0.0001)
        {
            transform.forward = Vector3.Lerp(transform.forward, facing, Time.deltaTime * 100 * cameraDamper / velocityDiff);
        }
        camera.fieldOfView = Mathf.Clamp(
            Mathf.Lerp(camera.fieldOfView, playerBody.velocity.magnitude * 3, Time.deltaTime),
            60f,
            80f
        );
    }
}
