using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestManager : SingletonBehaviour<QuestManager>
{
    private Quest currentQuest;
    private List<Quest> activeQuests = new ();
    private UI_Quest currentUi_Quest;
    [SerializeField] private RectTransform ui_QuestParent;
    [SerializeField] private UI_Quest ui_QuestPrefab;
    public Transform treesLocation;
    public Transform chmcken;
    public Transform wire;
    [SerializeField] private Transform treasureLocation;
    private List<Marker> markers = new();
    [SerializeField] private Image markerPref;

    private void Start()
    {
        bool scope = true;
        if (scope)
        {
            List<QuestObjective> objectives = new();
            LocationObjective findTreasureObjective = new(treesLocation, 7.5f, "Trees Location", "Find The Trees My Guy");
            objectives.Add(findTreasureObjective);


            if (ItemDataBase.GetItem("Pine") != null)
            {
                ItemRetrievalObjective getWood = new(ItemDataBase.GetItem("Pine"), 10, "Get Wood", "Retreive 10 Wood My Guy");
                objectives.Add(getWood);
            }
            Quest quest = new("Stranger's Request", "Find the hidden treasure and defeat the boss.", 1, objectives);
            AddQuest(quest);
        }
        if (scope)
        {
            List<QuestObjective> objectives = new();

            if (ItemDataBase.GetItem("Stick") != null)
            {
                ItemRetrievalObjective getStick = new(ItemDataBase.GetItem("Stick"), 2, "Craft Stick", "5 Sticks");
                objectives.Add(getStick);
            }
            if (ItemDataBase.GetItem("Wire") != null)
            {
                ItemRetrievalObjective getWire = new(ItemDataBase.GetItem("Wire"), 1, "Get Wire", "Retreive 1 Wire");
                objectives.Add(getWire);
                LocationObjective location = new(wire, 0.5f, "Wire Location", "Find the wire!");
                objectives.Add(location);
            }
            if (ItemDataBase.GetItem("Feather") != null)
            {
                ItemRetrievalObjective getFeather = new(ItemDataBase.GetItem("Feather"), 1, "Get Feather", "Kill A Chicken And Get Feathers");
                objectives.Add(getFeather);
                LocationObjective location = new(chmcken, 0.8f, "Chimcken Location", "Find The Trees My Guy");
                objectives.Add(location);
            }

            Quest quest = new("Gather Materials", "Find the hidden treasure and defeat the boss.", 1, objectives);
            AddQuest(quest);
        }
    }
    private void Update()
    {
        CheckQuestCompletion();
        CheckLocationObjectives();
    }
    public void GetNewQuest()
    {
        if (currentQuest == null)
        {
            currentQuest = activeQuests[0];
            UI_Quest ui_Quest = Instantiate(ui_QuestPrefab, ui_QuestParent);
            ui_Quest.m_quest = currentQuest;
            ui_Quest.Init();
            currentUi_Quest = ui_Quest;

            foreach (QuestObjective obj in currentQuest.Objectives)
            {
                if (obj is LocationObjective locObj)
                {
                    var objec = new GameObject("Marker").transform;
                    objec.parent = transform;
                    var marker = objec.gameObject.AddComponent<Marker>();
                    marker.target = locObj.TargetLocation.gameObject;
                    marker.markerPrefab = markerPref;
                    markers.Add(marker);
                    obj.OnComplete += () =>
                    {
                        i.markers.Remove(marker);
                        Destroy(marker.target);
                    };
                }
            }
        }
        else if (currentUi_Quest == null)
        {
            currentQuest = activeQuests[0];
            UI_Quest ui_Quest = Instantiate(ui_QuestPrefab, ui_QuestParent);
            ui_Quest.m_quest = currentQuest;
            ui_Quest.Init();
            currentUi_Quest = ui_Quest;
        }
    }
    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
    }
    public void RemoveQuest(Quest quest)
    {
        activeQuests.Remove(quest);
    }
    public static void ItemCollected(Item item, int quantity)
    {
        if(i.currentQuest != null)
        {
            Quest quest = i.currentQuest;
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (objective is ItemRetrievalObjective itemRetrievalObjective)
                {
                    itemRetrievalObjective.ItemCollected(item, quantity);
                }
            }
        }
    }
    public static void CheckLocationObjectives()
    {
        if (i.currentQuest != null)
        {
            Quest quest = i.currentQuest;
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (objective is LocationObjective locationObjective)
                {
                    if (!objective.IsCompleted)
                    {
                        locationObjective.CheckCompletion();
                    }

                }

            }
        }
    }
    public void CheckQuestCompletion()
    {
        List<Quest> completedQuests = new ();

        if (i.currentQuest != null)
        {
            Quest quest = i.currentQuest;
            if (quest.IsCompleted())
            {
                completedQuests.Add(quest);
                currentQuest = null;
                Debug.Log($"Quest '{quest.QuestTitle}' completed!");
                Dave dave = FindObjectOfType<Dave>();
                if(dave != null)
                {
                    dave.dialouges++;
                }
                if (currentUi_Quest != null)
                {
                    Destroy(currentUi_Quest.gameObject);
                    currentUi_Quest = null;
                }
            }
        }

        foreach (Quest completedQuest in completedQuests)
        {
            RemoveQuest(completedQuest);
        }
    }
}
