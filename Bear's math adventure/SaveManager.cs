using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/* This is the SaveManager script that is attached to the DataHandler object in the game and this script saves all of the important data into a JSON 
 * format, which is not ideal in the long run but it works for a basic prototype.
 */
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [SerializeField]
    private SaveDataClass saveData;
    [SerializeField]
    private CellSaveClass cellSaveClass;
    [SerializeField]
    private float playerHealth;
    private int playerScore;

    //This is simply used to check that there is only 1 instance of the script running at a time to avoid issues.
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //This is called at the start of the game and is used to make a new file if there is none and if there already is one it is loaded instead.
    public void InitializeData()
    {
        string saveJSONPath = Application.persistentDataPath + "/save.json";
        if (!File.Exists(saveJSONPath))
        {
            saveData = new SaveDataClass();
            saveData.cellData = new List<CellSaveClass>();
            saveData.completedZones = new List<int>();
            saveData.LoadedZones = new List<int>();
            saveData.TriviaQuestionIDs = new List<string>();
            saveData.currentZone = 1;
        }
        else
        {
            LoadJSONSave();
        }
    }
    
    // When saving the data this function also checks player's score and health which are also noted down in the save file.
    public void SaveJSONSave()
    {
        saveData.playerHealth = playerHealth;
        saveData.playerScore = playerScore;

        string saveDataJSON = JsonUtility.ToJson(saveData);
        string jsonPath = Application.persistentDataPath + "/save.json";
        if (!File.Exists(jsonPath))
        {
            using FileStream stream = File.Create(jsonPath);
            stream.Close();
        }

        File.WriteAllText(jsonPath, saveDataJSON);
    }

    //And when needed this function loads the data from the JSON file and updates the current data to what was found in the file.
    public void LoadJSONSave()
    {
        string saveJSONPath = Application.persistentDataPath + "/save.json";
        if (File.Exists(saveJSONPath)) 
        {
            string saveJSONContent = File.ReadAllText(saveJSONPath);
            saveData = JsonUtility.FromJson<SaveDataClass>(saveJSONContent);

            playerHealth = saveData.playerHealth;
            playerScore = saveData.playerScore;
        }
    }

    //Simple fuction used to add Cell's data so that the Cell can be loaded later.
    public void AddCellData(CellSaveClass cell)
    {
        saveData.cellData.Add(cell);
    }

    //Used to pull the info from the saved data to load the Zone and it's Cells into what they were when the game was saved.
    public CellSaveClass GetCellData(string ID)
    {
        for(int i = 0;i < saveData.cellData.Count; i++)
        {
            if (saveData.cellData[i].cellID == ID)
            {
                return saveData.cellData[i];
            }
        }
        return null;
    }

    //This gets called when player moves between Cells and keeps track of where the player is and which entrance they entered from.
    public void SetPlayerLocation(string cellName, string entranceDirection, int zoneID)
    {
        //Debug.Log("Saved Player Location");
        saveData.currentCell = cellName;
        saveData.cellEntranceDirection = entranceDirection;
        saveData.currentZone = zoneID;
        SaveJSONSave();
    }

    //Another function used to load player's location, done separately from the Cell loading so that the game can load the Zone first and then
    //add the player to their correct spot.
    public bool LoadPlayerLocation(out string cell, out string entrance)
    {
        cell = saveData.currentCell;
        entrance = saveData.cellEntranceDirection;
        return true;
    }


    // From here there are few more functions for loading and adding specific information into the saveData format which then gets saved later.
    // Most of these are quite simple and they get called when needed by the other scripts.
    public int CheckCurrentZone()
    {
        return saveData.currentZone;
    }

    public void AddCompletedZone()
    {
        saveData.completedZones.Add(saveData.currentZone);
    }

    public bool CheckCompletedZone(int currentZoneNumber)
    {
        if (saveData.completedZones.Contains(currentZoneNumber))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckLoadedZones(int currentZoneNumber)
    {
        if (saveData.LoadedZones != null && saveData.LoadedZones.Contains(currentZoneNumber))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddLoadedZone(int currentZoneNumber)
    {
        saveData.LoadedZones.Add(currentZoneNumber);
    }

    public bool CheckTutorialCompletion()
    {
        return saveData.hasDoneTutorial;
    }

    public void SaveTutorialCompletion(bool state)
    {
        saveData.hasDoneTutorial = state;
    }

    public void SavePlayerHealth(float health)
    {
        playerHealth = health;
        saveData.playerHealth = playerHealth;
    }

    public float GetPlayerHealth() 
    { 
        return playerHealth;
    }

    public void SavePlayerScore(int score)
    {
        playerScore = score;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public void UpdateCellEnemyState(string cell, bool state)
    {
        for (int i = 0; i < saveData.cellData.Count; i++)
        {
            if (saveData.cellData[i].cellID == cell)
            {
                saveData.cellData[i].hasEnemy = state;
            }
        }
    }

    public string LoadTriviaQuestion(int zoneNumber)
    {
        //Debug.Log("Returning zonenumber: " + zoneNumber);
        //Debug.Log("Return save data: " + saveData.TriviaQuestionIDs[zoneNumber]);
        return saveData.TriviaQuestionIDs[zoneNumber];
    }

    public void SaveTriviaQuestion(string id)
    {
        //Debug.Log("Trying to save trivia");
        saveData.TriviaQuestionIDs.Add(id);
    }
}