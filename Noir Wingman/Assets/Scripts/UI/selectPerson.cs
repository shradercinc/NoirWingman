using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public struct DialogueContainer 
{
    public string dialogueName;
    public string prompt;
    public string[] reaction;
    public string[] expression;
    public string conditionVar;
    public string conditionOpr;
    public int conditionVal;
    public string effectVar;
    public string effectOpr;
    public int effectVal;
    public bool read;
    public int[] position;
    public string[] speaker;
    public int patienceReq;
    public int patienceMod;


    public void DialogInstantiation(string name, string NPrompt, string[] NReaction, string[] Nexpression, string NConditionVar, string NConditionOpr, int NConditionVal, string NEffectVar, string NEffectOpr, int NEffectVal, int[] Nposition, string[] Nspeaker, int NpatienceReq, int NpatienceMod)
    {
        read = false;

        dialogueName = name;
        prompt = NPrompt;
        reaction = NReaction;
        expression = Nexpression;
        conditionVar = NConditionVar;
        conditionOpr = NConditionOpr;
        conditionVal = NConditionVal;
        effectVar = NEffectVar;
        effectOpr = NEffectOpr;
        effectVal = NEffectVal;
        position = Nposition;
        speaker = Nspeaker;
        patienceReq = NpatienceReq;
        patienceMod = NpatienceMod;

    }

    public void nowRead()
    {
        this.read = true;
    }

}
public class selectPerson : ButtonMaster
{
    [SerializeField] TextAsset MyConversationTSV;
    [SerializeField] TextAsset MyEndConversationsTSV;
    string[,] preparsed;
    public Dictionary<string, int> dialogVars = new();
    public List<DialogueContainer> fullConversation = new();
    public List<DialogueContainer> endConversation = new();
    [SerializeField] GameObject promptMenu;
    [SerializeField] GameObject patronMenu;
    // In order, expressions are always Neutral, Happy, Sad, Angry, Laughing (to be ammended or altered)
    [SerializeField] public List<Sprite> Expressions;
    public bool instantiated = false;
    public int patience = 10;

    //These variables are related to the voting/selection phase
    private bool selectionEnabled = false;
    [SerializeField] AnimationCurve throbCurve;
    [SerializeField] float throbTMax = 0.5f;
    [SerializeField] float throbMod = 0.2f;
    private float throbT = 0;
    bool throbI = false;

    [SerializeField] public string TempEndString;

    public override void Start()
    {
        fullConversation = convertTSVtoDialogue(MyConversationTSV);
        if (MyEndConversationsTSV != null)
        {
            endConversation = convertTSVtoDialogue(MyEndConversationsTSV);
        }

        instantiated = true;

    }

