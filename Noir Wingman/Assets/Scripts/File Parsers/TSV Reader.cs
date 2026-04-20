using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TSVReader : MonoBehaviour
{
    public static TSVReader instance;
    public TextAsset newTSV;
    public string[,] loadedConversation;

    private void Awake()
    {
        instance = this;
    }


    public string[,] readTSV(TextAsset data)
    {
        string editData = data.text;
        string[] numRows = editData.Split("\n");
        string[][] list = new string[numRows.Length][];
        int maxCol = 0;

        for (int i = 0; i < numRows.Length; i++)
        {
            list[i] = numRows[i].Split("\t");
            if (list[i].Length > maxCol)
                maxCol = list[i].Length;
        }

        string[,] grid = new string[numRows.Length, maxCol];
        for (int x = 0; x < numRows.Length; x++)
        {
            for (int y = 0; y < maxCol; y++)
            {
                try
                {
                    grid[x - 1, y] = list[x][y];
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }
        }

        return grid;
    }

}
