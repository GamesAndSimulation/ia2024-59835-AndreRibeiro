using UnityEngine;

public class Sound_PlayerMovement : MonoBehaviour
{

    [Header("Sound")]
    public AudioSource audioSrc;
    public AudioClip[] footstepSFX;
    public AudioClip slideSFX;
    private float nextStepTime;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlayFootsteps()
    {
        float vel = rb.velocity.magnitude;
        float stepInterval = Mathf.Lerp(0.5f, 0.2f, vel / 20);

        if (vel > 0.1f && Time.time > nextStepTime)
        {
            audioSrc.clip = footstepSFX[Random.Range(0, footstepSFX.Length)];
            audioSrc.Play();
            nextStepTime = Time.time + stepInterval;
        }
    }

    public void PlaySlide()
    {
        audioSrc.clip = slideSFX;
        if(!audioSrc.isPlaying)
            audioSrc.Play();
    }
}
