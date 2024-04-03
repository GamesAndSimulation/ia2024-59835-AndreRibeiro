using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVariables : MonoBehaviour
{

    public float health;
    public float shield;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;

    private void Update()
    {
        healthText.text = "Health: " + health;
        shieldText.text = "Shield: " + shield;
    }

    public void TakeDamage(float damage)
    {
        if((shield - damage) > 0)
        {
            shield -= damage;
        }

        else if (shield > 0)
        {
            shield = 0;
            health -= (damage - shield);
        }
        else
        {
            health -= damage;
        }

        Debug.Log("Health: " + health + " Shield: " + shield);
    }

    public void Heal(float heal)
    {
        health += heal;
    }

    public void AddShield(float shield)
    {
        this.shield += shield;
    }

}