    List<DialogueContainer> convertTSVtoDialogue(TextAsset newTSV)
    {
        preparsed = TSVReader.instance.readTSV(newTSV);
        List<DialogueContainer> TempDiaList = new List<DialogueContainer>();

        string tname;
        string tprompt;
        string[] treaction;
        string[] texpression;
        string tcondVar;
        string tcondOpr;
        int tcondVal;
        string teffVar;
        string teffOpr;
        int teffVal;
        string[] tspeaker;
        int[] tposition;
        int tpatienceReq;
        int tpatienceMod;

        for (int i = 0; i < preparsed.GetLength(0) - 2; i++)
        {
            tname = preparsed[i, 0].Trim(' ');

            tprompt = preparsed[i, 1].Trim(' ');

            treaction = preparsed[i, 2].Split("|");
            for (int s = 0; s < treaction.Length; s++) treaction[s] = treaction[s].Trim('"', ' '); 

            texpression = preparsed[i, 3].Split("|");
            for (int s = 0; s < texpression.Length; s++) texpression[s] = texpression[s].Trim('"', ' '); 

            tcondVar = preparsed[i, 4].Trim(' ');
            if (!dialogVars.ContainsKey(tcondVar) && tcondVar != "Null") dialogVars.Add(tcondVar, 0); 

            tcondOpr = preparsed[i, 5].Trim(' ');

            tcondVal = int.Parse(preparsed[i, 6]);

            teffVar = preparsed[i, 7].Trim(' ');
            if (!dialogVars.ContainsKey(tcondVar) && teffVar != "Null") dialogVars.Add(tcondVar, 0);

            teffOpr = preparsed[i, 8].Trim(' ');

            teffVal = int.Parse(preparsed[i, 9]);

            string[] prep = preparsed[i, 10].Split("|");
            tposition = new int[prep.Length];
            for (int j = 0; j < prep.Length; j++) tposition[j] = int.Parse(prep[j]);

            tspeaker = preparsed[i, 11].Split("|");
            for(int s = 0; s < tspeaker.Length; s++) tspeaker[s] = tspeaker[s].Trim(' ');
            
            tpatienceReq = int.Parse(preparsed[i, 12]);
            
            tpatienceMod = int.Parse(preparsed[i, 13]);



            DialogueContainer singleLine = new DialogueContainer();
            singleLine.DialogInstantiation(tname, tprompt, treaction, texpression, tcondVar, tcondOpr, tcondVal, teffVar, teffOpr, teffVal, tposition, tspeaker, tpatienceReq, tpatienceMod);
            TempDiaList.Add(singleLine);


        }
        return TempDiaList;
    }

    public void enableSelection()
    {
        selectionEnabled = true;
        backgroundRect.transform.localScale = baseSize;
    }

    public void disableSelection()
    {
        selectionEnabled = false;
        throbI = false;
        backgroundRect.transform.localScale = baseSize;
    }

    public override void OnMouseEnter()
    {
        mouseOver = true;
        if (!selectionEnabled) backgroundRect.transform.localScale = baseSize * hoverMod;
    }

    public override void OnMouseExit()
    {
        mouseOver = false;
        if (!selectionEnabled) backgroundRect.transform.localScale = baseSize;
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        mouseOver = true;
        if(!selectionEnabled) backgroundRect.transform.localScale = baseSize * hoverMod;
    }

    public override void OnPointerExit(PointerEventData data)
    {
        mouseOver = false;
        if (!selectionEnabled) backgroundRect.transform.localScale = baseSize;
    }

    public override void Update()
    {
        base.Update();
        if (selectionEnabled)
        {
            if (throbI)
            {
                throbT += Time.deltaTime;
                if (throbT >= throbTMax) throbI = false;
            }
            else
            {
                throbT -= Time.deltaTime;
                if (throbT <= 0) throbI = true;
            }
            backgroundRect.transform.localScale = baseSize * (1 + (throbCurve.Evaluate(throbT/throbTMax) * throbMod));

        }
    }

    public override void PressedEffect()
    {
        if (!selectionEnabled)
        {
            promptMenu.SetActive(true);
            promptManager.instance.resetContents(false);
            promptManager.instance.currentPerson = this;
            promptManager.instance.LoadNewConvo(fullConversation);
            patronMenu.SetActive(false);
        }
        else
        {
            selectionEnabled = false;
            print(endConversation[0].dialogueName);
            DialogController Dia = DialogController.instance;
            Dia.gameObject.SetActive(true);
            Dia.rootConversation = this;
            Dia.convoToPrint = endConversation[0];
            Dia.startNewConvo(false, true);
            patronMenu.SetActive(false);


            /*
            EndDialogController.endInstance.gameObject.SetActive(true);
            EndDialogController.endInstance.rootConversation = this;
            foreach (string i in endConversation[0].reaction) print(i);

            EndDialogController.endInstance.convoToPrint = endConversation[0]; //this should be changed to be relative to patience levels later
            
            EndDialogController.endInstance.startNewConvo(false);
            
            //simplified code, needs modification with patience system implementation
            DialogController.instance.gameObject.SetActive(true);
            DialogController.instance.convoToPrint = endConversation[0];
            DialogController.instance.rootConversation = this;
            DialogController.instance.startNewConvo(false);
            */

        }

    }
}
