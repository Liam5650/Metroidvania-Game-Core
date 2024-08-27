using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

public class MapController : MonoBehaviour
{
    [SerializeField] private Tilemap roomGrid;                      // The tilemap grid of the rooms that have been visited
    [SerializeField] private Tile visitedRoomTile;                  // The tile to use to indicate a room has been visited in the grid
    [SerializeField] private Vector2 roomSize;                      // The dimensions of one room, ie a 1x1 scale of the camera bounds
    [SerializeField] private Transform cameraTransform;             // Keeps track of the position of the camera to tell what part of the grid we are on
    private Vector3Int currLocationInGrid;                          // The current location in the grid in single units, ex. x=5, y=8
    private List<Vector3Int> visitedRooms = new List<Vector3Int>(); // A list of the vector3 ints of the visited rooms for saving
    [SerializeField] private Transform miniMapCamera;               // The minimap camera
    [SerializeField] private Transform fullMapCamera;               // The fullscreen map camera
    [SerializeField] private Transform roomIndicator;               // The flashing indicator on the map / minimap to show the room we are currently in
    private bool viewingMap;                                        // Used to track if the map screen is open or not
    [SerializeField] private float mapPanSpeed;                     // Panning speed when navigating the map screen
    [SerializeField] float maxXOffset, maxYOffset;                  // Max distance we can pan in the map screen
    public static MapController instance;                           // Used so the save controller can get the list of visited rooms

    private void Awake()
    {
        // Set up instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        viewingMap = false;
    }

    void Update()
    {
        // Handle minimap state 
        if (!viewingMap)
        {
            // Get our current coordinate in the room grid
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

            // Set the tile as visited in the tilemap
            if (roomGrid.GetTile(currLocationInGrid) == null)
            {
                roomGrid.SetTile(currLocationInGrid, visitedRoomTile);
            }
        }
        // Handle map screen state
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
        // Gets all of the active tiles in the visited room grid and returns a list of the coordinates
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

    public void RefreshMap(Vector3Int[] rooms)
    {
        // Refresh the state of the map to reflect the save data
        roomGrid.ClearAllTiles();
        if (rooms != null)
        {
            foreach (Vector3Int coord in rooms)
            {
                roomGrid.SetTile(coord, visitedRoomTile);
            }
        }
    }

    public void ViewingMap(bool value)
    {
        // Change state and set up indicator
        viewingMap = value;
        roomIndicator.gameObject.GetComponent<RoomIndicator>().IgnorePause(value);
    }
}