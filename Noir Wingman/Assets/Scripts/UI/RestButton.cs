using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestButton : ButtonMaster
{
    public override void PressedEffect()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
