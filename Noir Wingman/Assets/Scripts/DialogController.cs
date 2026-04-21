using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine.Rendering.PostProcessing;

public class DialogController : MonoBehaviour
{
    public selectPerson rootConversation;
    public static DialogController instance;
    public DialogueContainer convoToPrint;
    public TMP_Text dialogueText;
    protected int progress = 0;
    protected bool inUse = false;
    bool usingIntro;
    bool usingOutro;
    [SerializeField] protected GameObject patronMenu;
    [SerializeField] protected GameObject dialogueMenu;
    [SerializeField] protected GameObject promptMenu;

    [SerializeField] AnimationCurve ShadeBlendCurve;
    [SerializeField] AnimationCurve ExpressionBlendCurve;
    [SerializeField] float ExpressionBlendDuration = 0.1f;
    [SerializeField] Image ExpressionObject;
    [SerializeField] Image ExpressionShade;
    //[SerializeField]

    [SerializeField] TMP_Text dialogueSpeaker;
    [SerializeField] GameObject SpeakerObj;
    [SerializeField] GameObject protagonistOverlay;
    [SerializeField] PostProcessVolume NoirOverlay;
    [SerializeField] Vector3[] standardPositions;

    [SerializeField] GameObject EndReportMenu;



    public virtual void Awake()
    {
        instance = this;
    }
     
    public virtual void startNewConvo(bool intro, bool outro)
    {
        if(intro) usingIntro = true;
        else usingIntro = false;

        if (outro) usingOutro = true;
        else usingOutro = false;

            patronMenu.SetActive(true);

        patronMenu.SetActive(false);
        progress = 0;
        UpdateDialogue();

        inUse = true;
    }

    public virtual void EndConvo()
    {
        NoirOverlay.enabled = false;
        patronMenu.SetActive(true);
        if (!convoToPrint.read)
        {
            if (convoToPrint.effectVar != "Null")
            {
                print(convoToPrint.effectVar + " " + convoToPrint.effectOpr + " " + convoToPrint.effectVal);

                switch (convoToPrint.effectOpr)
                {
                    case "add":
                        print(rootConversation.dialogVars[convoToPrint.effectVar]);
                        rootConversation.dialogVars[convoToPrint.effectVar] += convoToPrint.effectVal;
                        print(rootConversation.dialogVars[convoToPrint.effectVar]);
                        break;
                    case "subtract":
                        rootConversation.dialogVars[convoToPrint.effectVar] -= convoToPrint.effectVal;
                        break;
                    case "multiply":
                        rootConversation.dialogVars[convoToPrint.effectVar] *= convoToPrint.effectVal;
                        break;
                    case "divide":
                        rootConversation.dialogVars[convoToPrint.effectVar] /= convoToPrint.effectVal;
                        break;
                }
            }
            rootConversation.patience -= convoToPrint.patienceReq;

            for (int i = 0; i < rootConversation.fullConversation.Count; i++)
            {
                if (convoToPrint.dialogueName == rootConversation.fullConversation[i].dialogueName)
                {
                    convoToPrint.read = true;
                    rootConversation.fullConversation[i] = convoToPrint;
                }
            }
        }
        if (usingIntro)
        {
            dialogueMenu.SetActive(false);
        }
        else if (usingOutro)
        {
            EndReportMenu.SetActive(true);
            ReferenceKeeper endRef = EndReportMenu.GetComponent<ReferenceKeeper>();
            endRef.nameObj.text = "You picked " + rootConversation.gameObject.name + " For " + endRef.levelFriend;
            endRef.summaryObj.text = rootConversation.TempEndString;
            dialogueMenu.SetActive(false);
            patronMenu.SetActive(false);
        }
        else
        {
            promptMenu.SetActive(true);
            promptManager.instance.LoadNewConvo(rootConversation.fullConversation);
            promptManager.instance.patienceHolder.text = rootConversation.patience.ToString();
            patronMenu.SetActive(false);
            dialogueMenu.SetActive(false);
        }
    }
    
