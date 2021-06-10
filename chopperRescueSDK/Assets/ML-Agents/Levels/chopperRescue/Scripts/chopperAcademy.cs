using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using System;
using System.IO;

public class chopperAcademy : Academy
{
    [HideInInspector]
    public chopperAgent[] agents;
    [HideInInspector]
    public TroopArea[] listArea;

    public GameObject barPrefab;

    public RectTransform troopsCollectedRegion;
    public RectTransform poisonedCollectedRegion;
    public RectTransform selfDisabledRegion;
    public RectTransform othersDisabledRegion;

    public int totalScore;
    public Text scoreText;

    void Start()
    {
        ClearEventLog();
    }

    public override void AcademyReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("troop"));
        ClearObjects(GameObject.FindGameObjectsWithTag("enemy"));

        agents = GameObject.FindObjectsOfType<chopperAgent>();

        // update agent ID
        int index = 0;
        foreach (var agent in agents)
            agent.agentID = index++;

        listArea = FindObjectsOfType<TroopArea>();
        foreach (TroopArea ba in listArea)
        {
            ba.ResetTroopArea(agents);
        }

        totalScore = 0;

        float barWidth = troopsCollectedRegion.rect.width / agents.Length;

        // reset troops
        foreach (RectTransform child in troopsCollectedRegion)
            Destroy(child.gameObject);
        index = 0;
        foreach (var agent in agents)
        {
            var go = Instantiate(barPrefab, troopsCollectedRegion);
            go.transform.localPosition = Vector3.right * (index * barWidth + barWidth * 0.5f);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, 0);
            go.GetComponent<Image>().color = agent.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color;
            index++;
        }

        // reset poisoned
        foreach (RectTransform child in poisonedCollectedRegion)
            Destroy(child.gameObject);
        index = 0;
        foreach (var agent in agents)
        {
            var go = Instantiate(barPrefab, poisonedCollectedRegion);
            go.transform.localPosition = Vector3.right * (index * barWidth + barWidth * 0.5f);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, 0);
            go.GetComponent<Image>().color = agent.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color;
            index++;
        }

        // reset self disabled
        foreach (RectTransform child in selfDisabledRegion)
            Destroy(child.gameObject);
        index = 0;
        foreach (var agent in agents)
        {
            var go = Instantiate(barPrefab, selfDisabledRegion);
            go.transform.localPosition = Vector3.right * (index * barWidth + barWidth * 0.5f);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, 0);
            go.GetComponent<Image>().color = agent.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color;
            index++;
        }

        // reset others disabled
        foreach (RectTransform child in othersDisabledRegion)
            Destroy(child.gameObject);
        index = 0;
        foreach (var agent in agents)
        {
            var go = Instantiate(barPrefab, othersDisabledRegion);
            go.transform.localPosition = Vector3.right * (index * barWidth + barWidth * 0.5f);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, 0);
            go.GetComponent<Image>().color = agent.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color;
            index++;
        }
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (GameObject trop in objects)
        {
            Destroy(trop);
        }
    }

    // simple FPS tracking
    float frame = 0;
    float fpsTimer = 0;
    float fps = 0;

    void Update()
    {
        // calculate simple FPS
        //frame += 1;
        //fpsTimer += Time.unscaledDeltaTime;
        //if (fpsTimer >= 1)
        //{
        //    fps = frame / fpsTimer;
        //    frame -= fps;
        //    fpsTimer -= 1;
        //}

        // gather highest scorers for bar graph scaling
        int poisonedMax = 0;
        int troopMax = 0;
        int frozenMax = 0;
        int frozenByMax = 0;
        foreach (var agent in agents)
        {
            poisonedMax = Mathf.Max(poisonedMax, agent.poisonedCount);
            troopMax = Mathf.Max(troopMax, agent.troops);
            frozenMax = Mathf.Max(frozenMax, agent.frozenCount);
            frozenByMax = Mathf.Max(frozenByMax, agent.frozeEnemyCount);
        }

        float barWidth = troopsCollectedRegion.rect.width / agents.Length;

        // update troops
        int index = 0;
        foreach (var agent in agents)
        {
            float barHeight = troopsCollectedRegion.rect.height / troopMax * agent.troops;
            RectTransform t = (troopsCollectedRegion.transform.GetChild(index) as RectTransform);

            t.sizeDelta = new Vector2(barWidth, barHeight);
            t.GetChild(0).GetComponent<Text>().text = agent.troops.ToString();

            index++;
        }

        // update poisoned
        index = 0;
        foreach (var agent in agents)
        {
            float barHeight = poisonedCollectedRegion.rect.height / poisonedMax * agent.poisonedCount;
            RectTransform t = (poisonedCollectedRegion.transform.GetChild(index) as RectTransform);

            t.sizeDelta = new Vector2(barWidth, barHeight);
            t.GetChild(0).GetComponent<Text>().text = agent.poisonedCount.ToString();

            index++;
        }

        // update disabled
        index = 0;
        foreach (var agent in agents)
        {
            float barHeight = selfDisabledRegion.rect.height / frozenMax * agent.frozenCount;
            RectTransform t = (selfDisabledRegion.transform.GetChild(index) as RectTransform);

            t.sizeDelta = new Vector2(barWidth, barHeight);
            t.GetChild(0).GetComponent<Text>().text = agent.frozenCount.ToString();

            index++;
        }

        // update others disabled
        index = 0;
        foreach (var agent in agents)
        {
            float barHeight = othersDisabledRegion.rect.height / frozenByMax * agent.frozeEnemyCount;
            RectTransform t = (othersDisabledRegion.transform.GetChild(index) as RectTransform);

            t.sizeDelta = new Vector2(barWidth, barHeight);
            t.GetChild(0).GetComponent<Text>().text = agent.frozeEnemyCount.ToString();

            index++;
        }
    }

   // public override void AcademyStep()
    //{
    //    scoreText.text = string.Format(@"Total Engagement: {0}", totalScore) + "\n" + string.Format(@"FPS: {0}", Mathf.FloorToInt(fps));
    //}

    public enum EventType
    {
        Poisoned,
        Unpoisoned,
        FrozenBy,
        Unfrozen,
        Collected
    }

    public struct Event
    {
        public DateTime timestamp;
        public EventType type;
        public int agent;
        public int targetID;

        public override string ToString()
        {
            return timestamp.ToString("MM/dd/yyyy HH:mm:ss:ffff") + "," + agent + "," + type.ToString() + "," + targetID;
        }
    }

    List<Event> loggedEvents = new List<Event>();

    public string logFilePath = "home/twyeld/temp/chopperlog.csv";

    public void LogEvent(EventType type, int agent, int targetID = -1, bool appendToFile = false)
    {
        Event e = new Event();
        e.timestamp = DateTime.Now;
        e.type = type;
        e.agent = agent;
        e.targetID = targetID;
        loggedEvents.Add(e);
        
        if (appendToFile)
        {
            if (!File.Exists(logFilePath))
            {
                using (StreamWriter log = File.CreateText(logFilePath))
                {
                    log.WriteLine("TimeStamp,AgentID,EventType,OtherID");
                }
            }

            using (StreamWriter log = File.AppendText(logFilePath))
                log.WriteLine(e.ToString());
        }
    }

    public void ClearEventLog(bool clearLogFile = true)
    {
        loggedEvents.Clear();

        if (clearLogFile &&
            File.Exists(logFilePath))
            File.Delete(logFilePath);
    }

    public void DumpEventLog()
    {
        if (File.Exists(logFilePath))
            File.Delete(logFilePath);

        using (StreamWriter log = File.CreateText(logFilePath))
        {
            log.WriteLine("TimeStamp,AgentID,EventType,OtherID");

            foreach (Event e in loggedEvents)
                log.WriteLine(e.ToString());
        }
    }
}
