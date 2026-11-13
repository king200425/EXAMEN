using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject bulletSpawnPoint;

    //El recoil de pistol duel funciona de la següent forma
    // 1) S'aplica una força cap amunt anul·lant prèviament la velocitat Y
    // 2) S'aplica un torque (anul·lant la velocitat angular actual) depenent d'on miri l'arma
    // 3) S'aplica una força en X depenent de la orientació de l'arma
    [SerializeField]
    private float forceRecoilFactor;
    [SerializeField]
    private float torqueRecoilFactor;

    [SerializeField]
    private float bulletImpulseForce = 10.0f;
    [SerializeField]
    private bool isFriendly = true;

    [SerializeField]
    private AudioModulator shotAudio;

    public virtual void Shoot()
    {
        SpawnBullet(0f);
        ApplyRecoilAndSound();
    }

    protected void SpawnBullet(float angleOffset)
    {
        //Instanciar i impulsar la bala
        GameObject.Instantiate(bulletPrefab, 
            bulletSpawnPoint.transform.position, 
            bulletSpawnPoint.transform.rotation * Quaternion.Euler(0f, 0f, angleOffset),
            null).GetComponent<Bullet>().Shoot(bulletImpulseForce, isFriendly);
    }

    protected void ApplyRecoilAndSound()
    {
        BulletVfx.MuzzleFlash(bulletSpawnPoint.transform);

        float rightAlignment = Vector2.Dot(this.transform.right, Vector2.right) * Mathf.Sign(transform.localScale.x);

        //Linear force
        rb.linearVelocityY = 0.0f;
        rb.AddForce(new Vector2(0.5f * -rightAlignment * forceRecoilFactor, forceRecoilFactor), ForceMode2D.Impulse);
        
        //AngularForce
        rb.angularVelocity = 0.0f;
        rb.AddTorque(rightAlignment > 0 ? torqueRecoilFactor : -torqueRecoilFactor);

        shotAudio.Play();
    }
}
