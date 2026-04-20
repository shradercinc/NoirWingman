using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class DataLoader : MonoBehaviour
{
    public static DataLoader instance;
    public bool downloadOn;
    private const string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private const string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        StartCoroutine(DownloadLevelSheet("OBL!A1:C3", "1rHnzNGv1H4TlnnvwC48CoM-8AL_GoipVMW5pJY3quXE", "OBL"));
    }

    IEnumerator DL(string ID, string range, string fileName)
    {
        if (downloadOn)
        {
            string url = $"{baseUrl}{ID}/values/{range}?key={apiKey}";
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(range);
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                string filePath = $"Assets/Sheets/{fileName}.json";
                File.WriteAllText($"{filePath}", www.downloadHandler.text);
                Debug.Log($"downloaded {fileName} from the internet");

                string[] allLines = File.ReadAllLines($"{filePath}");
                List<string> modifiedLines = allLines.ToList();
                modifiedLines.RemoveRange(1, 3);
                File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
            }
        }
    }

    // ID is the link code ID in the sheet link, range is the page in use
    public IEnumerator DownloadLevelSheet(string range, string sheetID, string fileName)
    {
        yield return DL(sheetID, range, fileName);
    }

}
