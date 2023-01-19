using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

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

    private bool viewingMap;
    [SerializeField] private float mapPanSpeed;
    [SerializeField] float maxXOffset, maxYOffset;

    void Start()
    {
        viewingMap= false;
    }

    void Update()
    {
        if (!viewingMap)
        {
            float xLoc = cameraTransform.position.x / roomSize.x;
            if (xLoc < 0)
            {
                xLoc -= 1;
            }

            float yLoc = cameraTransform.position.y / roomSize.y;
            if (yLoc < 0)
            {
                yLoc -= 1;
            }

            currLocationInGrid = new Vector3Int((int)xLoc, (int)yLoc, 0);

            // Move the cameras
            miniMapCamera.localPosition = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, miniMapCamera.localPosition.z);
            fullMapCamera.localPosition = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, fullMapCamera.localPosition.z);
            roomIndicator.localPosition = new Vector3((int)xLoc + 0.5f, (int)yLoc + 0.5f, roomIndicator.localPosition.z);

            // Set the tile
            if (roomGrid.GetTile(currLocationInGrid) == null)
            {
                roomGrid.SetTile(currLocationInGrid, visitedRoomTile);
            }
        }
        else
        {
            float xNewPos = fullMapCamera.localPosition.x + (mapPanSpeed * Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime);
            if (Mathf.Abs(xNewPos) > maxXOffset) xNewPos = maxXOffset*Mathf.Sign(xNewPos);
            float yNewPos = fullMapCamera.localPosition.y + (mapPanSpeed * Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime);
            if (Mathf.Abs(yNewPos) > maxYOffset) yNewPos = maxYOffset * Mathf.Sign(yNewPos);
            fullMapCamera.localPosition = new Vector3(xNewPos, yNewPos, fullMapCamera.localPosition.z);
        }

    }


    public Vector3Int[] SaveMap()
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
        return visitedRooms.ToArray();
    }

    public void LoadMap(Vector3Int[] rooms)
    {
        // If there is save data, set the tiles to visited
        if (rooms != null)
        {
            foreach (Vector3Int coord in rooms)
            {
                roomGrid.SetTile(coord, visitedRoomTile);
            }
        }
        // If there is no save data, make sure the tiles have been reset
        else
        {
            roomGrid.ClearAllTiles();
        }
    }

    public void ViewingMap(bool value)
    {
        viewingMap = value;
        roomIndicator.gameObject.GetComponent<RoomIndicator>().IgnorePause(value);
    }
}