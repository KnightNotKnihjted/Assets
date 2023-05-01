using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace JaasUtilities
{
    public class WorldRaycaster : GraphicRaycaster
    {

        [SerializeField]
        private int SortOrder = 0;

        public override int sortOrderPriority
        {
            get
            {
                return SortOrder;
            }
        }
    }
}
