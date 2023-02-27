using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, PlayerCombat.instance.gameObject.transform.position, moveSpeed * Time.deltaTime);
    }
}
