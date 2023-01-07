using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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

    private bool canMove;
    [SerializeField] float hitForce;
    [SerializeField] float hitstun;

    private bool canDoubleJump;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        canMove = true;
    }

    void Update()
    {
        if (canMove && Time.timeScale > 0f)
        {
            // Handle horizontal movement
            float horizontalVector = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontalVector * movementSpeed, rb.velocity.y);
            if (Mathf.Abs(horizontalVector) > 0.01)
            {
                anim.SetBool("isRunning", true);
                gameObject.transform.localScale = new Vector3(1f * horizontalVector, 1f, 1f);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }

            // Handle vertical movement
            isGrounded = Physics2D.OverlapBox(groundBoxCheck.transform.position, groundBoxCheck.GetComponent<SpriteRenderer>().bounds.size, 0f, groundLayer);
            if (isGrounded && rb.velocity.y < 0.01f)
            {
                anim.SetBool("isGrounded", true);
                canDoubleJump = true;
                if (Input.GetButtonDown("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
            else
            {
                anim.SetBool("isGrounded", false);
                if (Input.GetButtonDown("Jump") && canDoubleJump)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    canDoubleJump = false;
                }
            }            
        }
    }

    // Use a public getter attached to this script so the coroutine is run on the player object and persists through loads
    public void launchPlayer()
    {
        StartCoroutine(launch());
    }

    private IEnumerator launch()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(transform.localScale.x*-1, 1)* hitForce, ForceMode2D.Impulse); 
        yield return new WaitForSeconds(hitstun);
        rb.velocity = new Vector2(0f,0f);
        canMove = true;
    }

    // Reset some values when we load the menu/respawn
    private void OnDisable()
    {
        canMove = true;
    }
}