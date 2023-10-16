using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkShotController : MonoBehaviour
{
    [SerializeField] private float Lifetime = 3f;
    [SerializeField] private float Speed = 1f;
    [SerializeField] private LayerMask WhatIsGround;
    private Vector2 MousePosition;

    //PlatformHitboxSizes
    private float S_Start = 0.2f;
    private float S_End = 1.28f;

    //Children
    [SerializeField] private GameObject BlotHBox;
    [SerializeField] private GameObject PlatformHBox;

    //Parent
    private GameObject parent;

    //Components
    private Animator animator;

    //State
    private bool isPlatform = false;
    private bool canBePlatform = false;
    private GameObject CurPlatformCol = null;
    private GameObject CurStoppedWaterfall = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPlatform)
            transform.position = Vector2.MoveTowards(transform.position, MousePosition, Speed * Time.deltaTime);
    }

    private void Update()
    {

        if (Input.GetButtonDown("Fire2") && !isPlatform && canBePlatform)
        {
            BecomePlatform();
        }

        if(!Input.GetButton("Fire2") && isPlatform) {
            BecomeBlot();
        }

        if (!isPlatform) { 
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void SetParent(GameObject parent) { 
        this.parent = parent;
    }

    private void BecomePlatform() {
        //Debug.Log("Becoming Platform");
        isPlatform = true;
        BlotHBox.SetActive(false);
        PlatformHBox.SetActive(true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = CurPlatformCol.GetComponent<Renderer>().bounds.center;
        Debug.Log("Positon I want: " + CurPlatformCol.GetComponent<Renderer>().bounds.center + " What I get: " + transform.position);

        animator.SetBool("IsPlatform", true);
        animator.SetBool("IsBlot", false);

        StopAllCoroutines();
        StartCoroutine(GrowPlatform());
    }

    private IEnumerator GrowPlatform()
    {
        var col = PlatformHBox.GetComponent<BoxCollider2D>();

        while (col.size.x < S_End) {
            col.size = new Vector2(col.size.x + 0.1f, col.size.y);
            yield return new WaitForSeconds(0.01f);
        }

        col.size = new Vector2(S_End, col.size.y);

    }

    private void BecomeBlot() {
        Debug.Log("Becoming Blot");
        isPlatform = false;
        CurPlatformCol = null;
        canBePlatform = false;
        if (CurStoppedWaterfall != null) {
            CurStoppedWaterfall.GetComponent<WaterfallController>().ExitPlatform();
            CurStoppedWaterfall = null;
        }
        animator.SetBool("IsPlatform", false);
        animator.SetBool("IsBlot", true);
        StopAllCoroutines();
        StartCoroutine(ShrinkPlatform());
    }

    private IEnumerator ShrinkPlatform() {
        var col = PlatformHBox.GetComponent<BoxCollider2D>();

        while (col.size.x > S_Start)
        {
            col.size = new Vector2(col.size.x - 0.1f, col.size.y);
            yield return new WaitForSeconds(0.01f);
        }

        col.size = new Vector2(S_Start, col.size.y);
        PlatformHBox.SetActive(false);
        BlotHBox.SetActive(true);
        Debug.Log("Platform shurnk: ");
    }

    public bool GetIsPlatform() { 
        return isPlatform;
    }

    public void StopWaterfall(GameObject stopped) {
        CurStoppedWaterfall = stopped;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enter");
        if (collision.gameObject.layer == 8) {
            parent.GetComponent<CharacterController>().BlotKilled();
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == 9 && !isPlatform) {
            collision.GetComponent<Animator>().SetBool("IsHighlighted", true);
            CurPlatformCol = collision.gameObject;
            canBePlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Exit");
        if (collision.gameObject.layer == 9 && !isPlatform)
        {
            collision.GetComponent<Animator>().SetBool("IsHighlighted", false);
            CurPlatformCol = null;
            canBePlatform = false;
        }
    }
}
