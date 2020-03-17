using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerController : Agent
{
    private Rigidbody rb;
    public float speed;
    private int count;
    public GameObject ground;
    public GameObject[] pick_up;
    public GameObject[] prefabs;

    private Vector3 ball_position;
    private Vector3 diff;
    private int total_gem = 7;

    // private float add_part;

    //IFloatProperties m_ResetParams;

    public override void InitializeAgent()
    {
        Debug.Log("This is a new game!");
        count = 0;
        rb = GetComponent<Rigidbody>();

    }

    public override void CollectObservations() //22
    {
        // player rotation
        AddVectorObs(gameObject.transform.rotation.z); //1
        AddVectorObs(gameObject.transform.rotation.x); //1

        // player position
        ball_position = gameObject.transform.position;
        AddVectorObs(ball_position.z); //1
        AddVectorObs(ball_position.x); //1

        // whether pickup is avtive and its relative position to the player
        foreach (GameObject go in pick_up) //14 = 2 * 7
        {
            //AddVectorObs(go.activeSelf);
            if (go.activeSelf == true)
                diff = ball_position - go.transform.position;
            else
                diff = Vector3.zero;
            AddVectorObs(diff.x);
            AddVectorObs(diff.z);
        }
        //Debug.Log("count:" + count);
        AddVectorObs(total_gem - count); //1
        // player velocity
        AddVectorObs(rb.velocity); // 3
    }

    public override void AgentAction(float[] vectorAction)
    {
        var actionZ = Mathf.Clamp(vectorAction[0], -1f, 1f);
        var actionX = Mathf.Clamp(vectorAction[1], -1f, 1f);

        rb.AddForce(Vector3.forward * speed * actionZ);

        rb.AddForce(Vector3.right * speed * actionX);

        AddReward(-0.001f);

    }

    public override void AgentReset()
    {

        gameObject.transform.position = ground.transform.position + new Vector3(0, 1, 1);
        //Debug.Log("ground position: " + gameObject.transform.position);
        rb.velocity = Vector3.zero;
        count = 0;

        foreach (GameObject go in pick_up)
            go.SetActive(true);

    }
    // call before performing any physics
    public override float[] Heuristic()
    {
        var action = new float[2];
        float MoveVertical = Input.GetAxisRaw("Vertical");
        float MoveHorizontal = Input.GetAxisRaw("Horizontal");
        action[0] = MoveVertical;
        action[1] = MoveHorizontal;

        return action;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            count += 1;
            other.gameObject.SetActive(false);
            CheckCount(count);

        }

    }
    void CheckCount(int count)
    {
        // add_part = count - GetCumulativeReward();
        //Debug.Log("This is total"+ GetCumulativeReward() + " Part: " + add_part);
        AddReward(1f);

        //Debug.Log("score:" + GetCumulativeReward());
        if (count >= 7)
        {
            AddReward(1f);
            Debug.Log("collect" + count + "Last game scored:" + GetCumulativeReward());
            Done();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.001f);
            //Debug.Log("I hit the wall");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.001f);
            //Debug.Log("I am with the wall");
        }

    }
}