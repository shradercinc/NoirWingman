using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalState : MonoBehaviour
{
    public static GlobalState instance;
    public bool dossierUp = false;
    private void Awake()
    {
        instance = this;
    }
}
