using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToughEnemyController : MonoBehaviour
{
    public float speed;    

    public float changeTime = 3.0f;

    public ParticleSystem smokeEffect;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;

    private RubyController rubyController;

    Animator animator;

    public int hitValue;

    private Transform target;

    float horizontal;
    float vertical;
    Vector2 lookDirection = new Vector2(1, 0);


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        GameObject rubyControllerObject = GameObject.FindWithTag("rubyController");

        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
        }

        target = GameObject.FindGameObjectWithTag("rubyController").GetComponent<Transform>();

    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);


        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);




    }

    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;

        rigidbody2D.MovePosition(position);
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-3);
        }
    }

        
        public void MultipleHits()
        {
            hitValue += 1;    

            if (hitValue >= 3)
            {
                Fix();
            }

        }

    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");

        smokeEffect.Stop();

        if (rubyController != null)
        {
            rubyController.ChangeScore(1);
        }
    }

}
