using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/* This is the script connected to player's character, only needed for some basic stuff such as movement,
 * keeping track of score and player health.*/
public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private int scorePoints;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private HealthbarScript healthbar;
    [SerializeField]
    private SaveManager saveManager;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private bool isAllowedToMove;
    [SerializeField]
    private int playerSpeed;
    [SerializeField]
    private Vector3 movementDirection;
    /* Here in Start() the script grabs the saveManager script to check if there is any save and then loads the health and score points from it.*/
    void Start()
    {
        saveManager = FindFirstObjectByType<SaveManager>();
        currentHealth = saveManager.GetPlayerHealth();

        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }
        healthbar.SetHealthBarValue(currentHealth);
        scorePoints = saveManager.GetPlayerScore();
        scoreText.text = scorePoints.ToString();
    }

    /* FixedUpdate() keeps checking for movement and for the direction of movement, this is affected by playerSpeed variable which can
     * be set in editor.*/
    void FixedUpdate()
    {
        if (movementDirection == Vector3.zero)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
            if (movementDirection.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (gameObject.GetComponent<SpriteRenderer>().flipX == true && movementDirection.x > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = movementDirection * playerSpeed;
    }
    /* This function normalizes the direction of the movement so that how much the player moves the stick outside it's minimum range does not
     * affect the speed.*/
    public void ReceiveTouchStickInput(Vector3 direction)
    {
        movementDirection = direction.normalized;
    }

    /* This function is used to deal damage to the player and is called when wrong answer is selected in the trivia questions
     * This and next heal function could be merged into one function for streamlining but I decided to keep them seperate for
     * simplicity.*/
    public void DamagePlayer(float damage)
    {
        currentHealth -= damage;
        healthbar.SetHealthBarValue(currentHealth);
        saveManager.SavePlayerHealth(currentHealth);
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    /* This function is called to heal the player, used when selecting the right answer for trivia question. */
    public void HealPlayer(float healAmount)
    {
        currentHealth -= healAmount;
        healthbar.SetHealthBarValue(currentHealth);
        saveManager.SavePlayerHealth(currentHealth);
    }
    /* This function changes the player's score depending if it was right or wrong answer, each trivia question can
     * edit point amounts in editor for negative points for wrong answer and positive points for right, also checks that
     * player can't go into negative points.*/
    public void ChangeScore(int points)
    {
        scorePoints += points;

        if (scorePoints < 0)
        {
            scorePoints = 0;
        }

        scoreText.text = scorePoints.ToString();
        saveManager.SavePlayerScore(scorePoints);
        Debug.Log("Current points are: " + scorePoints.ToString());
        
    }
}
