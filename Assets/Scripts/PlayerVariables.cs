using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVariables : MonoBehaviour
{

    public int health;
    public int shield;
    public int speed;
    public int ammo;
    public int score;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;

    public GameObject cameraPP;
    public LevelScript levelScript;

    private void Update()
    {
        speed = (int)gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        ammo = GameObject.Find("SciFiGunLightRad").GetComponent<GunSystem>().bulletsLeft;
        UpdateUI();
        if (health <= 0 || gameObject.transform.position.y < -50)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Health: " + health;
        shieldText.text = "Shield: " + shield;
        speedText.text = "Speed: " + speed;
        ammoText.text = "Ammo: " + ammo;
        scoreText.text = "Score: " + score;
    }

    private void Die()
    {
        levelScript.GameOver();
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

        cameraPP.GetComponent<FirstPersonScript>().TakeDamage();
    }

    public void Heal(int heal)
    {
        health += heal;
    }

    public void AddShield(int shield)
    {
        this.shield += shield;
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

}
