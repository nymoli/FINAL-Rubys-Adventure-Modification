using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public GameObject healthIncrease;
    public GameObject healthDecrease;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public GameObject winMusic;
    public GameObject loseMusic;

    private int scoreValue = 0;
    private int scoreAmount;
    public Text scoreText;

    public Text winText;
    public Text loseText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject bgMusic;

    bool gameOver = false;

    public int cogs = 3;
    public Text cogsCount;
        
    int distanceAway;   

    public AudioClip jambiSound;
    public GameObject jambiSoundObject;    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        scoreText.text = "Robots Fixed: " + scoreValue.ToString();

        cogsCount.text = "Cogs: " + cogs.ToString();

        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);

        winMusic.SetActive(false);
        loseMusic.SetActive(false);
        bgMusic.SetActive(true);

        jambiSoundObject.SetActive(false);        

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && cogs > 0)
        {
            Launch();
            cogs -= 1;
            cogsCount.text = "Cogs: " + cogs;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));            

            if (hit.collider != null)
            {
                              

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    jambiSoundObject.SetActive(true);
                    PlaySound(jambiSound);

                    if (scoreValue == 4)
                    {
                        SceneManager.LoadScene("Scene2");
                    }
                }


            }
        }

              
            if (Input.GetKey(KeyCode.R))
            {

                if (gameOver == true)

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

                }

            }
        

        scoreText.text = "Robots Fixed: " + scoreValue.ToString();


               
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    

    

    public void ChangeScore(int ScoreAmount)
    {
        scoreValue += 1;
        scoreText.text = "Robots Fixed: " + scoreText.ToString();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            if (scoreValue == 4)
            {
                
                winText.text = "Talk to Jambi to visit Stage 2.";
                winTextObject.SetActive(true);

                gameOver = true;

                winMusic.SetActive(true);
                bgMusic.SetActive(false);
                audioSource.mute = audioSource.mute;

            }

        if (SceneManager.GetActiveScene().buildIndex == 1)
            if (scoreValue == 4)
            {

                winText.text = "Your win! Created by Nyah Molina.";
                winTextObject.SetActive(true);

                gameOver = true;

                winMusic.SetActive(true);
                bgMusic.SetActive(false);
                audioSource.mute = audioSource.mute;

            }
        
    }


    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
        
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            GameObject projectilePrefab = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            PlaySound(hitSound);

            
        }
        
        if (amount > 0)
        {
            GameObject projectilePrefab = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);

        if (currentHealth == 0)
        {
            loseText.text = "You lose! Press 'R' to restart.";
            loseTextObject.SetActive(true);

            gameOver = true;

            loseMusic.SetActive(true);
            bgMusic.SetActive(false);
            audioSource.mute = audioSource.mute;
        }
    }

    private void CollectAmmo()
    {
        cogsCount.text = "Cogs: " + cogs.ToString();

        if (Input.GetKeyDown(KeyCode.C) && cogs > 0) 
        {   
            Launch(); 
            cogs -= 1; 
            cogsCount.text = "Cogs: " + cogs; 
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            other.gameObject.SetActive(false);
            cogs = cogs + 4;

            CollectAmmo();
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);

        
    }

   

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
