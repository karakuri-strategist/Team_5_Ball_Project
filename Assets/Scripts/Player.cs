using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if(horizontal != 0)
        {
            body.AddForce(Vector3.right * horizontal);
        }
        if(vertical != 0)
        {
            body.AddForce(Vector3.forward * vertical);
        }
    }
}
