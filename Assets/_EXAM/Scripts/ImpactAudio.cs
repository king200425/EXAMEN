using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ImpactAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField, Min(1f)] private float fullVolumeForce = 500f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (audioSource == null || audioSource.clip == null) return;
        float impulse = 0f;
        for (int i = 0; i < collision.contactCount; i++)
            impulse += Mathf.Abs(collision.GetContact(i).normalImpulse);
        float force = impulse / Time.fixedDeltaTime;
        audioSource.volume = Mathf.Clamp01(force / fullVolumeForce);
        if (audioSource.volume > 0f) audioSource.Play();
    }
}
