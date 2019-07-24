using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private IEnemyState currentState;

    public GameObject Target { get; set; }

    private Vector3 startPos;
    [SerializeField]
    private Transform leftEdge;
    [SerializeField]
    private Transform rightEdge;

    [SerializeField]
    private float meleeRange;

    private Canvas healthCanvas;

    private bool starIsBorn ;

    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                Debug.Log(Vector2.Distance(transform.position, Target.transform.position));
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }
            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
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
        starIsBorn = false;
        base.Start();
        startPos = transform.position;
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        //first, set idle state
        ChangeState(new IdleState());
        healthCanvas = transform.GetComponentInChildren<Canvas>();
    }

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;

            if (xDir < 0 && facingRight || xDir > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }
            //update state
            LookAtTarget();
        }

    }

    public void ChangeState(IEnemyState newState)
    {
        //check if state != null, exit this state to go to the next state
        if (currentState != null)
        {
            currentState.Exit();
        }
        if (newState is PatrolState)
		{
			AudioManager.PlaySound("Walk");
		}
		else
		{
			AudioManager.StopSound("Walk");
		}
        //assign new state
        currentState = newState;
        //enter new state
        currentState.Enter(this);
    }

    public void Move()
    {
        if (!Attack)
        {
            if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x)  || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                //set to 1 so that it can move
                PlayerAnimator.SetFloat("speed", 1);

                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }
            else if (currentState is PatrolState)
            {
                ChangeDirection();
            }else if(currentState is RangeState)
            {
                Target = null;
                ChangeState(new IdleState());
            }

        }
       
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }

    public override IEnumerator TakeDamage()
    {
        if (!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }
        health.CurrentVal -= 10;

        //if not dead show damage animation
        if (!IsDead)
        {
            PlayerAnimator.SetTrigger("damage");
        }
        else
        {
            if (!starIsBorn)
            {
                starIsBorn = true;
                GameObject star = Instantiate(GameManager.Instance.StarPrefab, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity);
                Physics2D.IgnoreCollision(star.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
            PlayerAnimator.SetTrigger("die");
            yield return null;
        }
    }

    public override void ChangeDirection()
    {
        //Makes a reference to the enemys canvas
        Transform tmp = transform.Find("EnemyHealthBarCanvas").transform;

        //Stores the position, so that we know where to move it after we have flipped the enemy
        Vector3 pos = tmp.position;

        ///Removes the canvas from the enemy, so that the health bar doesn't flip with it
        tmp.SetParent(null);
        base.ChangeDirection();
        //Puts the health bar back on the enemy.
        tmp.SetParent(transform);

        //Pits the health bar back in the correct position.
        tmp.position = pos;
    }

    public override void Death()
    {
        PlayerAnimator.ResetTrigger("die");
        //PlayerAnimator.SetTrigger("idle");
        //health.CurrentVal = health.MaxVal;
        //transform.position = startPos;
        Destroy(gameObject);
        //healthCanvas.enabled = false;
        Destroy(healthCanvas);
    }
}
