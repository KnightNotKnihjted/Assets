using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JaasUtilities;

public class TooltipGenerator : MonoBehaviour
{
    public string tooltipText;
    public RectTransform rect;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Tooltip tooltip;

    private void Update()
    {
        if(rect != null)
        {
            Vector2 localMousePosition = rect.InverseTransformPoint(GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>());
            if (rect.rect.Contains(localMousePosition))
            {
                if (tooltip == null)
                {
                    tooltip = Instantiate(tooltip);
                    tooltip.text = tooltipText;
                    if (backgroundSprite != null)
                    {
                        tooltip.backgroundSprite = backgroundSprite;
                    }
                }
                RectTransform tR = tooltip.GetComponent<RectTransform>();
                tR.position = GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>();
                tooltip.lifetime = 0.8f;
            }
        }
    }
}
