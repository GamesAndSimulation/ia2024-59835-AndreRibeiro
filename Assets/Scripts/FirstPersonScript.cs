using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class FirstPersonScript : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private PostProcessVolume postProcessVolume;
    private Vignette vignetteLayer;
    private float intensity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        postProcessVolume = GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out vignetteLayer);
        vignetteLayer.enabled.Override(false);

    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public float getCurrentFOV()
    {
        return GetComponent<Camera>().fieldOfView;
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void TakeDamage()
    {
        StartCoroutine(DoVignette());
    }

    private IEnumerator DoVignette()
    {
        intensity = 0.4f;

        vignetteLayer.enabled.Override(true);
        vignetteLayer.intensity.Override(intensity);

        yield return new WaitForSeconds(0.4f);

        while (intensity > 0)
        {
            intensity -= 0.01f;
           
            if(intensity < 0)
            {
                intensity = 0;
            }

            vignetteLayer.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }

        vignetteLayer.enabled.Override(false);

        yield break;
    }

}
