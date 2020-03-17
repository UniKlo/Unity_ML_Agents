using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class GroundController : Agent
{
    private int count;
    public GameObject ball;
    private Rigidbody ballRB;
    public GameObject[] pick_up;

    private Quaternion board_rotation;
    private Vector3 ball_position;
    private Vector3 diff;
    private int total_gem = 7;

    public override void InitializeAgent()
    {
        Debug.Log("This is a new game!");
        count = 0;
        ballRB = ball.GetComponent<Rigidbody>();
        board_rotation = transform.rotation;
 
    }

    public override void CollectObservations() //22
    {
        // ground rotation
        AddVectorObs(gameObject.transform.rotation.z); //1
        AddVectorObs(gameObject.transform.rotation.x); //1

        // ball position
        ball_position = ball.transform.position;
        AddVectorObs(ball_position.z); //1
        AddVectorObs(ball_position.x); //1

        // whether pickup is avtive and its relative position to the ball
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

        // how many gem we have left
        //Debug.Log("count:" + count);
        AddVectorObs(total_gem - count); //1

        // ball velocity
        AddVectorObs(ballRB.velocity); // 3
    }

    public override void AgentAction(float[] vectorAction)
    {
        var actionZ = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

        if (ballRB.IsSleeping())
        {
            ballRB.WakeUp();
        }

        // z rotation
        if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
            (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
        }

        // x rotation
        if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
            (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
        }

        // fall off the board
        if( Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 10f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 10f)
        {
            SetReward(-1f);
            Done();
        }
        else
        {
            SetReward(-0.001f);
        }
    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = board_rotation;
        ball.transform.position = gameObject.transform.position + new Vector3(0, 1, 1);
        //Debug.Log("ground position: " + gameObject.transform.position);
        ballRB.velocity = Vector3.zero;
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

    public void HandleOnTriggerEnter(pick_up pick_up_object, Collider other)
    {
        //Debug.Log("I m in parent " + pick_up_object.name + " is picking up by " + other.name);
        if (other.name == "Ball")
        {
            count += 1;
            pick_up_object.gameObject.SetActive(false);
            //Debug.Log("I m getting pick up");
            CheckCount(count);

        }

    }

    void CheckCount(int count)
    {
        AddReward(1f);
        //Debug.Log("score:" + GetCumulativeReward());
        if (count >= 7)
        {
            AddReward(1f);
            Debug.Log("collect" + count + "Last game scored:" + GetCumulativeReward());
            Done();
        }
    }

}