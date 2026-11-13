using System.Collections.Generic;
using UnityEngine;

/// <summary>Anima un grupo de partículas y se destruye solo al terminar.</summary>
public class VfxBurst : MonoBehaviour
{
    private readonly List<VfxParticle> particles = new List<VfxParticle>();

    public static VfxBurst Create(string name, Vector2 position, Vector2 direction, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(
            new Vector3(position.x, position.y, 0f),
            Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        // Evitar que herede escalas raras del padre (p. ej. un personaje volteado con escala -1).
        if (parent != null)
        {
            Vector3 ls = parent.lossyScale;
            go.transform.localScale = new Vector3(1f / NonZero(ls.x), 1f / NonZero(ls.y), 1f);
        }
        return go.AddComponent<VfxBurst>();
    }

    public void Add(VfxParticle p)
    {
        var child = new GameObject(p.shape.ToString());
        child.transform.SetParent(transform, false);
        p.tr = child.transform;
        p.sr = child.AddComponent<SpriteRenderer>();
        p.sr.sprite = VfxSprites.Get(p.shape);
        p.sr.sortingOrder = p.sortingOrder;
        p.sr.enabled = p.delay <= 0f;
        Apply(p, 0f);
        particles.Add(p);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        bool alive = false;
        // Gravedad del mundo expresada en el espacio local del efecto.
        Vector2 down = transform.InverseTransformDirection(Vector3.down);

        foreach (var p in particles)
        {
            if (p.delay > 0f)
            {
                p.delay -= dt;
                alive = true;
                if (p.delay > 0f) continue;
                p.sr.enabled = true;
            }
            if (p.age >= p.life) continue;

            p.age += dt;
            p.vel += down * (p.gravity * dt);
            p.vel *= Mathf.Exp(-p.drag * dt);
            p.pos += p.vel * dt;

            float t = Mathf.Clamp01(p.age / p.life);
            Apply(p, t);
            if (t >= 1f) p.sr.enabled = false;
            else alive = true;
        }

        if (!alive) Destroy(gameObject);
    }

    private static void Apply(VfxParticle p, float t)
    {
        float eased = 1f - (1f - t) * (1f - t) * (1f - t); // ease-out cúbico
        Vector2 size = Vector2.LerpUnclamped(p.sizeFrom, p.sizeTo, eased);
        p.tr.localPosition = p.pos;
        p.tr.localScale = new Vector3(size.x, size.y, 1f);

        float angle = p.rotation;
        if (p.alignToVelocity && p.vel.sqrMagnitude > 0.0001f)
            angle = Mathf.Atan2(p.vel.y, p.vel.x) * Mathf.Rad2Deg;
        p.tr.localRotation = Quaternion.Euler(0f, 0f, angle);

        p.sr.color = Color.Lerp(p.colorFrom, p.colorTo, t * t); // se mantiene brillante y se apaga al final
    }

    private static float NonZero(float v) => Mathf.Abs(v) < 0.0001f ? 1f : v;
}
