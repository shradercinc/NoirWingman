using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;

public class CSVReader : MonoBehaviour
{
    public TextAsset newCSV;

    private void Awake()
    {
        
        //reminder: i = y axis (row), j = x axis (Collumn)
        string[,] chart = readCSV(newCSV);
        string dialog = "";
        print(chart.GetLength(0));
        print(chart.GetLength(1));
        for (int i = 0; i < chart.GetLength(0); i++)
        {
            for (int j = 0; j < chart.GetLength(1); j++)
            {
                //print("for " + i + "/" + j + ":" + chart[i, j]);
                dialog += chart[i, j];
                
            }
            print(dialog);
            dialog = "";
        }
        
    }


    public string[,] readCSV(TextAsset data)
    {
        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "").Replace("\"","").Replace("\n","").Replace("]","");

        string[] numRows = editData.Split("[");
        string[][] list = new string[numRows.Length][];
        int maxCol = 0;

        for (int i = 0; i < numRows.Length; i++)
        {
            list[i] = numRows[i].Split(",");
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
