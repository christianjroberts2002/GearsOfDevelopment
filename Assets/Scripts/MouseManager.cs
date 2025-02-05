using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class MouseManager : MonoBehaviour
{
    public Material buildMat;
    public Material preVizMat;

    public LayerMask layerMask;


    private Camera Camera;

    [SerializeField] List<GameObject> currentWall;
    [SerializeField] List<GameObject> currentRoof;
    [SerializeField] List<GameObject> currentCorners;

    [SerializeField] GameObject preVizGO;
    [SerializeField] GameObject finalGO;

    [SerializeField] GameObject testGO;
    [SerializeField] GameObject testCornerGO;
    [SerializeField] GameObject roofGO;
    [SerializeField] GameObject roofCornerGO;
    [SerializeField] GridSystem gridSystem;

    private GridPosition.TilePos[,] roofTiles;
    private GridPosition.TilePos[,,] roofTiles3D;

    [Range(0f, 1f)]
    public float targetAlpha = 0.5f;
    /*    [SerializeField] GridPosition.TilePos startTile;
        [SerializeField] GridPosition.TilePos endTile;*/

    GridPosition.TilePos startTile;
    GridPosition.TilePos endTile;

    GridPosition.TilePos oStartTile;
    GridPosition.TilePos oEndTile;

    public HouseBuilding houseBuildingState;

    private bool preVizWallDown = false;
    private bool preVizRoofDown = false;
    private bool startIsZero = false;

    private int roofLevel;



    private void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        houseBuildingState = HouseBuilding.None;

        
    }
    void Update()
    {
        switch(houseBuildingState)
        {
            case HouseBuilding.MouseDown:
                ClickSetStartTile();
                testGO = preVizGO;
                break;
            case HouseBuilding.MouseDragged:
                PreVizBuilding();
                SetMaterialsTransparent(currentWall);
                SetMaterialsTransparent(currentRoof);
                break;
            case HouseBuilding.MouseUp:
                testGO = finalGO;
                SetMaterialsOpaque(currentWall);
                SetMaterialsOpaque(currentRoof);

                currentWall = new List<GameObject>();
                currentRoof = new List<GameObject>();
                break;
            case HouseBuilding.None:
                break;
        }

        if (Input.GetMouseButtonDown(0))
        {
            houseBuildingState = HouseBuilding.MouseDown;
            
        }
        if (Input.GetMouseButtonUp(0) && houseBuildingState == HouseBuilding.MouseDragged)
        {
            houseBuildingState = HouseBuilding.MouseUp;
            preVizWallDown = false;
            preVizRoofDown= false;
            
           // DestroyPreVizBlocks(preVizWall);
            //DestroyPreVizBlocks(preVizRoof);
        }

        if (houseBuildingState == HouseBuilding.MouseDown)
        {

        }



    }

    private void ClickSetStartTile()
    {
        
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 1000f, layerMask))
        {
            if (raycastHit.transform != null)
            {
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                if(tilePose.IsOccupied == true)
                {
                    return;
                }
                startTile = tilePose;
                Debug.Log(tilePose.X + "," + tilePose.Z);
                

                houseBuildingState = HouseBuilding.MouseDragged;
            }
        }
    }

    private void ClickSetEndTile(GridPosition.TilePos endTile)
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                endTile = tilePose;
                if (endTile.X == startTile.X || endTile.Z == startTile.Z)
                {
                    return;
                }

                //Testing
                GetSelectedTiles(startTile, endTile);
                CreateWalls(GetSelectedTiles(startTile, endTile));

            }
        }


    }

    private void PreVizBuilding()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 1000f, layerMask))
        {
            float lastEndX = -1;
            float lastEndZ = -1;
            float lastEndY = -1;

            if (raycastHit.transform != null)
            {
                //Our custom method. 
                //Debug.Log(raycastHit.transform.gameObject.name);
                GridPosition hitGridPosition = raycastHit.transform.GetComponent<GridPosition>();
                GridPosition.TilePos tilePose = hitGridPosition.GetTilePos();
                if (tilePose.IsOccupied == true)
                {
                    return;
                }

                endTile = tilePose;

                oStartTile = startTile;
                oEndTile = endTile;

                

                if (lastEndX != endTile.X && lastEndY != endTile.Y && lastEndZ != endTile.Z )
                {
                    DestroyPreVizBlocks(currentWall);
                    DestroyPreVizBlocks(currentRoof);
                    preVizWallDown = false;
                    preVizRoofDown = false;
                }

                
                lastEndX = endTile.X;
                lastEndY = endTile.Y;
                lastEndZ = endTile.Z;

                if ((startTile.X == 0 && startTile.Z == 0 && startTile.Y == 0)|| (endTile.X == 0 && endTile.Z == 0 && endTile.Y == 0))
                {
                    startIsZero = true;
                }


                //Debug.Log(tilePose.X + "," + tilePose.Y);
                if (endTile.X == startTile.X && endTile.Z == startTile.Z && endTile.Y == startTile.Y)
                {
                    return;
                }

                if (Mathf.Abs(endTile.X - startTile.X) <= 1 || Mathf.Abs(endTile.Z - startTile.Z) <=1 || Mathf.Abs(endTile.Y - startTile.Y) <= 1)
                {
                    return;
                }
                //Testing

                //NormalizeTileBounds();

                CreateWalls(GetSelectedTiles(startTile, endTile));
                float roofHeight = 0;
                

                roofHeight = (Mathf.Min(Mathf.Abs(endTile.X - startTile.X), Mathf.Abs(endTile.Z - startTile.Z)));
                roofHeight = Mathf.CeilToInt(roofHeight / 2);

                int startx = Convert.ToInt32(Mathf.Min(startTile.X, endTile.X));
                int endx = Convert.ToInt32(Mathf.Max(startTile.X, endTile.X));
                int startz = Convert.ToInt32(Mathf.Min(startTile.Z, endTile.Z));
                int endy = Convert.ToInt32(Mathf.Max(startTile.Z, endTile.Z));

                currentRoof = new List<GameObject>();
                currentCorners = new List<GameObject>();
                roofLevel = 0;
                for (int i = 0; i <= roofHeight; i++)
                {
                    GridPosition.TilePos roofstart = new GridPosition.TilePos(startx + i, 0, startz + i);
                    GridPosition.TilePos roofend = new GridPosition.TilePos(endx - i, 0, endy - i);
                    preVizRoofDown = false;
                    roofLevel++;
                    
                    PreVizRoof(GetSelectedTiles(roofstart, roofend));
                }
                preVizRoofDown = true;

                //Vector3 newGOSpawn = new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 1.5f, raycastHit.transform.position.z);
                //Instantiate(testGO, newGOSpawn, Quaternion.identity);
            }
        }

        //Find all tiles in the array
        //Find all edge tiles
        // place GO in the edge tiles
    }

    private void NormalizeTileBounds()
    {
        int startX = (int)Mathf.Min(startTile.X, endTile.X);
        int endX = (int)Mathf.Max(startTile.X, endTile.X);
        int startY = (int)Mathf.Min(startTile.Z, endTile.Z);
        int endY = (int)Mathf.Max(startTile.Z, endTile.Z);

        startTile = new GridPosition.TilePos(startX, 0, startY);
        endTile = new GridPosition.TilePos(endX, 0, endY);
    }

    private void PlaceBuilding()
    {
                    if(endTile.X == startTile.X ||  endTile.Z == startTile.Z)
                    {
                        return;
                    }

                    //Testing
                    GetSelectedTiles(startTile, endTile);
                    CreateWalls(GetSelectedTiles(startTile, endTile));

                    houseBuildingState = HouseBuilding.None;
                    

        //Find all tiles in the array
        //Find all edge tiles
        // place GO in the edge tiles
    }

    private GridPosition.TilePos[,,] GetSelectedTiles(GridPosition.TilePos startTile, GridPosition.TilePos endTile)
    {
        int startx = Convert.ToInt32(Mathf.Min(startTile.X, endTile.X));
        int endx = Convert.ToInt32(Mathf.Max(startTile.X, endTile.X));
        int starty = Convert.ToInt32(Mathf.Max(startTile.Y, endTile.Y));
        int startz = Convert.ToInt32(Mathf.Min(startTile.Z, endTile.Z));
        int endz = Convert.ToInt32(Mathf.Max(startTile.Z, endTile.Z));
        int endy = Convert.ToInt32(Mathf.Max(startTile.Y, endTile.Y));



        //Debug.Log(startx +"," + starty);

        int arrayLengthX = endx - startx + 1;
        int arrayLengthY = endy - starty + 1;
        int arrayLengthZ = endz - startz + 1;

        int xpos = startx;
        int ypos = starty;
        int zpos = startz;
;

        // First, we need to clear out the array before filling it
        GridPosition.TilePos[,,] selectedTiles = new GridPosition.TilePos[arrayLengthX,arrayLengthY, arrayLengthZ];

        

        

        for (int y = 0; y < arrayLengthY; y++) // Avoid duplicate corners
        {
            // Top and Bottom Edges (Y stays constant)
            for (int i = 0; i < arrayLengthX; i++)
            {
                // Top edge: (i, 0)
                selectedTiles[i, y, 0] = gridSystem.GetTileInArray(startx + i, starty + y, startz);  // starty is constant for top
                                                                                            // Bottom edge: (i, arrayLengthZ - 1)
                selectedTiles[i, y, arrayLengthZ - 1] = gridSystem.GetTileInArray(startx + i, starty + y, startz + arrayLengthZ - 1); // starty + arrayLengthZ - 1 for the bottom
            }
            
        }

        for (int y = 0; y < arrayLengthY; y++) // Avoid duplicate corners
        {
            // Left and Right Edges (X stays constant)
            for (int i = 1; i < arrayLengthZ - 1; i++) // Avoid duplicate corners
            {
                // Left edge: (0, i)
                selectedTiles[0, y, i] = gridSystem.GetTileInArray(startx, starty + y, startz + i);  // startx is constant for left
                                                                                            // Right edge: (arrayLengthX - 1, i)
                selectedTiles[arrayLengthX - 1, y, i] = gridSystem.GetTileInArray(startx + arrayLengthX - 1, starty + y, startz + i);  // startx + arrayLengthX - 1 for the right
            }

        }




            return selectedTiles;
    }

    private void CreateWalls(GridPosition.TilePos[,,] selectedTiles)
    {
        if (preVizWallDown)
        {
            return;
        }

        
        

        if(selectedTiles == null)
        {
            return;
        }

        currentWall = new List<GameObject>();
        
        int arrayLengthX = selectedTiles.GetLength(0);
        int arrayLengthY = selectedTiles.GetLength(1);
        int arrayLengthZ = selectedTiles.GetLength(2);

        int roofArrayX = arrayLengthX - 1;
        int roofArrayY = 1;
        int roofArrayZ = arrayLengthZ - 1;


        roofTiles = new GridPosition.TilePos[roofArrayX, roofArrayZ];
        /*if(arrayLengthX !> 2 ||  arrayLengthZ !> 2)
        {
            return;
        }*/



        for (int x = 0; x < arrayLengthX; x++)
        {
            for (int y = 0; y < arrayLengthY; y++)
            {
                for (int z = 0; z < arrayLengthZ; z++)
                {

                    if (selectedTiles[x, y, z].X == 0 && selectedTiles[x, y, z].Z == 0 && !startIsZero)
                    {
                        //Debug.Log("zero is end or start");
                        continue; // Skip invalid tiles

                    }
                    // Place corner blocks with rotation
                    if ((x == 0 && y == 0 && z == 0)) // Top-left corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 180, 0); // No rotation
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f, selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                        currentWall.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == 0 && z == arrayLengthZ - 1)) // Top-right corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f, selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                        currentWall.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == arrayLengthX - 1 && z == 0)) // Bottom-left corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, -90, 0); // Rotate -90 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f, selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                        currentWall.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == arrayLengthX - 1 && z == arrayLengthZ - 1)) // Bottom-right corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f, selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(testCornerGO, newGOCornerSpawn, rotation);
                        currentWall.Add(newCornerBlock);
                        continue;
                    }
                    if (x == arrayLengthX - 1 || x + arrayLengthZ - 1 == arrayLengthZ - 1)
                    {
                        float tileOffsetX = -.5f;
                        if (x == arrayLengthX - 1)
                        {
                            tileOffsetX = .5f;
                        }

                        Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f, (selectedTiles[x, y, z].X * 1.5f) + tileOffsetX);
                        GameObject newblock = Instantiate(testGO, newGOSpawn, rotation);
                        currentWall.Add(newblock);
                        preVizWallDown = true;
                        startIsZero = false;
                        continue;
                    }
                    else if (z == arrayLengthZ - 1 || z + arrayLengthX - 1 == arrayLengthX - 1)
                    {
                        float tileOffsetY = -.5f;
                        if (z == arrayLengthZ - 1)
                        {
                            tileOffsetY = .5f;
                        }
                        Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOSpawn = new Vector3((selectedTiles[x, y, z].Z * 1.5f) + tileOffsetY, 1.5f, selectedTiles[x, y, z].X * 1.5f);
                        GameObject newblock = Instantiate(testGO, newGOSpawn, rotation);
                        currentWall.Add(newblock);
                        preVizWallDown = true;
                        startIsZero = false;
                        continue;
                    }

                    /*for (i = 0  ; i < arrayLengthX - 2; i++)
                    {
                        for(j = 0  ; j < arrayLengthZ - 2; j++)
                        {
                            roofTiles[i,j] = selectedTiles[i,j];
                        }
                    }*/
                }

            }
        }
        //PreVizRoof(roofTiles);

    }

    private void PreVizRoof(GridPosition.TilePos[,,] selectedTiles)
    {
        if (preVizRoofDown)
        {
            return;
        }




        if (selectedTiles == null)
        {
            Debug.Log("Reutrn lisst is null");
            return;
        }

        

        int arrayLengthX = selectedTiles.GetLength(0);
        int arrayLengthY = selectedTiles.GetLength(1);
        int arrayLengthZ = selectedTiles.GetLength(2);

        int roofArrayX = arrayLengthX - 1;
        int roofArrayY = arrayLengthY - 1;
        int roofArrayZ = arrayLengthZ - 1;


        /*if(arrayLengthX !> 2 ||  arrayLengthZ !> 2)
        {
            return;
        }*/

        float roofIntialOffset = 1.5f;
        float roofHeight = 1.5f;


        for (int x = 0; x < arrayLengthX; x++)
        {
            for (int y = 0; y < arrayLengthY; y++)
            {
                for (int z = 0; z < arrayLengthZ; z++)
                {

                    /*if (selectedTiles[i, j].X == 0 && selectedTiles[i, j].Y == 0 && !startIsZero)
                    {
                        //Debug.Log("zero is end or start");
                        continue; // Skip invalid tiles

                    }*/
                    // Place corner blocks with rotation
                    if ((x == 0 && z == 0)) // Top-left corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 180, 0); // No rotation
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                        currentRoof.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == 0 && z == arrayLengthZ - 1)) // Top-right corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                        currentRoof.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == arrayLengthX - 1 && z == 0)) // Bottom-left corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, -90, 0); // Rotate -90 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                        currentRoof.Add(newCornerBlock);
                        continue;
                    }
                    else if ((x == arrayLengthX - 1 && z == arrayLengthZ - 1)) // Bottom-right corner
                    {
                        Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOCornerSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[x, y, z].X * 1.5f);
                        GameObject newCornerBlock = Instantiate(roofCornerGO, newGOCornerSpawn, rotation);
                        currentRoof.Add(newCornerBlock);
                        continue;
                    }
                    if (x == arrayLengthX - 1 || x + arrayLengthZ - 1 == arrayLengthZ - 1)
                    {
                        float tileOffsetX = -.0f;
                        if (x == arrayLengthX - 1)
                        {
                            tileOffsetX = .0f;
                        }

                        Quaternion rotation = Quaternion.Euler(0, 90, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOSpawn = new Vector3(selectedTiles[x, y, z].Z * 1.5f, 1.5f + (roofIntialOffset * roofLevel), (selectedTiles[x, y, z].X * 1.5f) + tileOffsetX);
                        GameObject newblock = Instantiate(roofGO, newGOSpawn, rotation);
                        currentRoof.Add(newblock);
                        //preVizRoofDown = true;
                        startIsZero = false;
                        continue;
                    }
                    else if (z == arrayLengthZ - 1 || z + arrayLengthX - 1 == arrayLengthX - 1)
                    {
                        float tileOffsetY = -.0f;
                        if (z == arrayLengthZ - 1)
                        {
                            tileOffsetY = .0f;
                        }
                        Quaternion rotation = Quaternion.Euler(0, 0, 0); // Rotate 180 degrees around Y-axis
                        Vector3 newGOSpawn = new Vector3((selectedTiles[x, y, z].Z * 1.5f) + tileOffsetY, 1.5f + (roofIntialOffset * roofLevel), selectedTiles[x, y, z].X * 1.5f);
                        GameObject newblock = Instantiate(roofGO, newGOSpawn, rotation);
                        currentRoof.Add(newblock);
                        //preVizRoofDown = true;
                        startIsZero = false;
                        continue;
                    }
                }

                /*for (i = 0  ; i < arrayLengthX - 2; i++)
                {
                    for(j = 0  ; j < arrayLengthZ - 2; j++)
                    {
                        roofTiles[i,j] = selectedTiles[i,j];
                    }
                }*/


            }
        }
        //PreVizRoof(roofTiles);
    }

    private void DestroyPreVizBlocks(List<GameObject> goList)
    {
        foreach (GameObject go in goList)
        {
            Destroy(go);
        }
            
    }

    private void SetMaterialsTransparent(List<GameObject> goList)
    {
        foreach (GameObject go in goList)
        {
            Renderer rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                Material material = rend.material;
                // Optionally, set shader properties for transparency if needed
                // material.SetFloat("_SurfaceType", 1);
                // material.SetFloat("_BlendMode", 0);

                // Modify the alpha value:
                Color color = material.color;
                color.a = 0.5f;  // Set to 50% opacity
                material.color = color;  // Reassign the color to the material


            }

            if (go.transform.childCount > 0)
            {
                foreach (Transform go2 in go.transform)
                {
                    Renderer rendChild = go2.GetComponent<Renderer>();
                    if (rendChild != null)
                    {
                        Material childMaterial = rendChild.material;

                        // Modify the alpha value:
                        Color childColor = childMaterial.color;
                        childColor.a = .5f;  // Set to 50% opacity
                        childMaterial.color = childColor;  // Reassign the color to the material
                    }
                }
            }
        }
    }



    private void SetMaterialsOpaque(List<GameObject> goList)
    {
        foreach (GameObject go in goList)
        {
            Renderer rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                Material material = rend.material;
                // Optionally, set shader properties for transparency if needed
                // material.SetFloat("_SurfaceType", 1);
                // material.SetFloat("_BlendMode", 0);

                // Modify the alpha value:
                Color color = material.color;
                color.a = 1;  // Set to 50% opacity
                material.color = color;  // Reassign the color to the material
            }

            if(go.transform.childCount > 0)
            {
                foreach (Transform go2 in go.transform)
                {
                    Renderer rendChild = go2.GetComponent<Renderer>();
                    if (rendChild != null)
                    {
                        Material childMaterial = rendChild.material;

                        // Modify the alpha value:
                        Color childColor = childMaterial.color;
                        childColor.a = 1f;  // Set to 50% opacity
                        childMaterial.color = childColor;  // Reassign the color to the material
                    }
                }
            }
        }
    }
}



public enum HouseBuilding
{
    None,
    MouseDown,
    MouseDragged,
    MouseUp,
}
