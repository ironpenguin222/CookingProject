using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeCanvas.Tasks.Actions;

public class EnemyHealth : MonoBehaviour
{
    //Variable values

    public string enemyName = "Enemy";
    public int maxHealth = 100;
    public int maxSus = 10;
    public int currentHealth;
    public float suspicion;
    public bool tookDamage = false;
    public float thresholds;

    public Image susIMG;
    public Image winIMG;
    public Image exeIMG;
    public float fadeDuration = 3f;


    public TextMeshProUGUI enemyNameText;
    public Slider healthBar;
    public Slider susBar;
    public GameObject player;
    public int detectionRange = 5;
    public float iFrames = 0;

    private void Start()
    {
        // Default Values
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.currentHealth = maxHealth;
        currentHealth = maxHealth;
        enemyNameText.text = enemyName;
        UpdateHealthUI();
        UpdateSusUI();

        //Make UI Transparent

        Color c1 = susIMG.color;
        c1.a = 0f;
        susIMG.color = c1;

        Color c2 = exeIMG.color;
        c2.a = 0f;
        exeIMG.color = c2;

        Color c3 = winIMG.color;
        c3.a = 0f;
        winIMG.color = c3;
    }

    private IEnumerator FadeIn(Image img)
    {
        // Fades in the UI to show to the player in a less sudden way. Uses a lerp to slowly return alpha.


        float elapsed = 0f;
        Color originalColor = img.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        img.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    private void Update()
    {

        // Checks taken to damage to make sure can't get hit multiple times per swing

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (tookDamage)
        {
            iFrames += Time.deltaTime;
        }
        if(iFrames >= 0.6)
        {
            tookDamage = false;
            iFrames = 0;
        }

        // Makes sure suspicion is more than 0

        if (suspicion < 0){
            suspicion = 0;
        }
        UpdateSusUI();

        // Ends game if suspicion is too high

        if(suspicion >= 10)
        {
            StartCoroutine(FadeIn(susIMG));
            player.SetActive(false);
        }

        // Get player health and if at 0 and suspicion not too high, player win

        if(suspicion < 10 && playerController.currentHealth <= 0)
        {
            StartCoroutine(FadeIn(winIMG));
            player.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        // enemy's health lowers and the UI lowers accordingly, which once at 0 leads to death

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange && tookDamage == false)
        {

        PlayerController playerController = player.GetComponent<PlayerController>();

            // Sees if player is attacking too much or too little to add suspicion

            if (playerController.attackCount > 3)
            {
                suspicion += 2;
                playerController.attackCount = 0;
            }

            if (playerController.attackCount < -2)
            {
                suspicion += 2;
                playerController.attackCount = 0;
            }

            // Checks distance and then take damage if player is attacking

            if (playerController.isAttacking)
            {
                suspicion += 0.5f;
                TakeDamage(10);
                tookDamage = true;
            }
        }
    }

    private void UpdateHealthUI()
    {
        // Updates the health bar

        healthBar.value = (float)currentHealth / maxHealth;

        // health thresholds to incentivize sometimes hitting enemy

        if (currentHealth <= 70 && thresholds == 0)
        {
            suspicion -=1;
            thresholds += 1;
        }
        if (currentHealth <= 50 && thresholds == 1)
        {
            suspicion -=2;
            thresholds += 1;
        }
        if (currentHealth <= 50 && thresholds == 2)
        {
            suspicion -=3;
            thresholds += 1;
        }
    }

    private void UpdateSusUI()
    {
        // Update sus bar

        susBar.value = suspicion / maxSus;
    }

    private void Die()
    {
        // Enemy dies and player gets executed

        Debug.Log(enemyName + " has died.");
        StartCoroutine(FadeIn(exeIMG));
        player.SetActive(false);
    }
}
