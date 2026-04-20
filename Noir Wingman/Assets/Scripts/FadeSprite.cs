using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSprite : MonoBehaviour
{
    Image myImage;
    [SerializeField] GameObject expressionParent;   

    private void Start()
    {
        myImage = GetComponent<Image>();
        transform.localScale = expressionParent.transform.localScale;
        transform.localPosition = expressionParent.transform.localPosition;
        myImage.color = new Color(1f, 1f, 1f, 0f);
    }
}
