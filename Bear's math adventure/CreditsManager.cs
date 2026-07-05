using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script is called when the game ends and the player is shown their score, this script also deletes the save file since the run is complete.
public class CreditsManager : MonoBehaviour
{
    [SerializeField]
    private SaveManager saveManager;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private int maxScore;

    private void Start()
    {
        saveManager = FindFirstObjectByType<SaveManager>();
        scoreText.text = "Loppu pisteet: " + saveManager.GetPlayerScore() + "/" + maxScore;
    }
    public void ReturnToMainMenu()
    {
        string savePath = Application.persistentDataPath + "/save.json";
        Debug.Log("Trying to delete file");
        Debug.Log(Application.persistentDataPath);
        File.Delete(savePath);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        string savePath = Application.persistentDataPath + "/save.json";
        Debug.Log("Trying to delete file");
        Debug.Log(Application.persistentDataPath);
        File.Delete(savePath);
        Application.Quit();
    }
}
