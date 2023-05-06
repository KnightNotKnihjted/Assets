using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestManager : SingletonBehaviour<QuestManager>
{
    private Quest currentQuest;
    private List<Quest> activeQuests = new ();
    [SerializeField] private RectTransform ui_QuestParent;
    [SerializeField] private UI_Quest ui_QuestPrefab;
    [SerializeField] private Transform treesLocation;
    [SerializeField] private Transform treasureLocation;
    [SerializeField] private List<Marker> markers = new();
    [SerializeField] private Image markerPref;

    private void Start()
    {
        bool scope = true;
        if (scope)
        {
            List<QuestObjective> objectives = new();
            LocationObjective findTreasureObjective = new(treesLocation, 7.5f, "Treasure Hunt!", "Find the hidden treasure.");
            objectives.Add(findTreasureObjective);


            if (ItemDataBase.GetItem("Pine") != null)
            {
                ItemRetrievalObjective getWood = new(ItemDataBase.GetItem("Pine"), 10, "Get Wood", "Retreive 10 Wood My Guy");
                objectives.Add(getWood);
            }
            Quest treasureHuntQuest = new("Strange Request", "Find the hidden treasure and defeat the boss.", 1, objectives);
            AddQuest(treasureHuntQuest);
        }
        if (scope)
        {
            List<QuestObjective> objectives = new();
            LocationObjective findTreasureObjective = new(treasureLocation, 1f, "Treasure Hunt!", "Find the hidden treasure.");
            objectives.Add(findTreasureObjective);


            if (ItemDataBase.GetItem("Pine") != null)
            {
                ItemRetrievalObjective getWood = new(ItemDataBase.GetItem("Pine"), 20, "Get More Wood", "Retreive 20 Wood My Guy");
                objectives.Add(getWood);
            }
            Quest treasureHuntQuest = new("Request", "Find the hidden treasure and defeat the boss.", 1, objectives);
            AddQuest(treasureHuntQuest);
        }
    }
    private void Update()
    {
        CheckQuestCompletion();
        CheckLocationObjectives();
    }
    public void GetNewQuest()
    {
        if(currentQuest == null)
        {
            currentQuest = activeQuests[0];
            UI_Quest ui_Quest = Instantiate(ui_QuestPrefab, ui_QuestParent);
            ui_Quest.m_quest = currentQuest;
            ui_Quest.Init();

            foreach (QuestObjective obj in currentQuest.Objectives)
            {
                if (obj is LocationObjective locObj)
                {
                    var marker = gameObject.AddComponent<Marker>();
                    marker.target = locObj.TargetLocation.gameObject;
                    marker.markerPrefab = markerPref;
                    markers.Add(marker);
                }
            }
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
                    locationObjective.CheckCompletion();

                    if (objective.IsCompleted)
                    {
                        foreach(Marker marker in i.markers)
                        {
                            if(marker.target == locationObjective.TargetLocation.gameObject)
                            {
                                i.markers.Remove(marker);
                                Destroy(marker.target);
                            }
                        }
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
            }
        }

        foreach (Quest completedQuest in completedQuests)
        {
            RemoveQuest(completedQuest);
        }
    }
}
