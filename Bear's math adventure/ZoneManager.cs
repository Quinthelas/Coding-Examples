using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// ZoneManager handles creating the maze for each level and is mostly just used as an anchor point afterwards for the Cells to orient themselves from.
public class ZoneManager : MonoBehaviour
{
    public GameObject zoneStartingPoint { private set; get; }
    public Vector3 anchorCornerPosition { private set; get; }
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private GameObject dataManager;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    SaveManager saveManager;
    [SerializeField]
    private GameObject cameraHolderObject;
    [SerializeField]
    private int zoneNumber;
    [SerializeField]
    private int maxRows;
    [SerializeField]
    private int maxColumns;
    [SerializeField]
    private ZoneCellManager zoneExit;
    [SerializeField]
    private bool isStartingZone;
    [SerializeField]
    private bool isLastZone;
    [SerializeField]
    private int numberOfEnemies;


    void Start()
    {
        dataManager = FindFirstObjectByType<SaveManager>().gameObject;
        saveManager = FindFirstObjectByType<SaveManager>();
        BuildZone();
    }

    /* BuildZone function is a bit more complicated as when building the zone it has to first make a list of all the cells as it creates them. 
     * Each Cell in the first column is marked into a different list for potential starting points, and each Cell in last column is marked as
     * potential Exit into the next Zone. After all the Cells have been instantiated the function then goes through the Cells and gives them
     * their important information one by one depending on the location of the Cell. This way the Cells which do not have any neighbours above
     * them for example will get the other neighbours but the missing neighbour will be set to Null so that the Cell knows not to bother with
     * the missing neighbour. Once all the Cells have their basic information the function checks for save file and then tells the Cells to load
     * data if the save file exists, otherwise it will randomize enemies for the Cells.*/
    private void BuildZone()
    {
        List<GameObject> cells = new List<GameObject>();
        List<GameObject> StartCells = new List<GameObject>();
        List<GameObject> exitCells = new List<GameObject>();

        for (int r = 1; r <= maxRows; r++)
        {
            for (int c = 1; c <= maxColumns; c++)
            {
                GameObject newCell = Instantiate(cellPrefab, this.gameObject.transform);
                newCell.transform.SetParent(this.gameObject.transform, false);
                newCell.name = "Zone" + zoneNumber.ToString() + " " + r.ToString() + "_" + c.ToString();
                newCell.GetComponent<ZoneCellManager>().SetRowAndColumn(r, c);
                cells.Add(newCell);

                if (c == maxColumns)
                {
                    exitCells.Add(newCell);
                }

                if (c == 1)
                {
                    StartCells.Add(newCell);
                }
            }
        }

        gameObject.GetComponent<GridLayoutGroup>().CalculateLayoutInputHorizontal();
        gameObject.GetComponent<GridLayoutGroup>().CalculateLayoutInputVertical();
        gameObject.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();
        gameObject.GetComponent<GridLayoutGroup>().SetLayoutVertical();


        string saveJSONPath = Application.persistentDataPath + "/save.json";
        if (!File.Exists(saveJSONPath) || !saveManager.CheckLoadedZones(zoneNumber))
        {
            zoneExit = exitCells[Random.Range(0, exitCells.Count)].GetComponent<ZoneCellManager>();
            zoneStartingPoint = StartCells[Random.Range(0, StartCells.Count)];
            zoneExit.SetExitPoint(true, isLastZone);
            zoneStartingPoint.GetComponent<ZoneCellManager>().SetStartingPoint();
        }
       

        for(int i = 0; i < cells.Count; i++)
        {
            int row = cells[i].GetComponent<ZoneCellManager>().rowIndex;
            int column = cells[i].GetComponent<ZoneCellManager>().columnIndex;

            if (row <= 1 && column <= 1)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, null, null, cells[i + maxColumns], cells[i+1], saveManager, player);
            }
            else if (row <= 1 && column >= maxColumns)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, null, cells[i - 1], cells[i + maxColumns], null, saveManager, player);
            }
            else if (row >= maxRows && column <= 1)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], null, null, cells[i + 1], saveManager, player);
            }
            else if (row >= maxRows && column >= maxColumns)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], cells[i - 1], null, null, saveManager, player);
            }
            else if(row <= 1)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, null, cells[i - 1], cells[i + maxColumns], cells[i + 1], saveManager, player);
            }
            else if (column <= 1)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], null, cells[i + maxColumns], cells[i + 1], saveManager, player);
            }
            else if (row >= maxRows)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], cells[i - 1], null, cells[i + 1], saveManager, player);
            }
            else if (column >= maxColumns)
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], cells[i - 1], cells[i + maxColumns], null, saveManager, player);
            }
            else
            {
                string id = "Zone" + zoneNumber.ToString() + " " + row.ToString() + "_" + column.ToString();
                cells[i].GetComponent<ZoneCellManager>().BuildCell(this.gameObject, zoneNumber, dataManager, id, cells[i - maxColumns], cells[i - 1], cells[i + maxColumns], cells[i + 1], saveManager, player);
            }

            
        }

        

        if (File.Exists(saveJSONPath) && saveManager.CheckLoadedZones(zoneNumber))
        {
            saveManager.LoadJSONSave();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<ZoneCellManager>().LoadData();
            }
        }
        else
        {

            for (int e = 0; e < numberOfEnemies; e++)
            {
                GameObject child = gameObject.transform.GetChild(Random.Range(0, gameObject.transform.childCount)).gameObject;

                if (child != zoneStartingPoint && child != zoneExit && child.GetComponent<ZoneCellManager>().hasEnemy == false)
                {
                    child.GetComponent<ZoneCellManager>().SetEnemyState(true);
                    Debug.Log("Enemy created: " + e.ToString() + "to " + child.name);
                }
                else
                {
                    e--;
                }
            }

            //This was a bit tricky as due to how the Cells are positioned in grid form I had to get the corner's position for the Cells
            //to know their position to which pull the Camera to as otherwise it was acting oddly.

            zoneStartingPoint.GetComponent<ZoneCellManager>().NextPathReceived(0, this.gameObject, 0);
            Vector3[] corners = new Vector3[4];
            gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
            //Debug.Log("Starting Point: " +  zoneStartingPoint.transform.position);
            anchorCornerPosition = corners[2];
            zoneStartingPoint.GetComponent<ZoneCellManager>().PlayerEnteringCell("West");
            //cameraHolderObject.transform.position = zoneStartingPoint.transform.position + corners[2];
            //player.transform.position = zoneStartingPoint.transform.position + corners[2];
            //saveManager.SaveJSONSave();
        }
    }

}
