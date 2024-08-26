using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    private void Awake()
    { 
        // Set up the main camera to follow the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        }

        // Get the bounds of the camera movement
        GetBounds();
    }

    public void GetBounds()
    {
        // Get the camera bounds and apply to limit movement of the camera
        GameObject cameraBounds = GameObject.FindGameObjectWithTag("CameraBounds");
        if (cameraBounds != null)
        {
            GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraBounds.GetComponent<PolygonCollider2D>(); ;
        }
    }
}
