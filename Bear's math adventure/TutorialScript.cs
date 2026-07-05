using UnityEngine;

/* This is just a simple short code that checks if the player has completed the tutorial already and if not it activates the tutorial as 
 * long as the zone is the starting point as well. The check happens by sending a request to SaveManager which keeps track of the state.
 */
public class TutorialScript : MonoBehaviour
{
    [SerializeField]
    private SaveManager saveManager;
    [SerializeField]
    private GameObject tutorialScreen;
    [SerializeField]
    private PlayerScript player;

    void Start()
    {
        
        saveManager = FindFirstObjectByType<SaveManager>();
        player = FindFirstObjectByType<PlayerScript>();
        if (!saveManager.CheckTutorialCompletion() && gameObject.transform.parent.parent.GetComponent<ZoneCellManager>().isStartingPoint)
        {
            tutorialScreen.SetActive(true);
            player.gameObject.SetActive(false);
        }
    }

    public void TutorialCompleted()
    {
        tutorialScreen.SetActive(false);
        saveManager.SaveTutorialCompletion(true);
        player.gameObject.SetActive(true);
    }
}
