using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI; // Make sure to add this if you're using UI elements
using UnityEngine;
using TMPro;

public class DialogueManager : SingletonBehaviour<DialogueManager>
{
    [System.Serializable]
    public class Line
    {
        public string character;
        public string text;
        public List<DialogueOption> options;
        public List<Condition> conditions;
    }
    [System.Serializable]
    public class Condition
    {
        public string key;
        public string value;
        public EventResult yes;
        public EventResult no;
    }
    [System.Serializable]
    public class EventResult
    {
        public string eventId;
        public string action;
        public int goToLine;
    }
    [System.Serializable]
    public class Dialogue
    {
        public List<Line> lines;
        public string id;
    }
    [System.Serializable]
    public class DialogueOption
    {
        public string text;
        public string eventId;
        public string action;
        public int goToLine;
    }

    [System.Serializable]
    public class DialogueContainer
    {
        public List<Dialogue> dialogues;
    }

    public TextAsset dialogueJson;
    public List<Dialogue> dialogues;

    public Button optionButtonPrefab;
    public Transform optionsParent;

    public GameObject dialoguePanel; // The panel containing the dialogue UI elements
    public TMP_Text characterNameText; // The UI Text element for the character's name
    public TMP_Text dialogueText; // The UI Text element for the dialogue itself
    public float typingSpeed = 0.05f; // The speed at which the dialogue text is typed out
    [SerializeField] private float newLineSpeed;
    [SerializeField] private bool autoPlay;
    public WorkBench questWorkbench;

    private Dictionary<string, UnityAction> eventMap;

    private void Start()
    {
        LoadDialoguesFromJson();
        SetupEventMap(); // Set up the event map
        dialoguePanel.SetActive(false); // Hide the dialogue panel initially
    }

