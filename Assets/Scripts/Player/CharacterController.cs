using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Tilemaps;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Settings
    [Header("Move Settings")]
    public float moveSpeed = 1f;
    public float accelerationRate = 0.1f;
    public float deccelerationRate = 0.1f;
    public float velPower = 1f;
    public float friction = 2f;

    [Header("Jump Settings")]
    public float jumpForce = 1f;
    public float coyoteTime = 0.5f;
    public float jumpBufferTime = 0.5f;
    public float jumpCutMultiplier = 2f;
    public float fallGravityMultiplier = 2f;
    private float gravityScale;

    //public float walkSpeed = 40f;
    //public float jumpHeight = 40f;
    //[Range(0, .3f)][SerializeField] private float MovementSmoothing = .05f;

    //Input
    private float hMove = 0f;
    private bool jump = false;
    private bool shoot = false;

    //State
    private bool grounded = true;
    private Transform CurRespwawnAnchor;
    private bool canMove = true;
    private bool canUseBlot = true;

    //Blot Charges
    [SerializeField] private int StartingBlotCharges = 2;
    private int CurBlotCharges;

    //Facing
    private bool facingRight = true;

    #region Components
    //Rigidbody
    private Rigidbody2D cRigidBody;

    //Animator
    private Animator animator;
    #endregion

    #region Jump
    //Jump State
    [SerializeField]
    private LayerMask WhatIsGround;
    private float lastGroundTime = 0f;
    private float lastJumpTime = 0f;
    private Vector2 groundCheckSize = new Vector2(0.2f, 0.2f);
    private bool isJumping = false;
    private bool jumpInputReleased = false;
    #endregion

    #region Children

    [SerializeField]
    private GameObject CielingCheck;
    [SerializeField]
    private GameObject GroundCheck;

    #endregion

    //Blot
    private GameObject blot = null;

    // Start is called before the first frame update
    void Start()
    {
        cRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        CurBlotCharges = StartingBlotCharges;
        gravityScale = cRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Get Input
        hMove = Input.GetAxisRaw("Horizontal");
        //hMove = Input.GetAxisRaw("Horizontal") * walkSpeed;

        if (Input.GetButtonDown("Jump")) 
        {
            OnJump();
        }

        if (Input.GetButtonUp("Jump")) {
            OnJumpUp();
        }

        if (Input.GetButtonDown("Fire1") && blot == null && canUseBlot) 
        { 
            shoot = true;
        }
        
    }

    private void FixedUpdate()
    {
        //Increase Timers
        lastGroundTime -= Time.deltaTime;
        
        //Check if jumping
        if (cRigidBody.velocity.y <= 0 && isJumping) {
            Debug.Log("Done Jump");
            isJumping = false;
        }

        //Check if grounded
        if (Physics2D.OverlapBox(GroundCheck.transform.position, groundCheckSize, 0, WhatIsGround))
        {
            lastGroundTime = coyoteTime;
        }

        if (cRigidBody.velocity.y < 0) {
            cRigidBody.gravityScale = gravityScale * fallGravityMultiplier;
        } else {
            cRigidBody.gravityScale = gravityScale;
        }

        float targetSpeed = hMove * moveSpeed;
        float speedDif = targetSpeed - cRigidBody.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerationRate : deccelerationRate;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        cRigidBody.AddForce(movement * Vector2.right);

        //ADD FRICTION

        //Check if in ground
        /*
        //If shooting take shoot action
        if (grounded && shoot && CurBlotCharges > 0)
        {
            Fire();
        }

        //If not firing, calc move
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot") && canMove)
            Move(); 

        //Reset State*/

        shoot = false;
    }

    private void OnJump() {
        if (!isJumping && lastGroundTime > 0)
        {
            Debug.Log("Jump");
            //zero vertical momentum
            cRigidBody.velocity = cRigidBody.velocity * Vector2.right;
            cRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastGroundTime = 0;
            isJumping = true;
            jumpInputReleased = false;
        }
    }

    private void OnJumpUp() {
        if (cRigidBody.velocity.y > 0 && isJumping) {
            cRigidBody.AddForce(Vector2.down * cRigidBody.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        jumpInputReleased = true;
    }

    private void Move() {

        //Apply Input, and smooth velocity
        var targetVelocity = new Vector2(hMove * Time.deltaTime * 10, 0);
        Vector3 vel = Vector3.zero;
        //cRigidBody.velocity = Vector3.SmoothDamp(cRigidBody.velocity, targetVelocity, ref vel, MovementSmoothing);

        //Flip facing direction
        if (hMove > 0 && !facingRight) 
        {
            Flip();
        } else if (hMove < 0 && facingRight) 
        {
            Flip();
        }

        //Walking anim state
        if (Input.GetAxisRaw("Horizontal") != 0 && !animator.GetBool("IsWalking"))
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
        }
        else if (Input.GetAxisRaw("Horizontal") == 0 && !animator.GetBool("IsIdle")) {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
        }

        //Jump code
        if (grounded && jump) {
            grounded = false;
            //cRigidBody.AddForce(new Vector2(0f, jumpHeight * 10));
            animator.SetBool("IsJumping", true);
        }

        if (!grounded && cRigidBody.velocity.y < 0) {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
        }

        if (grounded) {
            animator.SetBool("IsFalling", false); 
        }
    }

    private void Flip() {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void Fire() 
    {
        //firing = true;
        CurBlotCharges--;
        animator.SetTrigger("Shoot");
        blot = Instantiate(Resources.Load("Prefabs/Blot"), new Vector3(transform.position.x, transform.position.y + 0.2f, 0), Quaternion.identity) as GameObject;
        blot.GetComponent<InkShotController>().SetParent(gameObject);

        //animator.Play("Shoot");
    }

    public void BlotKilled() {
        blot = null;
        UIController.instance.LoseHealth();
    }

    public void Kill() {
        transform.position = CurRespwawnAnchor.position;
        UIController.instance.ResetHealth();
        GameObject.Destroy(blot);
        blot = null;
        StartCoroutine(FreezeMoveRespawn());
    }

    private IEnumerator FreezeMoveRespawn() { 
        canMove = false;
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }

    public void SetSpawn(Transform spawnPoint) {
        CurRespwawnAnchor = spawnPoint;
    }

    public void ReplenishInk() {
        if (CurBlotCharges != StartingBlotCharges && blot == null)
        {
            CurBlotCharges = StartingBlotCharges;
            UIController.instance.ResetHealth();
        }
    }

    public void EnterNoBlotZone() {
        canUseBlot = false;
    }

    public void LeaveNoBlotZone() {
        canUseBlot = true;
    }

}
