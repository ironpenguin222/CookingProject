using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy1Health : MonoBehaviour
{
    //Variable values

    public string enemyName = "Enemy";
    public int maxHealth = 100;
    public int currentHealth;
    public bool tookDamage = false;
    public float iFrames = 0f;
    public static int currEnemy = 1;

    public TextMeshProUGUI enemyNameText;
    public Slider healthBar;
    public GameObject player;
    public GameObject enemy2;
    public int detectionRange = 5;

    public Image winIMG;
    public Image exeIMG;
    public Image susIMG;
    public Image deadIMG;
    public float fadeDuration = 3f;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyNameText.text = enemyName;
        UpdateHealthUI();

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

        Color c4 = deadIMG.color;
        c4.a = 0f;
        deadIMG.color = c4;
    }

    private void Update()
    {

        // Checks taken to damage to make sure can't get hit multiple times per swing
        if (tookDamage)
        {
            iFrames += Time.deltaTime;
            if (iFrames >= 0.6f)
            {
                tookDamage = false;
                iFrames = 0f;
            }
        }
        // Checks is enemy is dead, then sets accordingly
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.currentHealth <= 0)
        {
            StartCoroutine(FadeIn(deadIMG));
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

        // Checks distance and then take damage if player is attacking

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange && !tookDamage)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController.isAttacking)
            {
                TakeDamage(10);
                tookDamage = true;
            }
        }
    }

    private void UpdateHealthUI()
    {
        // Updates health UI

        healthBar.value = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        // Kills enemy, spanws other enemy and sets current enemy variable to 2

        Debug.Log(enemyName + " has died.");
        currEnemy = 2;
        enemy2.SetActive(true);
        gameObject.SetActive(false);
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
}
