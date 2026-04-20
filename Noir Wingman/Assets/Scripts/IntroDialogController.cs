using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class IntroDialogController : MonoBehaviour
{
    selectPerson myTextHolder;
    private float frameCount = 0;

    private void Awake()
    {
        myTextHolder = gameObject.GetComponent<selectPerson>();
    }
    void StartIntroConvo()
    {
        print("Full conversation count" + myTextHolder.fullConversation.Count);
        DialogController.instance.gameObject.SetActive(true);
        DialogController.instance.rootConversation = myTextHolder;
        DialogController.instance.convoToPrint = myTextHolder.fullConversation[0];
        DialogController.instance.startNewConvo(true,false);
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount <= 0)
        {
            frameCount++;
            StartIntroConvo();
        }
    }
}