    protected void UpdateDialogue()
    {
        string newText = convoToPrint.reaction[progress];
        if (newText.StartsWith('*'))
        {
            newText = newText.Trim('*');
            NoirOverlay.enabled = true;
            //protagonistOverlay.SetActive(true);
            SpeakerObj.SetActive(false);
        } else
        {
            NoirOverlay.enabled = false;
            //protagonistOverlay.SetActive(false);
            SpeakerObj.SetActive(true);
        }
        dialogueText.text = newText;



        //Places, orients and changes the sprites based on how the scene changes
        if (progress == 0)
        {
            //to be replaced with code that slides new speakers on from off screen
            //if the dialogue just started: Fast placement, fade off
            print("First Line");
            fastPlacement();
            SnapExpression();
        }
        else if (convoToPrint.position[progress] == convoToPrint.position[progress - 1])
        {
            print("Same Position");
            //if the character's are in the same place Fast placement, fade off if speaker and expression are the same
            fastPlacement();

            if (convoToPrint.speaker[progress] == convoToPrint.speaker[progress - 1] && convoToPrint.expression[progress] == convoToPrint.expression[progress - 1]) SnapExpression();
            else StartCoroutine(BlendExpression(false));
        }
        else if (convoToPrint.speaker[progress] == convoToPrint.speaker[progress - 1])
        {
            print("Same Speaker");
            //if the speaker is the same: fast placement, blend 
            fastPlacement();
            StartCoroutine(BlendExpression(false));
        }
        else
        {
            //if nothing is the same, Slow placement, fade on
            ExpressionShade.transform.localPosition = standardPositions[convoToPrint.position[progress]];
            if (convoToPrint.position[progress] <= 1 && ExpressionObject.rectTransform.localScale.x > 0) ExpressionObject.rectTransform.localScale = new Vector3(-1, 1, 1);
            if (convoToPrint.position[progress] >= 2 && ExpressionObject.rectTransform.localScale.x < 0) ExpressionObject.rectTransform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(BlendExpression(true));
        }


        dialogueSpeaker.text = convoToPrint.speaker[progress];
    }

    void fastPlacement()
    {
        //changes the position and orientation of speaker, copies this too the shade
        ExpressionObject.transform.localPosition = standardPositions[convoToPrint.position[progress]];
        if (convoToPrint.position[progress] <= 1 && ExpressionObject.rectTransform.localScale.x > 0) ExpressionObject.rectTransform.localScale = new Vector3(-1, 1, 1);
        if (convoToPrint.position[progress] >= 2 && ExpressionObject.rectTransform.localScale.x < 0) ExpressionObject.rectTransform.localScale = new Vector3(1, 1, 1);
        ExpressionShade.transform.localPosition = ExpressionObject.transform.localPosition;
        ExpressionShade.rectTransform.localScale = ExpressionObject.rectTransform.localScale;
    }

    void SnapExpression()
    {
        ExpressionObject.sprite = ExpressionHolder.instance.ExpressionGrabber(convoToPrint.speaker[progress], convoToPrint.expression[progress]);
    }

    IEnumerator BlendExpression(bool slowPlacement)
    {
        Sprite newSprite = ExpressionHolder.instance.ExpressionGrabber(convoToPrint.speaker[progress], convoToPrint.expression[progress]);
        ExpressionShade.sprite = newSprite;
        float blendTimer = 0;
        float currentRatio = 0;
        while (blendTimer <= ExpressionBlendDuration)
        {
            
            blendTimer += Time.deltaTime;
            currentRatio = blendTimer / ExpressionBlendDuration;
            //print("Ratio " + currentRatio);
            //print("Eval " + ExpressionBlendCurve.Evaluate(currentRatio));

            ExpressionShade.color = new Color(1f,1f,1f, ShadeBlendCurve.Evaluate(currentRatio));
            ExpressionObject.color = new Color(1f, 1f, 1f, ExpressionBlendCurve.Evaluate(currentRatio));
            //print("Shade a " + ExpressionShade.color.a);
            //print("obj a " + ExpressionObject.color.a);
            yield return null;
        }

        if (slowPlacement)
        {
            ExpressionObject.transform.localPosition = ExpressionShade.transform.localPosition;
            ExpressionObject.rectTransform.localScale = ExpressionShade.rectTransform.localScale;
        }
        
        ExpressionObject.sprite = newSprite;
        ExpressionObject.color = new Color(1f,1f,1f,1f);
        ExpressionShade.color = new Color(1f, 1f, 1f, 0f);
        
    }

    void Update()
    {
        if (inUse && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) 
        {
            progress++;
            if (progress < convoToPrint.reaction.Length)
            {
                UpdateDialogue();
            }
            else
            {
                EndConvo();
            }

        }
    }
}
