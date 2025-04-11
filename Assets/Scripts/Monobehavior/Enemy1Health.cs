using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy1Health : MonoBehaviour
{
    public string enemyName = "Enemy";
    public int maxHealth = 100;
    public int currentHealth;
    public bool tookDamage = false;
    public float iFrames = 0f;

    public TextMeshProUGUI enemyNameText;
    public Slider healthBar;
    public GameObject player;
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
        if (tookDamage)
        {
            iFrames += Time.deltaTime;
            if (iFrames >= 0.6f)
            {
                tookDamage = false;
                iFrames = 0f;
            }
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController.currentHealth <= 0)
        {
            StartCoroutine(FadeIn(winIMG));
            player.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
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
        healthBar.value = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log(enemyName + " has died.");
        StartCoroutine(FadeIn(exeIMG));
        player.SetActive(false);
    }

    private IEnumerator FadeIn(Image img)
    {
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
