using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/* ZoneCells are the objects that make up each zone of the game and this script is connected to each one and manages a lot of information
 * for them. */
public class ZoneCellManager : MonoBehaviour
{
    [SerializeField]
    private GameObject zoneManager;
    [SerializeField]
    private CellPathPrefabHandler dataManager;
    public int rowIndex { get; private set; }
    public int columnIndex { get; private set; }
    [SerializeField]
    private GameObject northNeighbourObject;
    [SerializeField]
    private GameObject westNeighbourObject;
    [SerializeField]
    private GameObject southNeighbourObject;
    [SerializeField]
    private GameObject eastNeighbourObject;
    [SerializeField]
    private GameObject[] exits;
    private GameObject player;
    [SerializeField]
    private SaveManager saveManager;
    private CellSaveClass cellData;
    public bool hasPath {  get; private set; }
    [SerializeField]
    private int neighboursNotVisitedCount;
    [SerializeField]
    private string cellID;
    [SerializeField]
    private string pathType;
    [SerializeField]
    private bool hasTreasure;
    public bool hasEnemy {  get; private set; }
    public bool isExitToNextZone { get; private set; }
    [SerializeField]
    private GameObject previousZoneCell;
    private int exitCount;
    [SerializeField]
    private bool hasNorthExit;
    [SerializeField]
    private bool hasWestExit;
    [SerializeField]
    private bool hasSouthExit;
    [SerializeField]
    private bool hasEastExit;
    public int currentZoneID { private set; get; }
    [SerializeField]
    private GameObject entrance;
    [SerializeField]
    private int pathLength;
    [SerializeField]
    private int timesGoneStraight;
    [SerializeField]
    private GameObject oppositePathFromPrevious;
    private GameObject newPath;
    private bool hasPathTile;
    public bool isStartingPoint { private set; get; }
    private bool isLastZone;

    //This is called when adding Cells into a zone and attaches each Cell's location to it's information.
    //This function is called before the Builder function.
    public void SetRowAndColumn(int row, int column)
    {
        rowIndex = row;
        columnIndex = column;
    }

    /*This function is used to build the Cell used for the zone and attaches all the important associated information
     * such as the neighbours, dataManager for fetching saved data and if the Cell has enemies inside it as an obstacle or not.*/
    public void BuildCell(GameObject zone,int zoneID, GameObject dataM, string ID, GameObject nNeighbour, GameObject wNeighbour,
        GameObject sNeighbour, GameObject eNeighbour, SaveManager saveObject, GameObject playerObject)
    {
        zoneManager = zone;
        dataManager = dataM.GetComponent<CellPathPrefabHandler>();
        cellID = ID;
        northNeighbourObject = nNeighbour;
        westNeighbourObject = wNeighbour;
        southNeighbourObject = sNeighbour;
        eastNeighbourObject = eNeighbour;
        saveManager = saveObject;
        player = playerObject;
        hasEnemy = false;
        currentZoneID = zoneID;

        saveManager.UpdateCellEnemyState(ID, hasEnemy);
    }

    /* After all the Cells have been built this function gets called by the ZoneManager and it actually skips
     * to generating the path in the next function. If this function was called by a Cell instead of the ZoneManager however
     * it will add the exit between the current Cell and Cell which called this to the count and check which path is opposite of the
     * previous Cell after which the PathGeneration() function gets called as usual.*/
    public void NextPathReceived(int receivedPathLength, GameObject obtainedPreviousZoneCell, int timesRepeatedDirection)
    {
        //Debug.Log("Path received");
        pathLength = receivedPathLength + 1;
        previousZoneCell = obtainedPreviousZoneCell;
        timesGoneStraight = timesRepeatedDirection;

        if (obtainedPreviousZoneCell != zoneManager)
        {
            if (obtainedPreviousZoneCell == northNeighbourObject)
            {
                hasNorthExit = true;
                exitCount += 1;
                oppositePathFromPrevious = southNeighbourObject;
            }
            else if (obtainedPreviousZoneCell == westNeighbourObject)
            {
                hasWestExit = true;
                exitCount += 1;
                oppositePathFromPrevious = eastNeighbourObject;
            }
            else if (obtainedPreviousZoneCell == southNeighbourObject)
            {
                hasSouthExit = true;
                exitCount += 1;
                oppositePathFromPrevious = northNeighbourObject;
            }
            else if (obtainedPreviousZoneCell == eastNeighbourObject)
            {
                hasEastExit = true;
                exitCount += 1;
                oppositePathFromPrevious = westNeighbourObject;
            }
        }

        PathGeneration();
    }

