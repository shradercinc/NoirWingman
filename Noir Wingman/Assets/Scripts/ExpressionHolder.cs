using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionHolder : MonoBehaviour
{
    public static ExpressionHolder instance;
    //public SerializedDictionary<string,Dictionary<string,Sprite>> characterList;
    [SerializedDictionary("Character Name", "Expression Dictionary")] public SerializedDictionary<string, SerializedDictionary<string, Sprite>> characterList;

    private void Start()
    {
        instance = this;
    }

    public Sprite ExpressionGrabber(string charName, string exprType)
    {
        return characterList[charName][exprType];
    }

}
