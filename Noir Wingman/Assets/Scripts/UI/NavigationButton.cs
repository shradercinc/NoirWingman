using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationButton : ButtonMaster
{
    [SerializeField] GameObject home;
    [SerializeField] GameObject destination;


    public override void PressedEffect()
    {
        print("Navigation");
        destination.SetActive(true);
        home.SetActive(false);
    }
}
