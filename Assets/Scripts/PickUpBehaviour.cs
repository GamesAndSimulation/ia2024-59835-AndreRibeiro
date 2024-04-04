using UnityEngine;

public class PickUpBehaviour : MonoBehaviour
{

    public PickUpType type;
    public int value;
    public AudioClip sfx;
    private AudioSource audioSrc;

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public enum PickUpType
    {
        Health,
        Shield
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + " picked up " + type + " " + value + "!");
            DoPickUp(other);
        }

    }

    private void DoPickUp(Collider other)
    {
        switch (type)
        {
            case PickUpType.Health:
                other.gameObject.GetComponentInParent<PlayerVariables>().Heal(value);
                break;
            case PickUpType.Shield:
                other.gameObject.GetComponentInParent<PlayerVariables>().AddShield(value);
                break;
        }

        GameObject soundLeftover = new GameObject("Sound");
        AudioSource soundSrc = soundLeftover.AddComponent<AudioSource>();
        soundSrc.clip = sfx;
        soundSrc.Play();
        Destroy(soundLeftover, sfx.length+1f);
        Destroy(gameObject);
    }
}
