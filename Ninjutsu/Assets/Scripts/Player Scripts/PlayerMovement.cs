using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float startingSpeed;//The speed of the player at the start.
    [SerializeField] private float jumpForce;
    [SerializeField] private float deathJumpForce;
    [SerializeField] private float checkRadius;
    [SerializeField] private float fallTimer;
    [SerializeField] private LayerMask ground;//The layers that the player can walk on.
    [SerializeField] private Transform feetPos;
    [SerializeField] private Transform headPos;
    [SerializeField] private int deathDelay;


    private Rigidbody2D rb;//The rigidbody component of the player.
    private CapsuleCollider2D playerCollider;
    private float moveInput;//The input that the player gave to walk.
    private float nextFallTimer;
    private bool isGrounded;
    private bool headCheck;
    private bool isJumping;
    private bool isAlive;
    private bool deathJump;
    private float speed;//The speed of the player.


    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponentInParent<Rigidbody2D>();
        playerCollider = gameObject.GetComponentInParent<CapsuleCollider2D>();
        isAlive = true;
    }

     void Start()
    {
        speed = startingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            speed = 0;
            if (deathJump)
            {
                rb.velocity = Vector2.up * deathJumpForce;
                deathJump = false;
            }
            StartCoroutine(DelayDeath(deathDelay));
            return;
        }
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, ground);
        headCheck = Physics2D.OverlapCircle(headPos.position, checkRadius, ground);
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            nextFallTimer = fallTimer;
            rb.velocity = Vector2.up * jumpForce;
        }
        // If the player is jumping and still holds the jump button then jump longer.    
        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (nextFallTimer > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                nextFallTimer -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        // If the player stops pressing the jump button then stop jumping.
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
        //If there is colision with the players head then stop jumping.
        if (headCheck == true)
        {
            isJumping = false;
        }

    }


    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag.Equals("Kill Player"))
        {
            isAlive = false;
            deathJump = true;
            playerCollider.enabled = false;
        }
    }
    IEnumerator DelayDeath(int i)
    {
        yield return new WaitForSeconds(i);
    }
}
