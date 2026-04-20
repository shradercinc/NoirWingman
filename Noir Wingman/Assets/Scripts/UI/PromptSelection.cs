using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PromptSelection : MonoBehaviour
{
    Vector3 baseSize;
    [SerializeField] float hoverMod;
    promptPrefabController parentController;
    public bool mouseOver;

    private void Awake()
    {
        parentController = gameObject.GetComponentInParent<promptPrefabController>();
        baseSize = transform.localScale;
    }


}
