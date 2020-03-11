using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardForce = 10f;

    public float sidewayForce = 10f;

    private void Start()
    {
      
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(0, 0, forwardForce);

        if (Input.GetKey("d"))
        {
            rb.AddForce(sidewayForce, 0, 0, ForceMode.VelocityChange);
        }
        if (Input.GetKey("a"))
        {
            rb.AddForce(-sidewayForce, 0, 0, ForceMode.VelocityChange);
        }
    }
}
