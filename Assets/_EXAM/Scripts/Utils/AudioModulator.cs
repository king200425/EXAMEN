using UnityEngine;

public class AudioModulator : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSrc;

    // Cuidao que si els audios tenen el volum diff de 1 es bugueja això
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float minVolume = 0.95f;
    public float maxVolume = 1.05f; 
    public void Play()
    {
        audioSrc.pitch = Random.Range(minPitch, maxPitch);
        audioSrc.volume = Random.Range(minVolume, maxVolume);
        audioSrc.Play();
    }
}
