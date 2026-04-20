using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

public class promptPrefabController : ButtonMaster
{
    public DialogueContainer myContainer;
    [SerializeField] TMP_Text myText;
    public selectPerson conversationRoot;
    [SerializeField] TMP_Text patienceValue;
    [SerializeField] GameObject hasReadOverlay;
    [SerializeField] Outline boxOutline;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        myText.text = myContainer.prompt;
        patienceValue.text = myContainer.patienceMod.ToString();
        if(myContainer.read)
        {
            hasReadOverlay.SetActive(true);
        }
        if (conversationRoot.patience < myContainer.patienceReq)
        {
            hasReadOverlay.SetActive(true);
            boxOutline.effectColor = Color.red;
        }


    }

    public override void OnPointerEnter(PointerEventData data)
    {
        base.OnPointerEnter(data);
        hasReadOverlay.transform.localScale = baseSize * hoverMod;
    }

    public override void OnPointerExit(PointerEventData data)
    {
        base.OnPointerExit(data);
        hasReadOverlay.transform.localScale = baseSize;
    }

    public override void PressedEffect()
    {
        if (conversationRoot.patience > myContainer.patienceReq)
        {
            promptManager.instance.dialogueMenu.SetActive(true);
            DialogController.instance.rootConversation = conversationRoot;
            DialogController.instance.convoToPrint = myContainer;
            DialogController.instance.startNewConvo(false,false);
            promptManager.instance.resetContents(true);
        }

    }

}
