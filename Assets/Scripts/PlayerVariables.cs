using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVariables : MonoBehaviour
{

    public int health;
    public int shield;
    public int speed;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI speedText;

    private void Update()
    {
        speed = (int)gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        UpdateUI();
        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Health: " + health;
        shieldText.text = "Shield: " + shield;
        speedText.text = "Speed: " + speed;
    }

    private void Die()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }

    public void TakeDamage(int damage)
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

    }

    public void Heal(int heal)
    {
        health += heal;
    }

    public void AddShield(int shield)
    {
        this.shield += shield;
    }

}
