using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JaasUtilities;

public class UI_Quest : MonoBehaviour
{
    private TooltipGenerator tooltipGenerator;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform objectivesRect;
    [SerializeField] private float objectiveYSize;
    [SerializeField] private UI_Objective ui_QuestObjectivePrefab;
    public Quest m_quest;
    private float defaultY;
    private float fullY;

    public void Init()
    {
        tooltipGenerator = GetComponent<TooltipGenerator>();
        titleText.text = m_quest.QuestTitle;

        defaultY = GetComponent<RectTransform>().sizeDelta.y;
        fullY = defaultY;
        foreach(QuestObjective objective in m_quest.Objectives)
        {
            if (objective is LocationObjective) continue;
            UI_Objective obj = Instantiate(ui_QuestObjectivePrefab, objectivesRect);
            obj.m_questObjective = objective;
            fullY += objectiveYSize;

            objective.OnComplete += () =>
            {
                if (obj != null)
                {
                    if (obj.gameObject != null)
                    {
                        Destroy(obj.gameObject);
                    }
                }
            };
        }
        Destroy(gameObject, 60f);
    }

    private void Update()
    {
        tooltipGenerator.tooltipText = m_quest.QuestDescription;

        Vector2 localMousePosition = tooltipGenerator.rect.InverseTransformPoint(GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>());
        if (tooltipGenerator.rect.rect.Contains(localMousePosition))
        {
            objectivesRect.gameObject.SetActive(true);
            transform.GetComponent<RectTransform>().TweenHeight(fullY, 0.76f);
            objectivesRect.TweenHeight(m_quest.Objectives.Count * objectiveYSize, 0.76f);
        }
        else
        {
            if (objectivesRect.gameObject.activeSelf)
            {
                objectivesRect.TweenHeight(0, 0.56f)
                .setOnComplete(() =>
                {
                    objectivesRect.gameObject.SetActive(false);
                });
            }
            transform.GetComponent<RectTransform>().TweenHeight(defaultY, 0.56f);
        }
    }
}
