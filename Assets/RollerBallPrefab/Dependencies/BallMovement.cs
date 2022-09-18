using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class BallMovement : MonoBehaviour
{


    public float jumpForce = 150f;
    public Slider slider;
    public float maxSpeed = 20f;
    public float moveForceForward = 150f;
    public float moveForceSide = 150f;
    public float boostMaxSpeed = 30;
    public float boostForceForward = 200f;
    public float boostForceSide = 200f;
    public float speedPadMaxSpeed = 30f;
    public float speedPadForceForward = 200f;
    public float speedPadForceSide = 200f;
    
    [Tooltip("Amount of seconds it takes boost to run out")]
    public float boostSeconds = 5f;
    [Tooltip("Amount of seconds it takes boost to replenish from empty")]
    public float boostReplenishSeconds = 5f;
    private float boostLeft;
    private bool boostPressed = false;
    public float driftScale = 20;
    [Tooltip("Dictates how much velocity is lost to friction while drifting")]
    public float driftLost = 0.2f;
    public ResetHandler resetHandler;

    private bool driftPressed = false;
    private Vector3 driftDirection = Vector3.zero;
    private bool speedPadBoost = false;

    private Vector3 initialPosition;


    private Vector3 _facing;
    private Vector3 initialFacing;
    public Vector3 facing
    {
        get { return this._facing; }
    }

    private Rigidbody body;
    private bool isJumping = false;

    private Vector3 commandedDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        resetHandler.ResetEvent += HandleResetEvent;
        initialPosition = transform.position;
        boostLeft = boostSeconds;
        body = GetComponent<Rigidbody>();
        _facing = transform.forward;
        initialFacing = _facing;
    }

    private float GetForceForward()
    {
        if (speedPadBoost)
            return speedPadForceForward;
        if (boostPressed && boostLeft > 0)
            return boostForceForward;
        return moveForceForward;
    }

    private float GetForceSide()
    {
        if (speedPadBoost)
            return speedPadForceSide;
        if (boostPressed && boostLeft > 0)
            return boostForceSide;
        return moveForceSide;
    }

    private float MaxSpeed()
    {
        if (speedPadBoost)
            return speedPadMaxSpeed;
        if (boostPressed && boostLeft > 0)
            return boostMaxSpeed;
        return maxSpeed;
    }

    private void CheckIfFallen()
    {
        if (transform.position.y < -10)
        {
            resetHandler.Reset();
        }
    }

    public void HandleResetEvent(object sender)
    {
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        body.velocity = initialFacing.normalized;
        body.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.forward = initialFacing;
        _facing = initialFacing;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfFallen();
        CalculateBoost();
        MovePlayer();

        if (commandedDirection.y > 0)
            commandedDirection.y = 0;

        if (Input.GetKeyDown(KeyCode.R))
        {
            resetHandler.Reset();

        }
    }

    private void CalculateBoost()
    {
        if (boostPressed)
        {
            if (boostLeft > 0)
                boostLeft -= Time.deltaTime;
            else
                boostLeft = 0;
        }
        else
        {
            if (boostLeft < boostSeconds)
                boostLeft += Time.deltaTime * boostSeconds / boostReplenishSeconds;
            if (boostLeft > boostSeconds)
                boostLeft = boostSeconds;
        }
        slider.value = boostLeft / boostSeconds;
    }

    private void MovePlayer()
    {
        var forwardComponent = facing * (commandedDirection.z);
        var lateralComponent = Vector3.Cross(facing, Vector3.up) * -commandedDirection.x;
        var verticalComponent = new Vector3(0f, commandedDirection.y, 0f);
        if (commandedDirection == Vector3.zero && speedPadBoost)
        {
            forwardComponent = facing;
        }
        var force =
            (forwardComponent * GetForceForward()
            + lateralComponent * GetForceSide()
            + verticalComponent * jumpForce) * Time.deltaTime;
        if(driftPressed)
        {
            var driftForce = Vector3.Cross(facing, Vector3.up) * -driftDirection.x * body.velocity.magnitude * driftScale * Time.deltaTime;
            driftForce -= facing * body.velocity.magnitude * driftScale * Time.deltaTime;
            force += driftForce;
        }


        body.AddForce(force);

        if (body.velocity.magnitude > 0.001)
        {
            _facing.x = body.velocity.x;
            _facing.z = body.velocity.z;
        }
        else
        {
            _facing = transform.forward;
        }
        Vector3 lateralVelocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        if (lateralVelocity.magnitude > MaxSpeed())
            body.velocity = lateralVelocity.normalized * MaxSpeed() + new Vector3(0, body.velocity.y, 0);
    }

    public void Drift(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            driftPressed = true;
            driftDirection = commandedDirection;
        }
        if (ctx.canceled)
        {
            driftPressed = false;
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isJumping)
        {
            isJumping = true;
            commandedDirection.y = 1;
        }
    }

    public void Boost(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            boostPressed = true;
        }
        if (ctx.canceled)
        {
            boostPressed = false;
        }

    }

    public void Move(InputAction.CallbackContext ctx)
    {
        var movement = ctx.ReadValue<Vector2>();
        commandedDirection.x = movement.x;
        commandedDirection.z = movement.y;
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            ResetPlayer();
        }

        Vector3 delta = Vector3.zero;
        List<ContactPoint> list = new List<ContactPoint>();
        col.GetContacts(list);
        // print("Landing: " + col.contactCount);
        for (int i = 0; i < col.contactCount; i++)
        {
            delta += transform.position - list[i].point;
            //print(transform.position + " - " + list[i].point + " " + delta);
        }
        delta /= col.contactCount;
        //Debug.Log("Landing: Done " + delta + " --- " + Mathf.Abs(delta.y));
        if (Mathf.Abs(delta.y) > 0.25)
            isJumping = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "SpeedPad")
        {
            Debug.Log("Colliding");
            speedPadBoost = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "SpeedPad")
        {
            StartCoroutine(SpeedPadBoostExpiration());
        }
    }

    IEnumerator SpeedPadBoostExpiration()
    {
        yield return new WaitForSeconds(1.5f);
        speedPadBoost = false;
    }
}
