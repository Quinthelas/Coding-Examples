using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI taskText;
    [SerializeField]
    private GameObject crocPrefab;
    [SerializeField]
    List<GameObject> crocList;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private GameObject playerEntrance;
    [SerializeField]
    private GameObject[] exits;
    private int numberA;
    private int numberB;
    private int numberC;
    private float goalNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (!gameObject.transform.parent.parent.GetComponent<ZoneCellManager>().hasEnemy) //!gameObject.transform.parent.parent.GetComponent<ZoneCellManager>().hasEnemy
        {
            gameObject.SetActive(false);
        }
        else
        {
            crocList = new List<GameObject>();
            gameObject.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
            Multiplication();
            taskText.transform.parent.gameObject.SetActive(true);
        }
    }


    private void Multiplication()
    {
        numberA = Random.Range(1, 11);
        numberB = Random.Range(1, 11);
        numberC = numberA * numberB;

        int random = Random.Range(1, 3);

        switch (random)
        {
            case 1:
                goalNumber = numberA;
                taskText.text = "Löydä puuttuva luku _ * " + numberB.ToString() + " = " + numberC.ToString();
                break;
            case 2:
                goalNumber = numberB;
                taskText.text = "Löydä puuttuva luku " + numberA.ToString() + " * _ = " + numberC.ToString();
                break;
            case 3:
                goalNumber = numberC;
                taskText.text = "Löydä puuttuva luku " + numberA.ToString() + " * " + numberB.ToString() + " = _";
                break;
        }

        SetCrocodiles();
    }


    private void SetCrocodiles()
    {
        List<float> newValues = new List<float>();

        for (int i = 0; i<spawnPoints.Length; i++)
        {
            if (i == 0)
            {
                newValues.Add(goalNumber);
            }
            else
            {
                newValues.Add((int)Random.Range(goalNumber - 4, goalNumber + 6));
            }
        }



        

        for (int i=0; i < spawnPoints.Length; i++)
        {
            int newValue = (int)newValues[Random.Range(0, newValues.Count)];
            
            bool isFlipped = Random.value > 0.5f;

            

            GameObject newCroc = Instantiate(crocPrefab, spawnPoints[i].transform);
            newCroc.transform.SetParent(transform); 
            
            if (isFlipped)
            {
                newCroc.transform.GetChild(0).transform.rotation = new Quaternion(0, 180, 0, 0);
            }

            float randomSizeValue = Random.Range(4.2f, 4.5f);
            newCroc.transform.localScale = new Vector3(randomSizeValue,randomSizeValue,randomSizeValue);
            newCroc.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = newValue.ToString();
            newValues.Remove(newValue);
            newCroc.transform.GetChild(1).GetComponent<Canvas>().worldCamera = Camera.main;
            crocList.Add(newCroc);
        }
    }

    public bool CheckMath(int value, GameObject crocodile)
    {
        if (value == goalNumber)
        {
            foreach (var croc in crocList)
            {
                croc.GetComponent<CrocodileScript>().RunAway();
            }

            for (int i = 0; i < exits.Length; i++)
            {
                exits[i].gameObject.SetActive(true);
            }

            gameObject.transform.parent.parent.GetComponent<ZoneCellManager>().SetEnemyState(false);
            taskText.transform.parent.gameObject.SetActive(false);

            return true;
        }
        else
        {
            crocList.Remove(crocodile);
        }
            return false;
    }

    public void CloseExits(GameObject entrance)
    {


        for (int i = 0; i < exits.Length; i++)
        {
            if (exits[i] != entrance)
            {
                exits[i].gameObject.SetActive(false);
            }
        }
    }
}
