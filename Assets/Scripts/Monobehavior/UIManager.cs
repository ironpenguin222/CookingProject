using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    // UI Elements

    public Slider healthBar;
    public Slider staminaBar;
    public TMP_Text flaskText;
    public TMP_Text flaskNum;

    public void UpdateUI(float currentHealth, float maxHealth, float currentStamina, float maxStamina, int flasks)
    {
        // Adjusts Values

        healthBar.value = currentHealth / maxHealth;
        staminaBar.value = currentStamina / maxStamina;
        flaskText.text = "Cubestus Flask";
        flaskNum.text = "" + flasks;
    }
}
