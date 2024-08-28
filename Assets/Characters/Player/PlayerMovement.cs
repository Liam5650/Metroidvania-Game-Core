using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;           // Player movement speed
    [SerializeField] float jumpForce;               // Force of which the player jumps with
    public GameObject groundBoxCheck;               // Used to check if the player is grounded
    public LayerMask groundLayer;                   // Used to differentiate the ground layer which is standable
    private Rigidbody2D rb;                         // Used to apply velocity changes
    private Animator anim;                          // Used to change player animation from movement
    private bool isGrounded;                        // Reference if we are currently touching the ground
    private bool canMove;                           // The player movement can be disabled when they have taken damage knockback
    [SerializeField] float hitForce;                // The amount of knockback the player experiences when taking damage
    [SerializeField] float hitstun;                 // The amount of hitstun from taking damage
    private bool canDoubleJump;                     // Reference if the player has the double jump ability
    [SerializeField] RuntimeAnimatorController ballAnimController;          // The ball-state animation controller
    [SerializeField] BoxCollider2D ballCollider, standingCollider;          // The player standing and ball colliders
    private RuntimeAnimatorController standingAnimController;               // The standing-state anim controller
    [SerializeField] GameObject ballGroundBoxCheck, ballCeilingBoxCheck;    // Checks for jumping in ball mode and exiting ball mode
    private Unlocks unlocked;                       // The current player unlocked abilities
    private bool impactPlayed;                      // Reference if we have played a ground impact noise from landing
    [SerializeField] private float bombImpulseStrength;                     // Amount of force a bomb applies to the player in ball mode

    void Start()
    {
        // Initialize values
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        standingAnimController = anim.runtimeAnimatorController;
        canMove = true;
        unlocked = GetComponent<Unlocks>();
    }

    void Update()
    {
        if (canMove && Time.timeScale > 0f)
        {
            // Control transition between standing/ball state
            if (unlocked.Ball())
            {
                float verticalVector = Input.GetAxisRaw("Vertical");
                if (verticalVector < 0 && standingCollider.enabled == true)
                {
                    anim.runtimeAnimatorController = ballAnimController;
                    standingCollider.enabled = false;
                    ballCollider.enabled = true;
                    AudioManager.instance.PlaySFX("PlayerMovement", 4);

                }
                else if (verticalVector > 0 && standingCollider.enabled == false && !(Physics2D.OverlapBox(ballCeilingBoxCheck.transform.position, ballCeilingBoxCheck.GetComponent<SpriteRenderer>().bounds.size, 0f, groundLayer)))
                {
                    anim.runtimeAnimatorController = standingAnimController;
                    ballCollider.enabled = false;
                    standingCollider.enabled = true;
                    AudioManager.instance.PlaySFX("PlayerMovement", 5);
                }
            }

            // Limit vertical falling speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -30f, 1000f));

            // Handle standing state movement
            if (standingCollider.enabled == true)
            {
                // Handle horizontal component
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
                    if (!impactPlayed)
                    {
                        AudioManager.instance.PlaySFX("PlayerMovement", 3); 
                        impactPlayed = true;
                    }
                    anim.SetBool("isGrounded", true);
                    canDoubleJump = true;
                    if (Input.GetButtonDown("Jump"))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        AudioManager.instance.PlaySFX("PlayerMovement", 1);
                    }
                }
                else
                {
                    impactPlayed = false;
                    anim.SetBool("isGrounded", false);
                    if (Input.GetButtonDown("Jump") && canDoubleJump && unlocked.DoubleJump())
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        AudioManager.instance.PlaySFX("PlayerMovement", 2);
                        canDoubleJump = false;
                    }
                }
            }
            // Handle ball state movement
            else
            {
                // Handle horizontal component
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
                isGrounded = Physics2D.OverlapBox(ballGroundBoxCheck.transform.position, ballGroundBoxCheck.GetComponent<SpriteRenderer>().bounds.size, 0f, groundLayer);
                if (isGrounded && !impactPlayed)
                {
                    AudioManager.instance.PlaySFX("PlayerMovement", 3);
                    impactPlayed = true;
                }
                else if (!isGrounded)
                {
                    impactPlayed= false;
                }
                if (isGrounded && rb.velocity.y < 0.01f && Input.GetButtonDown("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    AudioManager.instance.PlaySFX("PlayerMovement", 1);
                }
            }
        }
    }

    
    public void launchPlayer(Vector3 hitPosition)
    {
        // Start the launch sequence from taking damage
        StartCoroutine(Launch(hitPosition));
    }

    private IEnumerator Launch(Vector3 hitPoition)
    {
        // Launch the player
        canMove = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - hitPoition.x), 1)* hitForce, ForceMode2D.Impulse); 
        yield return new WaitForSeconds(hitstun);
        rb.velocity = new Vector2(0f,0f);
        canMove = true;
    }

    private void OnDisable()
    {
        // Reset some values when we load the menu/respawn
        canMove = true;
        anim.runtimeAnimatorController = standingAnimController;
        ballCollider.enabled = false;
        standingCollider.enabled = true;
        gameObject.transform.localScale = Vector3.one;
    }

    public bool IsStanding()
    {
        // Return player state for combat controller 
        return standingCollider.enabled;
    }

    private void PlayFootstepAudio()
    {
        // Played by syncing with animation
        AudioManager.instance.PlayAdjustedSFX("PlayerMovement", 0, 0.1f);
    }

    public void BombImpulse()
    {
        // Launch the player if in ball mode
        if (standingCollider.enabled == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0, 1) * bombImpulseStrength, ForceMode2D.Impulse);
        }
    }
}