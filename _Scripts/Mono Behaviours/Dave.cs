using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dave : MonoBehaviour
{
    [SerializeField] private List<string> dialogueIds = new ();
    public int dialouges;

    private void Update()
    {
        var obj = GetComponent<JaasUtilities.WorldObjectButton>();
        if (obj != null)
        {
            obj.onClickAction = () =>
            {
                DialogueManager.i.ShowDialogue(dialogueIds[dialouges]);
            };
        }
    }
}
