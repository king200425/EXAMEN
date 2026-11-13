using System.Collections;
using UnityEngine;

public class AIControllerRifle : AIController
{
    protected override void Start()
    {
        GameManager.Instance.NotifyEnemySpawn();
        healthSystem.OnDeath += delegate {
            deathTimer.enabled = true;
            Destroy(this);
            Destroy(healthSystem);
            GameManager.Instance.NotifyEnemyDeath();
            StopAllCoroutines();
        };
    }
    protected override void Shoot()
    {
        StartCoroutine(Burst());
    }

    IEnumerator Burst()
    {
        gun.Shoot();
        yield return new WaitForSeconds(0.2f);
        gun.Shoot();
        yield return new WaitForSeconds(0.2f);
        gun.Shoot();
    }
}
