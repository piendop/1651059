using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();
public class Player : Character
{
    private static Player instance;
    private Vector3 startPos;
    public event DeadEventHandler Dead;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
        set => instance = value;
    }

    private bool run;
   
    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;


    [SerializeField]
    private bool airControl;
    [SerializeField]
    private float jumpForce;

    public Rigidbody2D PlayerRigidbody { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool OnGround { get; set; }
    private bool immortal = false;
    [SerializeField]
    private float immortalTime;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public override bool IsDead
    {
        get
        {
            if (health.CurrentVal<=0)
                OnDead();
            return health.CurrentVal <= 0;
        }

        set
        {
            IsDead = value;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        startPos = transform.position;
        //reference rigid body of the player to variable player
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health.CurrentVal -= 10;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            health.CurrentVal += 10;
        }
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -14f)
            {
                Death();
            }
            HandleInput();
        }
    }
    //FixedUpdate is called based on time
    void FixedUpdate()
    {
        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();

            HandleMovement(horizontal);

            Flip(horizontal);

            HandleLayers();
        }

    }
    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }
    //handle the movement of the character
    private void HandleMovement(float horizontal)
    {
        if (IsFalling)
        {
            gameObject.layer = 11;
            PlayerAnimator.SetBool("land", true);
        }
        if(!Attack && !Run && (OnGround || airControl))
        {
            AudioManager.PlaySound("Walk");
            PlayerRigidbody.velocity = new Vector2(horizontal * movementSpeed, PlayerRigidbody.velocity.y);
        }
        else
        {
            AudioManager.PlaySound("Walk");
        }
        if (Jump && PlayerRigidbody.velocity.y == 0)
        {
            PlayerRigidbody.AddForce(new Vector2(0, jumpForce));
        }
        if (Run)
        {
            PlayerRigidbody.velocity = new Vector2(horizontal * movementSpeed*2, PlayerRigidbody.velocity.y);
        }
        PlayerAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    //handle input from keyboard
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsFalling)
        {
            PlayerAnimator.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PlayerAnimator.SetTrigger("run");
        }
    }

    //flip player
    private void Flip(float horizontal)
    {
        //check if player need to flip facing right and facing left
        if (horizontal > 0 && !facingRight || horizontal<0 && facingRight)
        {
            ChangeDirection();
        }
    }


    private bool IsGrounded()
    {
        //velocity in y-axis <= 0 => falling down
        if (PlayerRigidbody.velocity.y <= 0)
        {
            //check collison
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for(int i = 0; i < colliders.Length; ++i)
                {
                    //if collide to something not player
                    if(colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }

    private void HandleLayers()
    {
        //check if the player is not on the ground
        if (!OnGround)
        {
            PlayerAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            PlayerAnimator.SetLayerWeight(1, 0);
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }
    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
            health.CurrentVal -= 10;
            if (!IsDead)
            {
                PlayerAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);

                immortal = false;
            }
            else
            {
                PlayerAnimator.SetLayerWeight(1, 0);
                PlayerAnimator.SetTrigger("die");
            }
        }
    }

    public override void Death()
    {
        PlayerRigidbody.velocity = Vector2.zero;
        PlayerAnimator.SetTrigger("idle");
        health.CurrentVal = health.MaxVal;
        transform.position = startPos;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            GameManager.Instance.CollectedStars++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name == "TombStone")
        {
            PlayerAnimator.SetLayerWeight(1, 0);
            PlayerAnimator.SetTrigger("die");
            Destroy(other.gameObject);
        }
    }

    public bool IsFalling
    {
        get
        {
            return PlayerRigidbody.velocity.y < 0;
        }
    }
}
