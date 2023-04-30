using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool isShaking = false;
    public float shakeDuration = 0.0f;
    public float shakeMagnitude = 0.0f;
    public float damping = 0.0f;
    public Vector3 initialPosition;

    public void Shake(float duration = 0.5f, float magnitude = 0.1f, float damping = 1.0f)
    {
        initialPosition = transform.localPosition;
        isShaking = true;

        shakeDuration = duration;
        shakeMagnitude = magnitude;
        this.damping = damping;

        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        while (shakeDuration > 0)
        {
            var magnitude = shakeMagnitude * shakeDuration / damping;
            transform.localPosition = initialPosition + Random.insideUnitSphere * magnitude;

            shakeDuration -= Time.deltaTime;

            yield return null;
        }

        shakeDuration = 0.0f;
        transform.localPosition = initialPosition;
        isShaking = false;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
