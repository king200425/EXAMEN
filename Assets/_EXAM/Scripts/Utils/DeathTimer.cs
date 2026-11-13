using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    public float timer = 0.5f;
    public GameObject deathParticles;
    void Start()
    {
        Invoke("Clean", timer);
    }

    void Clean()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, this.transform.position, this.transform.rotation, null);

        Destroy(this.gameObject);
    }
}
