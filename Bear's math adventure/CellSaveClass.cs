using UnityEngine;


/* This is the data class I used to save information for each Cell, most are simple information such as ID, the ID for the tile it contains 
 * and which type of path goes through the Cell. The treasure boolean is not used currently as it was designed to be potential collectibles
 * or spots where player can heal themselves. isExit is for the Cell which goes to the next Zone, Last is the Exit Cell of the last Zone and
 * Start is the Cell of the first Zone from which the player starts.*/
[System.Serializable]
public class CellSaveClass
{
    public string cellID;
    public string tileID;
    public string pathType;
    public bool hasTreasure;
    public bool hasEnemy;
    public bool isExit;
    public bool isLast;
    public bool isStart;
}
