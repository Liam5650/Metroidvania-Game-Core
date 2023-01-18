using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    [SerializeField] private Tilemap roomGrid;
    [SerializeField] private Tile visitedRoomTile;
    [SerializeField] private Vector2 roomSize;
    [SerializeField] private Transform cameraTransform;
    private Vector3Int currLocationInGrid;
    private List<Vector3Int> visitedRooms = new List<Vector3Int>();
    [SerializeField] private Transform miniMapCamera;
    [SerializeField] private Transform fullMapCamera;
    [SerializeField] private Transform roomIndicator;

    void Start()
    {
 
    }

    void Update()
    {
        float xLoc = cameraTransform.position.x / roomSize.x;
        if (xLoc<0)
        {
            xLoc -= 1;
        }

        float yLoc = cameraTransform.position.y / roomSize.y;
        if (yLoc<0)
        {
            yLoc -= 1;
        }

        currLocationInGrid = new Vector3Int((int)xLoc, (int)yLoc, 0);

        // Move the cameras
        miniMapCamera.position = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, miniMapCamera.position.z);
        fullMapCamera.position = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, fullMapCamera.position.z);
        roomIndicator.position = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, roomIndicator.position.z);

        // Set the tile
        if (roomGrid.GetTile(currLocationInGrid) == null)
        {
            roomGrid.SetTile(currLocationInGrid, visitedRoomTile);
        }
    }


    public List<Vector3Int> SaveMap()
    {
        roomGrid.CompressBounds();
        var bounds = roomGrid.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int roomCoord = new Vector3Int(x, y, 0);
                if (roomGrid.GetTile(roomCoord) != null)
                {
                    visitedRooms.Add(roomCoord);
                }
            }
        }
        return visitedRooms;
    }

    public void LoadMap(List<Vector3Int> visitedRooms)
    {
        foreach (Vector3Int coord in visitedRooms)
        {
            roomGrid.SetTile(coord, visitedRoomTile);
        }
    }
}
