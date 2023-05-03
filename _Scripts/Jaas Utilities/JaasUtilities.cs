using LeanTweenFramework = LeanTween;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

namespace JaasUtilities
{
    public static class MouseUtils
    {
        public static Vector3 MouseWorldPos()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
            return mousePos;
        }
    }

    public static class LeanTweenExtensions
    {
        public static LTDescr TweenWidth(this RectTransform rectTransform, float to, float duration)
        {
            float originalHeight = rectTransform.sizeDelta.x;
            return LeanTweenFramework.value(rectTransform.gameObject, originalHeight, to, duration)
                .setOnUpdate((float value) =>
                {
                    Vector2 sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.x = value;
                    rectTransform.sizeDelta = sizeDelta;
                });
        }
        public static LTDescr TweenHeight(this RectTransform rectTransform, float to, float duration)
        {
            float originalHeight = rectTransform.sizeDelta.y;
            return LeanTweenFramework.value(rectTransform.gameObject, originalHeight, to, duration)
                .setOnUpdate((float value) =>
                {
                    Vector2 sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.y = value;
                    rectTransform.sizeDelta = sizeDelta;
                });
        }
    }

}
