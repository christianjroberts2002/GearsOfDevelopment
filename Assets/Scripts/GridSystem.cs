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
    public GridPosition.TilePos[,,] gameGridArray;
    public GridPosition.TilePos[,,] usedGameGridArray;


    private void Start()
    {
        gameGridArray = new GridPosition.TilePos[gridXSize, gridYSize , gridZSize];
        usedGameGridArray = new GridPosition.TilePos[gridXSize, gridYSize, gridZSize];
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
        float yPos = 0;
        float zPos = 0;

        for (int x = 0; x < Mathf.FloorToInt(gridXSize); x++)
        {
            for (int y = 0; y < Mathf.FloorToInt(gridYSize); y++)
            {
                for (int z = 0; z < Mathf.FloorToInt(gridZSize); z++)
                {
                    // Store the grid cell data.
                    StoreGridArray(z, y, x);

                    // Calculate the position of the tile.
                    Vector3 newTilePos = new Vector3(xPos, yPos, zPos);

                    // Optionally instantiate a visual tile.
                    GameObject newTileGO = Instantiate(gridTileVisual, newTilePos, Quaternion.identity);
                    TextMeshPro newTileText = newTileGO.GetComponentInChildren<TextMeshPro>();
                    newTileText.text = x.ToString() + "," + y.ToString() + "," + z.ToString();

                    // Move along the z-axis.
                    zPos += tileSize;
                }

                // Reset z position and move along the y-axis.
                zPos = 0;
                yPos += tileSize;
            }

            // Reset y position and move along the x-axis.
            yPos = 0;
            xPos += tileSize;
        }
    }



    public float GetTileSize()
    {
        return tileSize;
    }

    private void StoreGridArray(int xGridPos, int yGridPos, int zGridPos)
    {
        gameGridArray[xGridPos, yGridPos, zGridPos] =  new GridPosition.TilePos(xGridPos, yGridPos,  zGridPos);
    }

    public GridPosition.TilePos GetTileInArray(int xGridPos, int yGridPos, int zGridPos)
    {
        return gameGridArray[xGridPos, yGridPos ,zGridPos];
    }

    public void StoreUsedTiles(int xGridPos, int yGridPos, int zGridPos, GridPosition.TilePos tilePos)
    {
        usedGameGridArray[xGridPos,yGridPos,zGridPos] = tilePos;
        tilePos.IsOccupied = true;
    }

    


}
