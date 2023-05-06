using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestObjective
{
    public string ObjectiveName { get; set; }
    public string ObjectiveDescription { get; set; }
    public bool IsCompleted { get; set; }
    public Action OnComplete { get; set; }
    public bool HasClaimed { get; set; }

    protected QuestObjective(string name, string description)
    {
        ObjectiveName = name;
        ObjectiveDescription = description;
        IsCompleted = false;
    }

    public abstract void CheckCompletion();
}

public class LocationObjective : QuestObjective
{
    public Transform TargetLocation { get; private set; }
    public float CompletionDistance { get; private set; }

    public LocationObjective(Transform targetLocation, float completionDistance, string name, string description) : base(name, description)
    {
        TargetLocation = targetLocation;
        CompletionDistance = completionDistance;
    }

    public override void CheckCompletion()
    {
        float distance = Vector3.Distance(PlayerController.playerTransform.position, TargetLocation.position);
        if (distance <= CompletionDistance)
        {
            IsCompleted = true;
            if(HasClaimed == false)
            {
                OnComplete?.Invoke();
                HasClaimed = true;
            }
        }
    }
}

public class ItemRetrievalObjective : QuestObjective
{
    public Item item;
    public int quantityNeeded;
    private int currentQuantity = 0;

    public ItemRetrievalObjective(Item item, int quantityNeeded, string name, string description) : base(name, description)
    {
        this.item = item;
        this.quantityNeeded = quantityNeeded;
    }

    public void ItemCollected(Item collectedItem, int quantity)
    {
        if (collectedItem == item)
        {
            currentQuantity += quantity;
            CheckCompletion();
        }
    }

    public override void CheckCompletion()
    {
        if (currentQuantity >= quantityNeeded)
        {
            IsCompleted = true;
            if (HasClaimed == false)
            {
                OnComplete?.Invoke();
                HasClaimed = true;
            }
        }
    }
}

public class Quest
{
    public string QuestTitle { get; set; }
    public string QuestDescription { get; set; }
    public int Priority { get; set; }
    public List<QuestObjective> Objectives { get; set; }

    public Quest(string questTitle, string questDescription, int priority, List<QuestObjective> objectives)
    {
        QuestTitle = questTitle;
        QuestDescription = questDescription;
        Priority = priority;
        Objectives = objectives;
    }

    public bool IsCompleted()
    {
        foreach (QuestObjective objective in Objectives)
        {
            if (!objective.IsCompleted)
            {
                return false;
            }
        }
        return true;
    }
}
