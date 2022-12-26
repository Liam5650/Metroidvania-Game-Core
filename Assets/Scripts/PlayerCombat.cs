using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] float shotVelocity;
    [SerializeField] float shotCooldown;
    [SerializeField] float shotLife;
    [SerializeField] GameObject shot;
    [SerializeField] Transform shotPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject shotReference = Instantiate(shot, shotPoint.position, Quaternion.identity);
            shotReference.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2 (shotVelocity*transform.localScale.x, 0f);
            Destroy(shotReference, shotLife);
        }
    }
}
