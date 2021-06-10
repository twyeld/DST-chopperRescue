using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class chopperAgent : Agent
{
    private chopperAcademy myAcademy;
    public GameObject area;
    TroopArea myArea;
    bool frozen;
    bool poisioned;
    bool satiated;
    bool shoot;
    float frozenTime;
    float effectTime;
    Rigidbody agentRb;

    public int agentID;
    public int troops;
    public int poisonedCount;
    public int frozenCount;
    public int frozeEnemyCount;

    private float laser_length;
    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public Material normalMaterial;
    public Material enemyMaterial;
    public Material troopMaterial;
    public Material frozenMaterial;
    public GameObject myLaser;
    public bool contribute;
    private RayPerception3D rayPer;
    public bool useVectorObs;  

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myArea = area.GetComponent<TroopArea>();
        rayPer = GetComponent<RayPerception3D>();
        myAcademy = FindObjectOfType<chopperAcademy>();

        SetResetParameters();
    }

    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            float rayDistance = 50f;
            //float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f }; //original
		//float[] rayAngles = { 0f, -15f, -30f, -45f, -60f, -75f, -90f}; //new
		//float[] rayAngles = { 90f, 105f, 120f, 135f, 150f, 165f, 180f}; //new + 90
		//float[] rayAngles = { 180f, 195f, 210f, 225f, 240f, 255f, 270f}; //new +180
		float[] rayAngles = { 135f, 150f, 165f, 180f, 195f, 210f, 225f}; //new +135
            string[] detectableObjects = { "troop", "agent", "wall", "enemy", "frozenAgent" };
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            Vector3 localVelocity = transform.InverseTransformDirection(agentRb.velocity);
            AddVectorObs(localVelocity.x);
            AddVectorObs(localVelocity.z);
            AddVectorObs(System.Convert.ToInt32(frozen));
            AddVectorObs(System.Convert.ToInt32(shoot));
        }
    }

    public Color32 ToColor(int hexVal)
    {
        byte r = (byte)((hexVal >> 16) & 0xFF);
        byte g = (byte)((hexVal >> 8) & 0xFF);
        byte b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    public void MoveAgent(float[] act)
    {
        shoot = false;

        if (Time.time > frozenTime + ChopperSeedValues.instance.FrozenInterval && frozen)
        {
            Unfreeze();
        }
        if (Time.time > effectTime + 0.5f)
        {
            if (poisioned)
            {
                Unpoison();
            }
            if (satiated)
            {
                Unsatiate();
            }
        }

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        if (!frozen)
        {
            bool shootCommand = false;
            if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
            {
                dirToGo = transform.forward * Mathf.Clamp(act[0], -1f, 1f);
                rotateDir = transform.up * Mathf.Clamp(act[1], -1f, 1f);
                shootCommand = Mathf.Clamp(act[2], -1f, 1f) > 0.5f;
            }
            else
            {
                var forwardAxis = (int)act[0];
                var rightAxis = (int)act[1];
                var rotateAxis = (int)act[2];
                var shootAxis = (int)act[3];
                
                switch (forwardAxis)
                {
                    case 1:
                        dirToGo = transform.forward;
                        break;
                    case 2:
                        dirToGo = -transform.forward;
                        break;
                }
                
                switch (rightAxis)
                {
                    case 1:
                        dirToGo = transform.right;
                        break;
                    case 2:
                        dirToGo = -transform.right;
                        break;
                }

                switch (rotateAxis)
                {
                    case 1:
                        rotateDir = -transform.up;
                        break;
                    case 2:
                        rotateDir = transform.up;
                        break; 
                }
                switch (shootAxis)
                {
                    case 1:
                        shootCommand = true;
                        break;
                }
            }
            if (shootCommand)
            {
                shoot = true;
                dirToGo *= 0.5f;
                agentRb.velocity *= 0.75f;
            }
            agentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        }

        if (agentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            agentRb.velocity *= 0.95f;
        }

        if (shoot)
        {
            // before we were using the parameter from the Academy:
            // myLaser.transform.localScale = new Vector3(1f, 1f, laser_length);
            
            // now we are using the parameter controlled by the sliders:
            myLaser.transform.localScale = new Vector3(1f, 1f, ChopperSeedValues.instance.LaserReach);
            
            // TODO: is laser length not ued here for calculations???
            Vector3 position = transform.TransformDirection(RayPerception3D.PolarToCartesian(25f, 180f)); //(25f, 90f)
            Debug.DrawRay(transform.position, position, Color.red, 0f, true);
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 2f, position, out hit, 25f))
            {
                if (hit.collider.gameObject.CompareTag("agent"))
                {
                    hit.collider.gameObject.GetComponent<chopperAgent>().Freeze(agentID);
                    frozeEnemyCount++;
                }
            }
        }
        else
        {
            myLaser.transform.localScale = new Vector3(0f, 0f, 0f);

        }
    }

    void Freeze(int frozenBy, bool log = true)
    {
        var selfLineData = GraphCollection.instance.disabledSelf.lines[agentID];
        var newX1 = selfLineData.points.Count; // x coordinate = num of points already there
        selfLineData.points.Add(new Vector3(newX1, 0, Random.Range(0f, 15f)));
        
        var targetLineData = GraphCollection.instance.disabledTarget.lines[agentID];
        var newX2 = targetLineData.points.Count; // x coordinate = num of points already there
        targetLineData.points.Add(new Vector3(newX2, 0, Random.Range(0f, 10f)));
        
        frozenCount++;
        gameObject.tag = "frozenAgent";
        frozen = true;
        frozenTime = Time.time;
        gameObject.GetComponent<Renderer>().material = frozenMaterial;

        if (log)
            myAcademy.LogEvent(chopperAcademy.EventType.FrozenBy, agentID, frozenBy);
    }


    void Unfreeze(bool log = true)
    {
        frozen = false;
        gameObject.tag = "agent";
        gameObject.GetComponent<Renderer>().material = normalMaterial;

        if (log)
            myAcademy.LogEvent(chopperAcademy.EventType.Unfrozen, agentID);
    }

    void Poison(bool log = true)
    {
        poisonedCount++;
        poisioned = true;
        effectTime = Time.time;
        gameObject.GetComponent<Renderer>().material = enemyMaterial;

        if (log)
            myAcademy.LogEvent(chopperAcademy.EventType.Poisoned, agentID);
    }

    void Unpoison(bool log = true)
    {
        poisioned = false;
        gameObject.GetComponent<Renderer>().material = normalMaterial;

        if (log)
        myAcademy.LogEvent(chopperAcademy.EventType.Unpoisoned, agentID);
    }

    void Satiate()
    {
        satiated = true;
        effectTime = Time.time;
        gameObject.GetComponent<Renderer>().material = troopMaterial;
    }

    void Unsatiate()
    {
        satiated = false;
        gameObject.GetComponent<Renderer>().material = normalMaterial;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
    }

    public override void AgentReset()
    {
        Unfreeze(false);
        Unpoison(false);
        Unsatiate();
        shoot = false;
        agentRb.velocity = Vector3.zero;
        troops = 0;
        poisonedCount = 0;
        frozenCount = 0;
        frozeEnemyCount = 0;
        myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                                         2f, Random.Range(-myArea.range, myArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        SetResetParameters();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("troop"))
        {
            Satiate();
            collision.gameObject.GetComponent<TroopLogic>().OnEaten();
            AddReward(ChopperSeedValues.instance.troopReward);
            var lineData = GraphCollection.instance.redGraph.lines[agentID];
            var newX = lineData.points.Count; // x coordinate = num of points already there
            lineData.points.Add(new Vector3(newX, 0,
                ChopperSeedValues.instance.troopReward * newX * 0.2f + Random.Range(0f, 5f)));
            troops += 1;
            if (contribute)
            {
                myAcademy.totalScore += 1;
            }
        }
        if (collision.gameObject.CompareTag("enemy"))
        {
            Poison();
            collision.gameObject.GetComponent<TroopLogic>().OnEaten();

            AddReward(ChopperSeedValues.instance.enemyReward);
            
            var lineData = GraphCollection.instance.blueGraph.lines[agentID];
            var newX = lineData.points.Count; // x coordinate = num of points already there
            lineData.points.Add(new Vector3(newX, 0,
                ChopperSeedValues.instance.enemyReward * newX * 0.2f - Random.Range(0f, 5f)));
            
            if (contribute)
            {
                myAcademy.totalScore -= 1;
            }
        }
    }

    public override void AgentOnDone()
    {

    }

    public void SetLaserLengths()
    {
        laser_length = myAcademy.resetParameters["laser_length"];
    }

    public void SetAgentScale()
    {
        var agent_scale = myAcademy.resetParameters["agent_scale"];
        gameObject.transform.localScale = new Vector3(agent_scale, agent_scale, agent_scale);
    }
    
    public void SetResetParameters()
    {
        SetLaserLengths();
        SetAgentScale();
    }
}
