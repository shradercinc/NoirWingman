using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class POIIntroHold : MonoBehaviour
{
    [SerializeField] TextAsset POIIntroText;
    public List<DialogueContainer> POIIntroContainer;
    selectPerson parentPerson;

    private void Start()
    {
        parentPerson = GetComponentInParent<selectPerson>();
        POIIntroContainer = parentPerson.convertTSVtoDialogue(POIIntroText);
    }
}
