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
    string dialogueCondition;
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
     
    public virtual void startNewConvo(string condition)
    {
        dialogueCondition = condition;


        patronMenu.SetActive(true);

        patronMenu.SetActive(false);
        progress = 0;
        UpdateDialogue();

        inUse = true;
    }

    public virtual void EndConvo()
    {
        //dialogue menu reset
        ExpressionObject.rectTransform.localScale = new Vector3(Mathf.Abs(ExpressionObject.rectTransform.localScale.x), ExpressionObject.rectTransform.localScale.y, ExpressionObject.rectTransform.localScale.z);
        NoirOverlay.enabled = false;


        patronMenu.SetActive(true);
        if (!convoToPrint.read)
        {
            if (convoToPrint.effectVar[0] != "Null")
            {
                for (int i = 0; i < convoToPrint.effectVar.Length; i++)
                {
                    switch (convoToPrint.effectOpr[i])
                    {
                        case "add":
                            rootConversation.dialogVars[convoToPrint.effectVar[i]] += convoToPrint.effectVal[i];
                            break;
                        case "subtract":
                            rootConversation.dialogVars[convoToPrint.effectVar[i]] -= convoToPrint.effectVal[i];
                            break;
                        case "multiply":
                            rootConversation.dialogVars[convoToPrint.effectVar[i]] *= convoToPrint.effectVal[i];
                            break;
                        case "divide":
                            rootConversation.dialogVars[convoToPrint.effectVar[i]] /= convoToPrint.effectVal[i];
                            break;
                    }
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
        switch (dialogueCondition)
        {
            case "Intro":
                dialogueMenu.SetActive(false);
                break;
            case "Outro":
                EndReportMenu.SetActive(true);
                ReferenceKeeper endRef = EndReportMenu.GetComponent<ReferenceKeeper>();
                endRef.nameObj.text = "You picked " + rootConversation.gameObject.name + " For " + endRef.levelFriend;
                endRef.summaryObj.text = rootConversation.TempEndString;
                dialogueMenu.SetActive(false);
                patronMenu.SetActive(false);
                break;
            case "POIIntro":
                break;
            case "":
                promptMenu.SetActive(true);
                promptManager.instance.LoadNewConvo(rootConversation.fullConversation);
                promptManager.instance.patienceHolder.text = rootConversation.patience.ToString();
                patronMenu.SetActive(false);
                dialogueMenu.SetActive(false);
                break;
        }
    }
    
    protected void UpdateDialogue()
    {
        dialogueSpeaker.text = convoToPrint.speaker[progress];
        //checks whether filter needs to be displayed and then displays text
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

        //places the character
        if (progress == 0)
        {
            fastPlacement();
        }
        else
        {
            //If the speaker, position and expression are all the same, use fast placement (kinda pointless cause nothing changes but it's for clarity)
            if (convoToPrint.position[progress] == convoToPrint.position[progress - 1] && convoToPrint.speaker[progress] == convoToPrint.speaker[progress - 1] && convoToPrint.expression[progress] == convoToPrint.expression[progress - 1])
            {
                fastPlacement();
            }
            else
            {
                StartCoroutine(BlendExpression());
            }
        }
    }

    void fastPlacement() //Immediately places, sizes and orients a character, copying the shade
    {
        //changes the position of speaker
        ExpressionObject.transform.localPosition = standardPositions[convoToPrint.position[progress]];
        

        //changes the size of the speaker 
        ExpressionObject.transform.localScale = ExpressionHolder.instance.characterSizing[convoToPrint.speaker[progress]];

        //changes the orientation based on position
        if (convoToPrint.position[progress] >= 2) ExpressionObject.transform.localScale = new Vector3(ExpressionObject.transform.localScale.x * -1, ExpressionObject.transform.localScale.y, 1);

        //changes the appearance
        ExpressionObject.sprite = ExpressionHolder.instance.ExpressionGrabber(convoToPrint.speaker[progress], convoToPrint.expression[progress]);

        //copies to shade
        ExpressionShade.transform.localPosition = ExpressionObject.transform.localPosition;
        ExpressionShade.transform.localScale = ExpressionObject.transform.localScale;
        ExpressionShade.sprite = ExpressionObject.sprite;

    }

    IEnumerator BlendExpression()
    {
        Sprite newSprite = ExpressionHolder.instance.ExpressionGrabber(convoToPrint.speaker[progress], convoToPrint.expression[progress]);
        Vector3 newSize = ExpressionHolder.instance.characterSizing[convoToPrint.speaker[progress]];
        if (convoToPrint.position[progress] >= 2) newSize = new Vector3(newSize.x * -1, newSize.y, 1);

        //Expression Shade moves to the new location, take the new look then fades in as the old sprite fades out. Once the fade is complete, expression object jumps to expression shade, copies it, and reappears

        //changing the appearnace of the shade
        ExpressionShade.sprite = newSprite;
        ExpressionShade.rectTransform.localScale = newSize;
        ExpressionShade.color = new Color(1f, 1f, 1f, 0f);

        //placing shade
        ExpressionShade.rectTransform.localPosition = standardPositions[convoToPrint.position[progress]];

        float blendTimer = 0;
        float currentRatio = 0;
        while (blendTimer <= ExpressionBlendDuration)
        {
            blendTimer += Time.deltaTime;
            currentRatio = blendTimer / ExpressionBlendDuration;

            //tweens the alpha of each sprite so that the shade is visible while the object is not
            ExpressionShade.color = new Color(1f, 1f, 1f, ShadeBlendCurve.Evaluate(currentRatio));
            ExpressionObject.color = new Color(1f, 1f, 1f, ExpressionBlendCurve.Evaluate(currentRatio));
            yield return null;
        }

        ExpressionObject.sprite = newSprite;
        ExpressionObject.rectTransform.localScale = newSize;
        ExpressionObject.rectTransform.localPosition = standardPositions[convoToPrint.position[progress]];
        ExpressionObject.color = new Color(1f, 1f, 1f, 1f);
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
