using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BallMovement : MonoBehaviour
{
    public float moveForce = 150f;
    public float maxSpeed = 50f;
    public float jumpForce = 150f;
    public Slider slider;

    public float boostForce = 200f;
    public float boostMaxSpeed = 15f;
    [Tooltip("Amount of seconds it takes boost to run out")]
    public float boostSeconds = 5f;
    [Tooltip("Amount of seconds it takes boost to replenish from empty")]
    public float boostReplenishSeconds = 5f;
    private float boostLeft;
    private bool boostPressed = false;

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
        initialPosition = transform.position;
        boostLeft = boostSeconds;
        body = GetComponent<Rigidbody>();
        _facing = transform.forward;
        initialFacing = _facing;
    }

    private float GetForce()
    {
        if (boostPressed && boostLeft > 0)
            return boostForce;
        return moveForce;
    }

    private float MaxSpeed()
    {
        if (boostPressed && boostLeft > 0)
            return boostMaxSpeed;
        return maxSpeed;
    }

    private void CheckIfFallen()
    {
        if(transform.position.y < -10)
        {
            ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        transform.position = initialPosition;
        _facing = initialFacing;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfFallen();
        if(boostPressed)
        {
            if (boostLeft > 0)
                boostLeft -= Time.deltaTime;
            else
                boostLeft = 0;
        }
        else
        {
            if(boostLeft < boostSeconds)
                boostLeft += Time.deltaTime * boostSeconds / boostReplenishSeconds;
            if (boostLeft > boostSeconds)
                boostLeft = boostSeconds;
        }
        slider.value = boostLeft / boostSeconds;
        var forwardComponent = facing * commandedDirection.z;
        var lateralComponent = Vector3.Cross(facing, Vector3.up) * -commandedDirection.x;
        var verticalComponent = new Vector3(0f, commandedDirection.y, 0f);

        var force = (forwardComponent + lateralComponent + verticalComponent) * Time.deltaTime * GetForce();
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
        
        if (body.velocity.magnitude > MaxSpeed())
            body.velocity = body.velocity.normalized * MaxSpeed();

        if (commandedDirection.y > 0)
            commandedDirection.y = 0;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();

        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isJumping)
        {
            isJumping = true;
            commandedDirection.y = jumpForce;
        }
    }

    public void Boost(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            boostPressed = true;
        }
        if(ctx.canceled)
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
        if(col.gameObject.tag == "Obstacle")
        {
            ResetPlayer();
            
        }
        Vector3 delta = Vector3.zero;
        List<ContactPoint> list = new List<ContactPoint>();
        col.GetContacts(list);
       // print("Landing: " + col.contactCount);
        for(int i = 0; i < col.contactCount; i++)
        {
            delta += transform.position - list[i].point;
            //print(transform.position + " - " + list[i].point + " " + delta);
        }
        delta /= col.contactCount;
        //Debug.Log("Landing: Done " + delta + " --- " + Mathf.Abs(delta.y));
        if(Mathf.Abs(delta.y)>0.25)
            isJumping = false;
    }
}
