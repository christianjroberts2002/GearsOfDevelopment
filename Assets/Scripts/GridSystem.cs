using System;
using TMPro;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float tileSize = 1.5f;
    [SerializeField] private int gridXSize;
    [SerializeField] private int gridYSize;
    [SerializeField] private int gridZSize;
    [SerializeField] GameObject gridTileVisual;
    public GridPosition.TilePos[,] gameGridArray;


    private void Start()
    {
        gameGridArray = new GridPosition.TilePos[gridXSize + 1, gridZSize + 1];
        CreateGrid(gridXSize, gridYSize, gridZSize);

        /*for (int i = 0; i <= gridXSize; i++)
        {
            for (int j = 0; j <= gridZSize; j++)
            {
                Debug.Log(gameGridArray[i, j].X + "." + gameGridArray[i, j].Y);

            }
        }*/
    }

    void CreateGrid(float gridXSize, float gridYSize, float gridZSize)
    {
        float xPos = 0;
        //float yPos = 0;
        float zPos = 0;
        
        for (int i = 0; i <= gridXSize; i++)
        {
            

            for (int j = 0; j <= gridZSize; j++)
            {  
                Vector3 newTilePos = new Vector3(xPos, 0, zPos);
                GameObject newTileGO = Instantiate(gridTileVisual, newTilePos, Quaternion.identity);
                TextMeshPro newTileText = newTileGO.GetComponentInChildren<TextMeshPro>();
                newTileText.text = i.ToString() + "," + j.ToString();
                xPos = xPos + tileSize;

                StoreGridArray(i, j);

            }
            zPos = zPos + tileSize;
            xPos = 0;

        }
    }


    public float GetTileSize()
    {
        return tileSize;
    }

    private void StoreGridArray(int xGridPos, int zGridPos)
    {
        gameGridArray[xGridPos,zGridPos] =  new GridPosition.TilePos(xGridPos, zGridPos);
    }

    public GridPosition.TilePos GetTileInArray(int xGridPos, int zGridPos)
    {
        return gameGridArray[xGridPos,zGridPos];
    }

    


}
