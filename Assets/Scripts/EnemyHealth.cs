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

    private void Start()
    {
        currentHealth = maxHealth;
        enemyNameText.text = enemyName;
        UpdateHealthUI();
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
