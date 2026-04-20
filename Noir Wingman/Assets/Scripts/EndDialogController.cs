using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class EndDialogController : DialogController
{
    //public static EndDialogController endInstance;
    //[SerializeField] GameObject EndReportMenu;

    public override void Awake()
    {
       // endInstance = this;
    }

    public override void startNewConvo(bool intro, bool outro)
    {
        //child may be edited with new intro dialogue controller child
        //currentExpressions = rootConversation.Expressions;
        progress = 0;
        inUse = true;
        UpdateDialogue();
        patronMenu.SetActive(false);

    }

    public override void EndConvo()
    {
        /*
        EndReportMenu.SetActive(true);
        endInstance.gameObject.SetActive(false);
        ReferenceKeeper endRef = EndReportMenu.GetComponent<ReferenceKeeper>();
        endRef.nameObj.text = "You picked " + rootConversation.gameObject.name + " For " + endRef.levelFriend;
        endRef.summaryObj.text = rootConversation.TempEndString;
        */
    }
}
