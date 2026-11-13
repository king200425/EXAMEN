using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    protected Gun gun;

    [SerializeField]
    protected HealthSystem healthSystem;

    [SerializeField]
    protected DeathTimer deathTimer;

    protected float minShotTime = 0.5f;
    protected float maxShotTime = 2.5f;
    protected float nextShotTime = 0.0f;
    protected float currentShotTime = 0.0f;

    protected virtual void Start()
    {
        GameManager.Instance.NotifyEnemySpawn();
        healthSystem.OnDeath += delegate { 
            deathTimer.enabled = true;
            Destroy(this);
            Destroy(healthSystem);
            GameManager.Instance.NotifyEnemyDeath();
        };
    }

    protected virtual void Think()
    {
        currentShotTime += Time.deltaTime;
        if(currentShotTime >= nextShotTime)
        {
            Shoot();
            currentShotTime = 0.0f;
            nextShotTime = Random.Range(minShotTime, maxShotTime);
        }
    }

    protected virtual void Shoot()
    {
        gun.Shoot();
    }

    // Update is called once per frame
    void Update()
    {
        Think();
    }
}
