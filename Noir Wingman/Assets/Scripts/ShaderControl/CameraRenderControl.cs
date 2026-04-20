using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRenderControl : MonoBehaviour
{
    Camera myCam;
    [SerializeField] Shader filterShader;

    private void Awake()
    {
        myCam = GetComponent<Camera>();
        myCam.RenderWithShader(filterShader, "");
        myCam.enabled = true;

    }
}
