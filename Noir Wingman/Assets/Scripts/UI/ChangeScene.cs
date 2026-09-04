using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeScene : ButtonMaster
{
    [SerializeField] string sceneToLoad;
    public override void PressedEffect()
    {
        SceneManager.LoadScene(sceneToLoad,0);
    }

    public override void Update()
    {
        if (mouseOver && Input.GetKeyDown(KeyCode.Mouse0))
        {
            PressedEffect();
        }
    }
}
