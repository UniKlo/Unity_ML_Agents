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
    public Text countText;
    public Text winText;

    public GameObject[] pick_up;
    public GameObject[] prefabs;


    //IFloatProperties m_ResetParams;

    public override void InitializeAgent()
    {
        Debug.Log("This is a new game!");
        count = 0;
        SetCountText();
        winText.text = "";
        rb = GetComponent<Rigidbody>();
        //prefabs = Resources.LoadAll<GameObject>("prefabs");
        //m_ResetParams = Academy.Instance.FloatProperties;

    }

    public override void CollectObservations() //28
    {
        AddVectorObs(gameObject.transform.rotation.z); //1
        AddVectorObs(gameObject.transform.rotation.x); //1
        AddVectorObs(gameObject.transform.position.z); //1
        AddVectorObs(gameObject.transform.position.x); //1
        foreach (GameObject go in pick_up) // 21 = 3 * 7 
            AddVectorObs(go.transform.position);
        AddVectorObs(rb.velocity); // 3
    }

    public override void AgentAction(float[] vectorAction)
    {
        var actionZ = Mathf.Clamp(vectorAction[0], -1f, 1f);
        var actionX = Mathf.Clamp(vectorAction[1], -1f, 1f);

        rb.AddForce(Vector3.forward * speed * actionZ);

        rb.AddForce(Vector3.right * speed * actionX);

        SetReward(-0.005f);
        Debug.Log("score:" + GetCumulativeReward());
    }

    public override void AgentReset()
    {
        
        gameObject.transform.position = Vector3.zero;
        rb.velocity = Vector3.zero;
        winText.text = "";
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
            SetCountText();
            SetReward(1f);
            
        }
    }
    void SetCountText ()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 7)
        {
            winText.text = "You Win!";
            Debug.Log("Last game scored:" + GetCumulativeReward());
            Done();
        }
    }

}
