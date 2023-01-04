using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    private PlayerMovement player;

    // Start is called before the first frame update
    void start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
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
