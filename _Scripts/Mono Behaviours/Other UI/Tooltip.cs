using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text tooltipTextLabel;
    [SerializeField] private Image bgImage;
    public Sprite backgroundSprite;
    public float lifetime = 0;
    private float maxLifetime;
    public string text = "Text";
    public Action onDestroy;

    private void Update()
    {
        if(lifetime > maxLifetime)
        {
            maxLifetime = lifetime;
        }
        bgImage.color = new Color(bgImage.color.r, bgImage.color.b, bgImage.color.g, lifetime / maxLifetime);
        tooltipTextLabel.color = new Color(tooltipTextLabel.color.r, tooltipTextLabel.color.b, tooltipTextLabel.color.g, lifetime / maxLifetime);
        if(lifetime > 0)
        {
            lifetime -= Time.deltaTime;
        }
        else
        {
            onDestroy?.Invoke();
            Destroy(gameObject);
        }

        tooltipTextLabel.text = text;
        if (backgroundSprite != null) bgImage.sprite = backgroundSprite;
    }
}
