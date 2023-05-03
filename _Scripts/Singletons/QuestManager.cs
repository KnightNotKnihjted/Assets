using System.Collections.Generic;
using UnityEngine;

public class QuestManager : SingletonBehaviour<QuestManager>
{
    private List<Quest> activeQuests = new ();
    [SerializeField] private RectTransform ui_QuestParent;
    [SerializeField] private UI_Quest ui_QuestPrefab;

    [SerializeField] private Transform treasureLocation;

    private void Start()
    {
        List<QuestObjective> objectives = new ();
        LocationObjective findTreasureObjective = new (treasureLocation, 1f, "Treasure Hunt!", "Find the hidden treasure.");
        objectives.Add(findTreasureObjective);

        if (ItemDataBase.GetItem("Pine") != null)
        {
            ItemRetrievalObjective getFifteenWood = new (ItemDataBase.GetItem("Pine"), 15, "Get Wood", "Retreive 15 Wood My Guy");
            objectives.Add(getFifteenWood);
        }
        Quest treasureHuntQuest = new ("Treasure Hunt", "Find the hidden treasure and defeat the boss.", 1, objectives);
        AddQuest(treasureHuntQuest);
    }
    private void Update()
    {
        CheckQuestCompletion();
        CheckLocationObjectives();
    }
    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);

        UI_Quest ui_Quest = Instantiate(ui_QuestPrefab, ui_QuestParent);
        ui_Quest.m_quest = quest;
        ui_Quest.Init();
    }
    public void RemoveQuest(Quest quest)
    {
        activeQuests.Remove(quest);
    }
    public static void ItemCollected(Item item, int quantity)
    {
        foreach (Quest quest in i.activeQuests)
        {
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (objective is ItemRetrievalObjective itemRetrievalObjective)
                {
                    itemRetrievalObjective.ItemCollected(item, quantity);
                }
            }
        }
    }
    public void CheckLocationObjectives()
    {
        foreach (Quest quest in i.activeQuests)
        {
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (objective is LocationObjective locationObjective)
                {
                    locationObjective.CheckCompletion();
                }
            }
        }
    }
    public void CheckQuestCompletion()
    {
        List<Quest> completedQuests = new ();

        foreach (Quest quest in i.activeQuests)
        {
            if (quest.IsCompleted())
            {
                completedQuests.Add(quest);
                Debug.Log($"Quest '{quest.QuestTitle}' completed!");
            }
        }

        foreach (Quest completedQuest in completedQuests)
        {
            RemoveQuest(completedQuest);
        }
    }
}