    /* This function starts by resetting the count of how many neighbouring Cells have been visited and then it checks if any of those
     * neighbouring Cells have a path already going through it, then function checks how many times the path has gone straight
     * so far, to prevent the maze from going all the way straight through the map by accident at first. Afterwards all acceptable neighbours
     * get added to a list from which the next Cell along the current path is chosen. If no acceptable Cell is found the previous Cell
     * along the path is called instead and it's ReturnPathPoint() function is called.*/
    private void PathGeneration()
    {
        if (hasPath == false)
        {
            hasPath = true;
        }
        neighboursNotVisitedCount = 0;

        if (northNeighbourObject != null && northNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
        {
            neighboursNotVisitedCount++;
        }
        if (westNeighbourObject != null && westNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
        {
            neighboursNotVisitedCount++;
        }
        if (southNeighbourObject != null && southNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
        {
            neighboursNotVisitedCount++;
        }
        if (eastNeighbourObject != null && eastNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
        {
            neighboursNotVisitedCount++;
        }

        List<GameObject> neighbours = new();

        if (timesGoneStraight >= 2 && neighboursNotVisitedCount > 1)
        {
            if (northNeighbourObject != null && northNeighbourObject != oppositePathFromPrevious && northNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(northNeighbourObject);
            }
            if (westNeighbourObject != null && westNeighbourObject != oppositePathFromPrevious && westNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(westNeighbourObject);
            }
            if (southNeighbourObject != null && southNeighbourObject != oppositePathFromPrevious && southNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(southNeighbourObject);
            }
            if (eastNeighbourObject != null && eastNeighbourObject != oppositePathFromPrevious && eastNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(eastNeighbourObject);
            }
            timesGoneStraight = 0;
        }
        else
        {
            if (northNeighbourObject != null && northNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(northNeighbourObject);
            }
            if (westNeighbourObject != null && westNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(westNeighbourObject);
            }
            if (southNeighbourObject != null && southNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(southNeighbourObject);
            }
            if (eastNeighbourObject != null && eastNeighbourObject.GetComponent<ZoneCellManager>().hasPath == false)
            {
                neighbours.Add(eastNeighbourObject);
            }
        }

        int listLength = neighbours.Count;
        //Debug.Log(listLength);
        if (listLength > 0)
        {
            GameObject newDirection = neighbours[Random.Range(0, listLength)];
            //Debug.Log(newDirection.name);

            if(newDirection == oppositePathFromPrevious)
            {
                timesGoneStraight += 1;
            }

            if(newDirection == northNeighbourObject)
            {
                hasNorthExit = true;
                exitCount += 1;
                neighboursNotVisitedCount--;
                newDirection.GetComponent<ZoneCellManager>().NextPathReceived(pathLength, this.gameObject, timesGoneStraight);
            }
            else if (newDirection == westNeighbourObject)
            {
                hasWestExit = true;
                exitCount += 1;
                neighboursNotVisitedCount--;
                newDirection.GetComponent<ZoneCellManager>().NextPathReceived(pathLength, this.gameObject, timesGoneStraight);
            }
            else if(newDirection == southNeighbourObject)
            {
                hasSouthExit = true;
                exitCount += 1;
                neighboursNotVisitedCount--;
                newDirection.GetComponent<ZoneCellManager>().NextPathReceived(pathLength, this.gameObject, timesGoneStraight);
            }
            else if (newDirection == eastNeighbourObject)
            {
                hasEastExit = true;
                exitCount += 1;
                neighboursNotVisitedCount--;
                newDirection.GetComponent<ZoneCellManager>().NextPathReceived(pathLength, this.gameObject, timesGoneStraight);
            }

        }
        else if(previousZoneCell != null && previousZoneCell != zoneManager)
        {
            previousZoneCell.GetComponent<ZoneCellManager>().ReturnPathPoint();
        }
    }

    /* ReturnPathPoint() Gets called when the path can't find a suitable neighbour to move onto next and the ZoneManager was not the previous object. 
     * Then the Cell calls up the previous Cell and returns to it. First it is checked if we are at the starting point with all of the neighbours
     * visited or null. If all neighbours have been visited and/or are null then the next part of the building is called, laying the Tiles onto the
     * path. If not then the function just calls up the PathGeneration() function again and continues working.*/
    public void ReturnPathPoint()
    {
        if(gameObject == zoneManager.GetComponent<ZoneManager>().zoneStartingPoint && (northNeighbourObject == null || northNeighbourObject.GetComponent<ZoneCellManager>().hasPath == true) && (southNeighbourObject == null || southNeighbourObject.GetComponent<ZoneCellManager>().hasPath == true) && eastNeighbourObject.GetComponent<ZoneCellManager>().hasPath == true)
        {
            LayingPrefabTiles();
        }
        else
        {
            PathGeneration();
        }
    }

    /* Once the path has been generated for the whole Zone, this function gets called. First function checks if the Cell has a Tile, then the 
     * amount of exits is checked and which exits are available, this determines which type of prefab is needed for this particular Cell and 
     * then the DataManager is called to grab a suitable Tile which is instantiated and connected to the Cell. Afterwards the Cell's data is 
     * saved and the neighbouring Cells are called to grab their Tiles.*/
    public void LayingPrefabTiles()
    {
        if (hasPathTile == false)
        {
            switch (exitCount)
            {
                case 0:
                    break;
                case 1:
                    if (hasNorthExit)
                    {
                        pathType = "NorthDeadEnd";
                        break;
                    }
                    else if (hasWestExit)
                    {
                        pathType = "WestDeadEnd";
                        break;
                    }
                    else if (hasSouthExit)
                    {
                        pathType = "SouthDeadEnd";
                        break;
                    }
                    else if (hasEastExit)
                    {
                        pathType = "EastDeadEnd";
                        break;
                    }
                    break;

                case 2:
                    if (hasNorthExit)
                    {
                        if (hasWestExit)
                        {
                            pathType = "NorthWestCorner";
                            break;
                        }
                        else if (hasSouthExit)
                        {
                            pathType = "VerticalStraight";
                            break;
                        }
                        else if (hasEastExit)
                        {
                            pathType = "NorthEastCorner";
                            break;
                        }
                    }
                    else if (hasWestExit && hasEastExit)
                    {
                        pathType = "HorizontalStraight";
                        break;
                    }
                    else if (hasSouthExit)
                    {
                        if (hasWestExit)
                        {
                            pathType = "SouthWestCorner";
                            break;
                        }
                        else if (hasEastExit)
                        {
                            pathType = "SouthEastCorner";
                            break;
                        }
                    }
                    break;

                case 3:
                    if (!hasSouthExit)
                    {
                        pathType = "NorthIntersection";
                        break;
                    }
                    else if (!hasEastExit)
                    {
                        pathType = "WestIntersection";
                        break;
                    }
                    else if (!hasNorthExit)
                    {
                        pathType = "SouthIntersection";
                        break;
                    }
                    else if (!hasWestExit)
                    {
                        pathType = "EastIntersection";
                        break;
                    }
                    break;

                case 4:

                    pathType = "CrossRoad";
                    break;
            }

            newPath = Instantiate(dataManager.PathTileSelector(pathType));
            newPath.transform.SetParent(transform, false);
            newPath.transform.position = transform.position;

            SaveData();

            hasPathTile = true;

            if (southNeighbourObject != null)
            {
                southNeighbourObject.GetComponent<ZoneCellManager>().LayingPrefabTiles();
            }
            if (eastNeighbourObject != null)
            {
                eastNeighbourObject.GetComponent<ZoneCellManager>().LayingPrefabTiles();
            }
            if (northNeighbourObject != null)
            {
                northNeighbourObject.GetComponent<ZoneCellManager>().LayingPrefabTiles();
            }
        }
    }

    /* This function is called if the Cell is the exit to the next Zone and this function sets the Cell as the exit
     * and adds information to the Cell if the current Zone it's in is the last Zone.*/
    public void SetExitPoint(bool isExit, bool isLast)
    {
        isExitToNextZone = isExit;
        hasEastExit = true;
        exitCount += 1;
        isLastZone = isLast;
    }

    /* This function is called when marking the Cell as a starting point for current Zone, and if it's not the first Zone an exit
     * is added for moving back to previous Zone.*/
    public void SetStartingPoint()
    {
        if(currentZoneID > 1)
        {
            hasWestExit = true;
            exitCount += 1;
        }
        isStartingPoint = true;
    }

    /* Function for saving Cell's data into the JSON file managed by the SaveManager. Contains all the information needed for loading
     * the game and continuing where the player left off. */
    private void SaveData()
    {
        cellData = new CellSaveClass
        {
            cellID = cellID,
            tileID = newPath.GetComponent<TileDataScript>().GetID(),
            pathType = pathType,
            hasTreasure = hasTreasure,
            hasEnemy = hasEnemy,
            isExit = isExitToNextZone,
            isLast = isLastZone,
            isStart = isStartingPoint
        };

        saveManager.AddCellData(cellData);
    }

    /* LoadData() is used to load all the relevant information the Cell needs to keep track of what is inside it and what kind of Tile
     * is used for this Cell. After loading the details the function instantiates the Tile for the Cell and then checks the player location.
     * If the Cell is the same as the last known location for player, the player gets teleported to the Cell. */
    public void LoadData()
    {
        CellSaveClass savedData = saveManager.GetCellData(cellID);
        hasEnemy = savedData.hasEnemy;
        hasTreasure = savedData.hasTreasure;
        isExitToNextZone = savedData.isExit;
        isLastZone = savedData.isLast;
        isStartingPoint = savedData.isStart;
        pathType = savedData.pathType;
        string tileID = savedData.tileID;
        hasPathTile = true;

        newPath = Instantiate(dataManager.FetchTileByID(tileID, pathType));
        newPath.transform.SetParent(transform, false);
        newPath.transform.position = transform.position;

        string exit;
        string cell;
        saveManager.LoadPlayerLocation(out cell, out exit);

        if (cell == cellID)
        {
            PlayerEnteringCell(exit);
        }
    }

    /* This function is used to move the player from current Cell to the next and it first updates the save state of this Cell in case there were
     * enemies that the player had defeated. Then it checks which direction the player is going which is given by the invisible objects at each
     * entrance on the Tile and the function compares the direction to the cases given in the function. Usually it just moves player as usual and
     * then calls the next Cell's PlayerEnteringCell() function, but if the Cell is the starting point or the exit to the next Cell then the
     * function loads the relevant Zone instead or the Credits scene if the Zone was already the last one.*/
    public void PlayerExitingCell(string exitDirection)
    {
        saveManager.UpdateCellEnemyState(cellID, hasEnemy);

        switch (exitDirection)
        {
            case "North":
                northNeighbourObject.GetComponent<ZoneCellManager>().PlayerEnteringCell("South");
                break;

            case "West":

                if (isStartingPoint)
                {
                    int previousZoneID = currentZoneID -= 1;
                    string previousZone = "Zone" + previousZoneID;
                    SceneManager.LoadScene(previousZone);
                }
                else
                {
                    westNeighbourObject.GetComponent<ZoneCellManager>().PlayerEnteringCell("East");
                }

                break;

            case "South":
                southNeighbourObject.GetComponent<ZoneCellManager>().PlayerEnteringCell("North");
                break;

            case "East":

                Debug.Log("isExit = " + isExitToNextZone);
                Debug.Log("isLast = " + isLastZone);
                if (isExitToNextZone && !isLastZone)
                {
                    int nextZoneID = currentZoneID + 1;
                    string nextZone = "Zone" + nextZoneID;
                    SceneManager.LoadScene(nextZone);
                }
                else if(isExitToNextZone && isLastZone)
                {
                    SceneManager.LoadScene("Credits");
                }
                else
                {
                    eastNeighbourObject.GetComponent<ZoneCellManager>().PlayerEnteringCell("West");
                }
                break;

        }
        
    }

    /* When player Enters a Cell this function gets called, it moves the Camera and player object to the relevant Cell, updates player's location
     * to the SaveManager which then saves it. This function also then marks down the entrance on the side the Player came from and if there are
     * enemies in this Cell it then closes the other entrances so that the player has to defeat the enemies or move back to previous Cell. */
    public void PlayerEnteringCell(string direction)
    {
        switch (direction)
        {
            case "North":
                Camera.main.gameObject.transform.parent.position = gameObject.transform.position + zoneManager.GetComponent<ZoneManager>().anchorCornerPosition;
                player.transform.position = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).transform.position;
                saveManager.SetPlayerLocation(cellID, "North", currentZoneID);
                entrance = gameObject.transform.GetChild(0).GetChild(0).gameObject;

                if (hasEnemy)
                {
                    gameObject.transform.GetChild(0).GetChild(5).GetComponent<EnemyManagerScript>().CloseExits(entrance);
                }

                break;
            case "West":
                Camera.main.gameObject.transform.parent.position = gameObject.transform.position + zoneManager.GetComponent<ZoneManager>().anchorCornerPosition;
                player.transform.position = gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform.position;
                saveManager.SetPlayerLocation(cellID, "West", currentZoneID);
                entrance = gameObject.transform.GetChild(0).GetChild(1).gameObject;

                if (hasEnemy)
                {
                    gameObject.transform.GetChild(0).GetChild(5).GetComponent<EnemyManagerScript>().CloseExits(entrance);
                }

                break;
            case "South":
                Camera.main.gameObject.transform.parent.position = gameObject.transform.position + zoneManager.GetComponent<ZoneManager>().anchorCornerPosition;
                player.transform.position = gameObject.transform.GetChild(0).GetChild(2).GetChild(0).transform.position;
                saveManager.SetPlayerLocation(cellID, "South", currentZoneID);
                entrance = gameObject.transform.GetChild(0).GetChild(2).gameObject;

                if (hasEnemy)
                {
                    gameObject.transform.GetChild(0).GetChild(5).GetComponent<EnemyManagerScript>().CloseExits(entrance);
                }

                break;
            case "East":
                Camera.main.gameObject.transform.parent.position = gameObject.transform.position + zoneManager.GetComponent<ZoneManager>().anchorCornerPosition;
                player.transform.position = gameObject.transform.GetChild(0).GetChild(3).GetChild(0).transform.position;
                saveManager.SetPlayerLocation(cellID, "East", currentZoneID);
                entrance = gameObject.transform.GetChild(0).GetChild(3).gameObject;

                if (hasEnemy)
                {
                    gameObject.transform.GetChild(0).GetChild(5).GetComponent<EnemyManagerScript>().CloseExits(entrance);
                }

                break;
        }

    }

    /* SetEnemyState() function is used to update the Cell's enemy state if Cell has any, this is called from the enemy's own code when player
     * defeats them. */
    public void SetEnemyState(bool state)
    {
        hasEnemy = state;
    }

}

