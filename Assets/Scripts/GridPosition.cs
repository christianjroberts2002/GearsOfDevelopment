using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    TilePos tilePos;

    public struct TilePos
    {
        public float X;

        public float Y;

        public float Z;

        public bool IsOccupied;

        public int BlockID;

        public TilePos(float x, float y, float z,
            bool isOccupied = false , int blockID = 0)

        {

            X = x;

            Y = y;

            Z = z;

            IsOccupied = isOccupied;

            BlockID = blockID;

        }

    }


    private void Awake()
    {
        tilePos = new TilePos(this.gameObject.transform.position.z / 1.5f, this.gameObject.transform.position.y / 1.5f, this.gameObject.transform.position.x / 1.5f);
    }

    public TilePos GetTilePos()
    {
        return tilePos;
    }


}
