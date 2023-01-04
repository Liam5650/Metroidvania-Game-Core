using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    private void Awake()
    { 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        }
    }
}
