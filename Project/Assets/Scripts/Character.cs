using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;
    public Animator PlayerAnimator { get; set; }
    [SerializeField]//to test speed in player's inspector
    protected float movementSpeed;
    //bool variable to flip player
    protected bool facingRight;
    public bool Attack { get; set; }

    public bool TakingDamage { get; set; }
    [SerializeField]
    protected Stat health;

    [SerializeField]
    private EdgeCollider2D swordCollider;

    [SerializeField]
    private List<string> damageSources;
    public abstract bool IsDead { get; set; }
    public EdgeCollider2D SwordCollider { get => swordCollider; set => swordCollider = value; }
    public AudioManager AudioManager { get => audioManager; set => audioManager = value; }

    // Start is called before the first frame update
    public virtual void Start()
    {
        
        health.Initialize();
        facingRight = true;
        //get animator from the player in the world
        PlayerAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Death();
    public virtual void ChangeDirection()
    {
        
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);

    }

    public abstract IEnumerator TakeDamage();

    public void MeleeAttack()
    {
        SwordCollider.enabled = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
    }
}
