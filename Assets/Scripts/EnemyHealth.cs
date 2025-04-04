using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public string enemyName = "Enemy";
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI enemyNameText;
    public Slider healthBar;
    public GameObject player;
    public int detectionRange = 5;
    public float tookDamage = 0;
    public float iFrames = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyNameText.text = enemyName;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (tookDamage == 1)
        {
            iFrames += Time.deltaTime;
        }
        if(iFrames >= 0.5)
        {
            tookDamage = 0;
            iFrames = 0;
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
        if (distance < detectionRange && tookDamage == 0)
        {

        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController.isAttacking)
            {
                TakeDamage(10);
                tookDamage = 1;
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
        gameObject.SetActive(false);
    }
}
