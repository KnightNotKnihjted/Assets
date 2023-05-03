using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JaasUtilities;

public class UI_Objective : MonoBehaviour
{
    private TooltipGenerator tooltipGenerator;
    [SerializeField] private TMP_Text titleText;
    public QuestObjective m_questObjective;
    private void Start()
    {
        tooltipGenerator = GetComponent<TooltipGenerator>();
    }
    private void Update()
    {
        titleText.text = m_questObjective.ObjectiveName;
        tooltipGenerator.tooltipText = m_questObjective.ObjectiveDescription;
    }
}