    [ContextMenu("TASIJ")]
    private void LoadDialoguesFromJson()
    {
        if (dialogueJson != null)
        {
            DialogueContainer container = JsonUtility.FromJson<DialogueContainer>(dialogueJson.text);
            dialogues = container.dialogues;
        }
        else
        {
            Debug.LogError("No JSON file assigned for dialogue data!");
        }
    }
    private void SetupEventMap()
    {
        eventMap = new Dictionary<string, UnityAction>();

        // Add your events here
        eventMap.Add("DoNothing", DoNothing);
        eventMap.Add("NewQuest", QuestManager.i.GetNewQuest);
        eventMap.Add("Workbench", Workbench);
        eventMap.Add("Dave", ProgressDave);
        // ...
    }
    private void ProgressDave()
    {
        Dave dave = FindObjectOfType<Dave>();
        if (dave != null)
        {
            dave.dialouges++;
        }
    }
    private void DoNothing() { }
    private void Workbench()
    {
        questWorkbench.active = false;
        questWorkbench.UpdatePanel();
    }
    public string dialogueId;
    [ContextMenu("Show Innit")]
    public void ShowDialouge()
    {
        ShowDialogue(dialogueId);
    }
    public void ShowDialogue(string dialogueId)
    {
        Dialogue dialogue = dialogues.Find(d => d.id == dialogueId);

        if (dialogue != null)
        {
            ClearOptionButtons();
            StopCoroutine(DisplayDialogue(dialogue));
            StartCoroutine(DisplayDialogue(dialogue));
        }
        else
        {
            Debug.LogError($"No dialogue found with ID {dialogueId}");
        }
    }
    private IEnumerator DisplayDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);

        for (int j = 0; j < dialogue.lines.Count; j++)
        {
            Line line = dialogue.lines[j];
            characterNameText.text = line.character;
            dialogueText.text = "";
            float _nextTypingTime = Time.time + typingSpeed;
            foreach (char letter in line.text.ToCharArray())
            {
                dialogueText.text += letter;

                // Wait for typingSpeed seconds or until the player presses Enter or left-clicks
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || Time.time >= _nextTypingTime);

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    // If the player presses Enter or left-clicks, display the full line immediately
                    dialogueText.text = line.text;
                    break; // Break out of the loop to skip the rest of the typing animation
                }
                else
                {
                    _nextTypingTime = Time.time + typingSpeed;
                }
            }

            bool optionPressed = false;
            for (int i = 0; i < line.options.Count; i++)
            {
                Button optionButton = Instantiate(optionButtonPrefab, optionsParent);
                var rect = optionButton.GetComponent<RectTransform>();
                Vector2 rectPos = rect.position;
                rectPos.y += (i * rect.sizeDelta.y) + 10;
                rect.position = rectPos;
                TMP_Text text = optionButton.GetComponentInChildren<TMP_Text>();
                text.text = line.options[i].text;
                string eventId = line.options[i].eventId;
                string action = line.options[i].action;
                int goToLine = line.options[i].goToLine;

                optionButton.onClick.AddListener(() => {
                    optionPressed = true;
                    if (!string.IsNullOrEmpty(eventId))
                    {
                        if (eventMap.ContainsKey(eventId))
                        {
                            eventMap[eventId].Invoke();
                        }
                        else
                        {
                            Debug.LogError($"No event found with ID {eventId}");
                        }
                    }
                    if (action == "EndDialogue")
                    {
                        j = dialogue.lines.Count;
                    }
                    else if (action == "GoToLine")
                    {
                        j = goToLine - 2; // Subtract 2 because the for loop increments lineIndex after the loop body
                    }
                    ClearOptionButtons();
                });
            }

            // Wait for the user to press an option if options are available
            if (line.options.Count > 0)
            {
                yield return new WaitUntil(() => optionPressed);
            }
            else
            {
                // Wait for the user to press a key or click/tap if no options are available
                yield return new WaitForSeconds(newLineSpeed);
                if (!autoPlay)
                {
                    yield return new WaitUntil(() => Input.anyKeyDown || Input.GetMouseButtonDown(0));
                }
            }
            bool conditionsMet = true;
            for(int i = 0; i < line.conditions.Count; i++)
            {
                Condition condition = line.conditions[i];

                if (!string.IsNullOrEmpty(condition.key))
                {

                    string key = condition.key;
                    string[] parameters = condition.value.Split("_");

                    if (parameters.Length > 0 && key.Contains("item"))
                    {
                        int quantity = int.Parse(parameters[2]);
                        if (PlayerInventoryManager.i.GetCountOfItemType(ItemDataBase.GetItem(parameters[1])) < quantity)
                        {
                            conditionsMet = false;
                        }
                    }
                }
                if (conditionsMet == false)
                {
                    if (condition.no != null)
                    {
                        string no_eventId = condition.no.eventId;
                        string no_action = condition.no.action;
                        int no_goToLine = condition.no.goToLine;
                        if (no_eventId != "")
                        {
                            if (eventMap.ContainsKey(no_eventId))
                            {
                                eventMap[no_eventId].Invoke();
                            }
                            else
                            {
                                Debug.LogError($"No event found with ID {no_eventId}");
                            }
                        }
                        if (no_action == "EndDialogue")
                        {
                            j = dialogue.lines.Count;
                            break;
                        }
                        else if (no_action == "GoToLine")
                        {
                            j = no_goToLine - 2; // Subtract 2 because the for loop increments lineIndex after the loop body
                            continue;
                        }
                        break;
                    }
                }

                string eventId = condition.yes.eventId;
                string action = condition.yes.action;
                int goToLine = condition.yes.goToLine;

                if (!string.IsNullOrEmpty(eventId)) // Add this null check
                {
                    if (eventMap.ContainsKey(eventId))
                    {
                        eventMap[eventId].Invoke();
                    }
                    else
                    {
                        Debug.LogError($"No event found with ID {eventId}");
                    }
                }
                if (action == "EndDialogue")
                {
                    j = dialogue.lines.Count;
                    break;
                }
                else if (action == "GoToLine")
                {
                    j = goToLine - 2; // Subtract 2 because the for loop increments lineIndex after the loop body
                    continue;
                }
            }
        }

        dialoguePanel.SetActive(false);
    }

    private void ClearOptionButtons()
    {
        foreach (Transform child in optionsParent)
        {
            Destroy(child.gameObject);
        }
    }
}