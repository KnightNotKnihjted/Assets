using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
}
