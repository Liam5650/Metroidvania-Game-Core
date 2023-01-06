using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    private PlayerMovement player;

    void start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        // Track and move towards player if they have been found
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }
        else
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.gameObject.transform.position, step);
        }
    }
}
