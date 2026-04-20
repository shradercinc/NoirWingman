using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DossierController : MonoBehaviour
{
    [SerializeField] RectTransform dossier;
    [SerializeField] GameObject dossierLeftPage;
    [SerializeField] GameObject dossierRightPage;
    [SerializeField] Vector3 observationPosition = Vector3.zero;
    [SerializeField] Vector3 startingPosition = new Vector3(0,-1500,0);

    [SerializeField] Vector3 observationRotationL =  Vector3.zero;
    [SerializeField] Vector3 startingRotationL = new Vector3(0, -90, 0);
    [SerializeField] Vector3 observationRotationR = new Vector3(0, -180, 0);
    [SerializeField] Vector3 startingRotationR = new Vector3(0, -90, 0);

    [SerializeField] AnimationCurve pullUp;
    [SerializeField] AnimationCurve open;
    [SerializeField] float pullUpTimerMax = 0.2f;
    [SerializeField] float openTimerMax = 0.4f;
    private bool inObservation = false;
    private bool moving = false;


    private void Start()
    {
        dossier.transform.localPosition = startingPosition;
        dossierLeftPage.transform.localEulerAngles = startingRotationL;
        dossierRightPage.transform.localEulerAngles = startingRotationR;
    }

    IEnumerator pullUpDossier()
    {
        moving = true;
        dossier.transform.localPosition = startingPosition;
        dossierLeftPage.transform.localEulerAngles = startingRotationL;
        dossierRightPage.transform.localEulerAngles = startingRotationR;
        float pullUpTimer = 0;
        float openTimer = 0;
        while (pullUpTimer < pullUpTimerMax)
        {
            pullUpTimer += Time.deltaTime;

            Vector3 newY = (observationPosition - startingPosition) * pullUp.Evaluate(pullUpTimer/pullUpTimerMax);
            dossier.localPosition = startingPosition + newY;
            yield return null;
        }

        while (openTimer < openTimerMax)
        {
            openTimer += Time.deltaTime;
            Vector3 newYRotL = (observationRotationL - startingRotationL) * open.Evaluate(openTimer/openTimerMax);
            Vector3 newYRotR = (observationRotationR - startingRotationR) * open.Evaluate(openTimer / openTimerMax);
            dossierLeftPage.transform.localEulerAngles = startingRotationL + newYRotL;
            dossierRightPage.transform.localEulerAngles = startingRotationL + newYRotR;
            yield return null;

        }
        moving = false;
        GlobalState.instance.dossierUp = true;
    }

    IEnumerator putDownDossier()
    {
        moving = true;
        dossier.transform.localPosition = observationPosition;
        dossierLeftPage.transform.localEulerAngles = observationRotationL;
        dossierRightPage.transform.localEulerAngles = observationRotationR;
        float pullUpTimer = 0;
        float openTimer = 0;
        while (openTimer < openTimerMax)
        {
            openTimer += Time.deltaTime;
            Vector3 newYRotL = (startingRotationL - observationRotationL) * open.Evaluate(openTimer / openTimerMax);
            Vector3 newYRotR = (startingRotationR - observationRotationR) * open.Evaluate(openTimer / openTimerMax);
            dossierLeftPage.transform.localEulerAngles = observationRotationL + newYRotL;
            dossierRightPage.transform.localEulerAngles = observationRotationR + newYRotR;
            yield return null;
        }

        while (pullUpTimer < pullUpTimerMax)
        {
            pullUpTimer += Time.deltaTime;

            Vector3 newY = (startingPosition - observationPosition) * pullUp.Evaluate(pullUpTimer / pullUpTimerMax);
            dossier.localPosition = observationPosition + newY;
            yield return null;
        }
        moving = false;
        GlobalState.instance.dossierUp = false;
    }

    //IEnumerator lerpThenRoatate(bool lerpFirst,  Vector3 origin, Vector3 destination, AnimationCurve moveCurve, float moveTimeMax, Vector3)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !moving)
        {
            if (!GlobalState.instance.dossierUp)
            {
                StartCoroutine(pullUpDossier());
            }
            if (GlobalState.instance.dossierUp)
            {
                StartCoroutine(putDownDossier());
            }
        }
    }

}
