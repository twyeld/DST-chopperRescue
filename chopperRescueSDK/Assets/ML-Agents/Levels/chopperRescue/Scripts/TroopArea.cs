using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TroopArea : Area
{
    public GameObject troop;
    public GameObject enemy;
    public int numTroops;
    public int numenemys;
    public bool respawnTroops;
    public float range;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreatTroop(int numTrop, GameObject troopType)
    {
        for (int i = 0; i < numTrop; i++)
        {
            GameObject trop = Instantiate(troopType, new Vector3(Random.Range(-range, range), 1f,
                                                              Random.Range(-range, range)) + transform.position,
                                          Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            trop.GetComponent<TroopLogic>().respawn = respawnTroops;
            trop.GetComponent<TroopLogic>().myArea = this;
        }
    }

    public void ResetTroopArea(Agent[] agents)
    {
        foreach (Agent agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                                                       Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreatTroop(numTroops, troop);
        CreatTroop(numenemys, enemy);
    }

    public override void ResetArea()
    {
    }
}
