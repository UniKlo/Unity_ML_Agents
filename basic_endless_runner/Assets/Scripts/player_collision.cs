using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_collision : MonoBehaviour
{
    public player movement;

    private void OnCollisionEnter(Collision collision)
    {
        movement.enabled &= collision.collider.tag != "obstacle";
    }
}
