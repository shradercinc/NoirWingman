using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection.Emit;

public class EnableSelection : ButtonMaster
{
    [SerializeField] float hoverLerpTMax = 1;
    float hoverLerpTimer = 0;
    [SerializeField] Vector3 restPosition;
    [SerializeField] Vector3 activePosition;
    [SerializeField] AnimationCurve lerpCurve;
    bool mouseAbove;

    bool voting = false;
    [SerializeField] public List<selectPerson> POIList = new List<selectPerson>();

    [SerializeField] GameObject votingOverlay;
    [SerializeField] TMP_Text TitleCommandText;
    [SerializeField] TMP_Text voteTabText;
    public static EnableSelection instance;


    public override void Start()
    {
        base.Start();
        instance = this;
    }

    public override void Update()
    {

        base.Update();

        if (Input.mousePosition.y > Screen.height)
        {
            mouseAbove = true;
        } else mouseAbove = false;


            //lerps the tab in frame if you mouse over it
        if (mouseOver || mouseAbove) 
        {
            if (hoverLerpTimer < hoverLerpTMax)
            {
                hoverLerpTimer += Time.deltaTime;
            }
        }
        else if (hoverLerpTimer > 0)
        {
            hoverLerpTimer -= Time.deltaTime;
            if (hoverLerpTimer < 0) hoverLerpTimer = 0; 
        }

        Vector3 interimPos = (activePosition - restPosition) * lerpCurve.Evaluate(hoverLerpTimer);
        transform.localPosition = restPosition + interimPos;
    }

    void beginVoting()
    {
        voteTabText.text = "Disable Voting";
        TitleCommandText.text = "Which one will it be detective?";
        votingOverlay.SetActive(true);
        voting = true;
        foreach (selectPerson i in POIList)
        {
            i.enableSelection();
        }
    }

    void disableVoting()
    {
        voteTabText.text = "Select Date";
        TitleCommandText.text = "Chat with a person of interest";
        votingOverlay.SetActive(false);
        voting = false;
        foreach (selectPerson i in POIList)
        {
            i.disableSelection();
        }
    }

    public override void PressedEffect()
    {
        if ((mouseAbove || mouseOver) && hoverLerpTimer >= hoverLerpTMax)
        {
            if (!voting) beginVoting();
            else disableVoting();
        }
    }
}
