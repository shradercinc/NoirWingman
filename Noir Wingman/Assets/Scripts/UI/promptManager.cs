using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class promptManager : MonoBehaviour
{
    Dictionary<string, string> choices = new Dictionary<string, string>();
    public static promptManager instance;
    [SerializeField] GameObject promptPrefab;
    public GameObject promptMenu;
    public GameObject dialogueMenu;
    [SerializeField] Vector3 initialPromptLocation;
    [SerializeField] Vector3 promptMargin;
    private int marginTrack = 0;
    public selectPerson currentPerson;
    List<GameObject> listOfPrompts = new List<GameObject>();
    [SerializeField] GameObject promptHolder;
    [SerializeField] public TMP_Text patienceHolder;

    private void Awake()
    {
        instance = this;
    }

    public void LoadNewConvo(List<DialogueContainer> newConvo)
    {
        print("Loading new Convo...");
        for (int i = 0; i < newConvo.Count; i++) 
        {
            if (newConvo[i].conditionVar != "Null")
            {
                //print("Condition Found");
                switch (newConvo[i].conditionOpr)
                {
                    case "eq":
                        if (currentPerson.dialogVars[newConvo[i].conditionVar] == newConvo[i].conditionVal)
                        {
                            CreateChoice(newConvo[i]);
                        }
                        break;
                    case "gt":
                        if (currentPerson.dialogVars[newConvo[i].conditionVar] > newConvo[i].conditionVal)
                        {
                            CreateChoice(newConvo[i]);
                        }
                        break;
                    case "egt":
                        if (currentPerson.dialogVars[newConvo[i].conditionVar] >= newConvo[i].conditionVal)
                        {
                            CreateChoice(newConvo[i]);
                        }
                        break;
                    case "lt":
                        if (currentPerson.dialogVars[newConvo[i].conditionVar] < newConvo[i].conditionVal)
                        {
                            CreateChoice(newConvo[i]);
                        }
                        break;
                    case "elt":
                        if (currentPerson.dialogVars[newConvo[i].conditionVar] < newConvo[i].conditionVal)
                        {
                            CreateChoice(newConvo[i]);
                        }
                        break;
                }
            }
            else
            {
                //print("No Condition");
                CreateChoice(newConvo[i]);
            }
        }
        print("Conversation Loaded!");
    }

    public void resetContents(bool close)
    {
        for (int i = 0; i < listOfPrompts.Count; i++)
        {
            Destroy(listOfPrompts[i]);
        }
        marginTrack = 0;
        listOfPrompts.Clear();
        if (close) gameObject.SetActive(false);
    }

    private void CreateChoice(DialogueContainer promptDialogue)
    {
        //print("Creating Choice");
        GameObject newChoice = Instantiate(promptPrefab, transform.position, Quaternion.identity, promptHolder.transform);
        promptPrefabController newChoiceCon = newChoice.GetComponent<promptPrefabController>();
        listOfPrompts.Add(newChoice);
        //newChoice.transform.position = initialPromptLocation + (promptMargin * marginTrack);
        newChoiceCon.conversationRoot = currentPerson;
        newChoiceCon.myContainer = promptDialogue;
        marginTrack++;
    }
}
