using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    public bool IsFriendly { get; private set; } = true;

    [field: SerializeField]
    public int Damage { get; private set; } = 1;

    private const int friendlyBulletLayer = 7; //Està definit a les layers de física
    private const int enemyBulletLayer = 9;

    [SerializeField]
    private GameObject playerHit;
    [SerializeField]
    private GameObject enemyHit;
    public void Shoot(float shootForce, bool isFriendly = true)
    {
        rb.AddForce(this.transform.right * shootForce, ForceMode2D.Impulse);

        IsFriendly = isFriendly;
        this.gameObject.layer = IsFriendly ? friendlyBulletLayer : enemyBulletLayer;
    }

    private const int playerLayer = 6;
    private const int enemyLayer = 8;

    [SerializeField, Min(0f)] private float pushImpulse = 2.5f;

    public void OnCollisionEnter2D(Collision2D collision){
        Rigidbody2D target = collision.rigidbody;
        if (target != null && target.bodyType == RigidbodyType2D.Dynamic)
        {
            Vector2 point = collision.contactCount > 0
                ? collision.GetContact(0).point : (Vector2)transform.position;
            target.AddForceAtPosition((Vector2)transform.right * pushImpulse, point, ForceMode2D.Impulse);
        }
        HealthSystem hs = collision.gameObject.GetComponent<HealthSystem>();
        if (hs != null)
        {
            hs.TakeDamage(Damage);
            Instantiate(IsFriendly ? enemyHit : playerHit, this.transform.position, Quaternion.identity, null);
        }

        SpawnImpactVfx(collision);
        Destroy(this.gameObject);
    }



    // EL CODI A CONTINUACIÓ ESTÀ FET AMB CLAUDE CODE OPUS 5.5 HIGH
    // No hi ha cap part de l'examen que estigui a codi fet amb IA
    private void SpawnImpactVfx(Collision2D collision)
    {
        Vector2 point = this.transform.position;
        Vector2 normal = -(Vector2)this.transform.right;
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);
            point = contact.point;
            normal = contact.normal; // apunta desde la superficie hacia la bala
        }

        int layer = collision.gameObject.layer;
        if (layer == playerLayer || layer == enemyLayer)
        {
            // Impacto en un personaje: estallido de 360º con su color.
            GameObject body = collision.rigidbody != null ? collision.rigidbody.gameObject : collision.gameObject;
            SpriteRenderer bodySprite = body.GetComponentInChildren<SpriteRenderer>();
            Color bodyColor = bodySprite != null ? bodySprite.color : Color.white;
            BulletVfx.BodyImpact(point, -normal, bodyColor);
        }
        else
        {
            // Impacto en pared: abanico de 180º alrededor de la normal.
            BulletVfx.WallImpact(point, normal);
        }
    }
}
