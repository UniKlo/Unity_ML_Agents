using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pick_up : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("I am a child :" + this.name);
        transform.parent.gameObject.GetComponentInParent<GroundController>().HandleOnTriggerEnter(this, other);
    }
}
