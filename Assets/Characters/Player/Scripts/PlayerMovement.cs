using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;

    public GameObject groundBoxCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        // Handle horizontal movement

        float horizontalVector = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2 (horizontalVector * movementSpeed, rb.velocity.y);

        if (Mathf.Abs(horizontalVector) > 0.01)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (horizontalVector < 0f)
        {
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (horizontalVector > 0f)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Handle vertical movement

        isGrounded = Physics2D.OverlapBox(groundBoxCheck.transform.position, groundBoxCheck.GetComponent<SpriteRenderer>().bounds.size, 0f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("isGrounded", false);
        }

        else if (isGrounded && rb.velocity.y < 0.1f)
        {
            anim.SetBool("isGrounded", true);
        }

    }
}
