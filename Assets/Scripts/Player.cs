using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public Vector3 forward = new Vector3(-1, 0, 0);
    [SerializeField]
    private float maxVelocity = 16;
    [SerializeField]
    private float forwardAcceleration = 1;
    [SerializeField]
    private float sideAccelerationScale = 1;
    Rigidbody body;
    public  float forwardVelocity;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log("velocity " + body.velocity);
        var cross = Vector3.Cross(body.velocity, new Vector3(1, 0, 1));
        Debug.Log("dot " + Vector3.Dot(new Vector3(1, 0, 1), body.velocity));
        // Check if there is XZ-plane velocity then update forward direction
        // The 1E-5 check is because there seems to be a bug with the Dot product
        // that causes it to return a very small number when it should return zero.
        // The small number seems to always be in the range of 1E-6.
        if (new Vector3(body.velocity.x, 0, body.velocity.z).magnitude > 1E-5)
        {
            // Forward is only the xz-plane direction
            // We are excluding the vertical velocity caused by gravity
            // Or by jumps, ramps, etc.
            forward = new Vector3(body.velocity.x, 0, body.velocity.z);
            forward.Normalize();
        }
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        // apply forward force if player is pressing forward.
        if (vertical != 0)
        {
            body.AddForce(forward * vertical * forwardAcceleration);
        }
        // Turn when player presses right and left.
        if (horizontal != 0)
        {
            // We want the horitontal force to be proportional to the forward velocity.
            // This causes the turning to feel consistent regardless of what speed you are going.
            // Think of turning in a car; turning the wheels applies a centripital force to the car,
            // And that force comes from the friction of the turned wheel + your current velocity.
            // As long as the wheel has enough grip, you wont skid and the car will turn as you expect.
            // To do this, we take the dot product of the velocity with the forward direction
            // (which is a unit vector in the xz plane). This gives us the magnitude of the velocity
            // in the xz plane. Look up "dot product" to learn if its unfamiliar!
            // to get the "side direction", i.e. the direction orthogonal to the current forward direction,
            // you take the cross product with a vector which excludes the plane you care about,
            // In this case the xz plane. I use down, -y, because that will give me the "right direction"
            // orthogonal vector, and Input.GetAxis("Horizontal") returns a positive value when the player
            // is pressing right, and negative if player is pressing left.
            // Look up "right hand rule cross product" to understand this.
            // If I used +y, I would get an orthogonal vector pointing in the "left" direction.
            // A lot of text to explain 3 lines of code!
            var forwardSpeed = Vector3.Dot(forward, body.velocity);
            var sideDirection = Vector3.Cross(forward, new Vector3(0, -1, 0)).normalized;
            body.AddForce(sideDirection * forwardSpeed * sideAccelerationScale * horizontal);
        }
        forwardVelocity = body.velocity.magnitude;
    }
}
