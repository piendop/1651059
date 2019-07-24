using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisonTrigger : MonoBehaviour
{
   

    [SerializeField]
    private BoxCollider2D platformCollider;

    [SerializeField]
    private BoxCollider2D platformTrigger;
    // Start is called before the first frame update
    void Start()
    {
        //get box collider of player
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.name == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.name == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }
    }
}
