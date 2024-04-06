using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public int health;
    public GameObject explosionPE;
    public AudioClip explosionSound;
    public bool isMainframe;

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    void Die()
    {
        Vector3 explosionTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        GameObject explosionInstance = Instantiate(explosionPE, explosionTransform, Quaternion.identity);
        AudioSource explosionSource = explosionInstance.AddComponent<AudioSource>();
        explosionSource.clip = explosionSound;
        explosionSource.volume = 0.2f;
        explosionSource.Play();
        if(isMainframe)
        {
            GameObject.Find("EventSystem").GetComponent<LevelScript>().Ending1();
        }
        Destroy(explosionInstance, explosionSound.length + 2f);
        Destroy(gameObject);
    }
}
