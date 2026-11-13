using UnityEngine;

// Final Boss B: a Gun subclass that emits a symmetric spread per shot.
public class ShotGun : Gun
{
    [SerializeField, Range(2, 9)] private int pelletCount = 5;
    [SerializeField, Range(1f, 90f)] private float spreadAngle = 30f;

    public override void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angle = Mathf.Lerp(-spreadAngle * 0.5f, spreadAngle * 0.5f,
                i / (float)(pelletCount - 1));
            SpawnBullet(angle);
        }
        ApplyRecoilAndSound();
    }
}
