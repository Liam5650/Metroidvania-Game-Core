using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    [SerializeField] string roomToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            // Disable the trigger so it cant be hit multiple times, and start the scene switch coroutine
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            FindObjectOfType<UIController>().LoadRoom(roomToLoad);
        }
    }
}
