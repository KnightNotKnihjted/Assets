using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBrain : MonoBehaviour
{
    public WoodType woodType;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        TreeLog log = Instantiate(woodType.prefab, transform);
        log.myLootTable = woodType.woodLootTable;
        log.brain = this;
    }
}
