using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_ItemObject : MonoBehaviour
{
    [SerializeField] private TMP_Text quantityLabelRef;
    [SerializeField] private Image iconDisplay;
    public UI_InventorySlot origin;
    public Item myItem;
    public int quantityHeld;

    private void Update()
    {
        if (myItem != null)
        {
            quantityLabelRef.text = quantityHeld.ToString();
            iconDisplay.sprite = myItem.Icon;
        }
    }
}
