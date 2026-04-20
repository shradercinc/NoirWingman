using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMaster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseOver = false;
    [SerializeField] bool disabledOnStart = false;
    [SerializeField] protected Image backgroundRect;
    [SerializeField] protected float hoverMod = 1.05f;
    protected Vector3 baseSize;

    // Start is called before the first frame update

    public virtual void Awake()
    {
        if (backgroundRect != null)
        {
            baseSize = backgroundRect.transform.localScale;
        }
    }

    public virtual void Start()
    {
        if (disabledOnStart)
        {
            enabled = false;
        }
    }

    public virtual void PressedEffect()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {

        if (mouseOver && Input.GetKeyDown(KeyCode.Mouse0) && !GlobalState.instance.dossierUp)
        {
            PressedEffect();
        }
    }


    private void OnDisable()
    {
        mouseOver = false;
    }



    //for world space
    public virtual void OnMouseEnter()
    {
        mouseOver = true;
        backgroundRect.transform.localScale = baseSize * hoverMod;
    }

    public virtual void OnMouseExit()
    {
        mouseOver = false;
        backgroundRect.transform.localScale = baseSize;
    }


    //for canvas space
    public virtual void OnPointerEnter(PointerEventData data)
    {
        mouseOver = true;
        backgroundRect.transform.localScale = baseSize * hoverMod;
    }

    public virtual void OnPointerExit(PointerEventData data)
    {
        mouseOver = false;
        backgroundRect.transform.localScale = baseSize;
    }
}
