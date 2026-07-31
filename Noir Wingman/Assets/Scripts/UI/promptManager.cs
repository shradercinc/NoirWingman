using System;
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
        for (int i = 0; i < newConvo.Count - 1; i++) 
        {
            if (newConvo[i].conditionVar[0] != "Null")
            {
                print(newConvo[i].dialogueName + " has " + newConvo[i].conditionVar.Length + " conditionals");
                for (int j = 0; j < newConvo[i].conditionVar.Length; j++)
                {

                    switch (newConvo[i].conditionOpr[j])
                    {
                        case "Eq":
                            if (currentPerson.dialogVars[newConvo[i].conditionVar[j]] == newConvo[i].conditionVal[j])
                            {                        
                                CreateChoice(newConvo[i]);
                                print("equal");
                            }
                            print("not equal");
                            break;
                        case "Gt":
                            if (currentPerson.dialogVars[newConvo[i].conditionVar[j]] > newConvo[i].conditionVal[j])
                            {
                                CreateChoice(newConvo[i]);
                                print("greater than");
                            }
                            print("not greater than");
                            break;
                        case "Egt":
                            if (currentPerson.dialogVars[newConvo[i].conditionVar[j]] >= newConvo[i].conditionVal[j])
                            {
                                CreateChoice(newConvo[i]);
                                print("equal/greater than");
                            }
                            print("not equal greater than");
                            break;
                        case "Lt":
                            if (currentPerson.dialogVars[newConvo[i].conditionVar[j]] < newConvo[i].conditionVal[j])
                            {                               
                                CreateChoice(newConvo[i]);
                                print("Less than");
                            }
                            print("not less than");
                            break;
                        case "Elt":
                            if (currentPerson.dialogVars[newConvo[i].conditionVar[j]] < newConvo[i].conditionVal[j])
                            {
                                CreateChoice(newConvo[i]);
                                print("equal/less than");
                            }
                            print("not equal less than");
                            break;
                        default:
                            throw new ArgumentException("syntax error " + newConvo[i].conditionOpr + " is non-functional operator");
                    }
                }
            }
            else
            {
                print(newConvo[i].dialogueName + " has no conditionals");
                CreateChoice(newConvo[i]);
                
            }

            print("-------------------------------");
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
