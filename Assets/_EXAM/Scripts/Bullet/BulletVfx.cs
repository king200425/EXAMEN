using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mini animaciones de disparo generadas 100% por código (sin assets):
///  - MuzzleFlash: fogonazo en la boca del arma.
///  - WallImpact:  impacto contra pared, chispas en abanico de 180º alrededor de la normal.
///  - BodyImpact:  impacto contra un jugador, estallido de 360º con el color del personaje.
/// </summary>
/// 



// EL CODI A CONTINUACIÓ ESTÀ FET AMB CLAUDE CODE OPUS 5.5 HIGH
// No hi ha cap part de l'examen que estigui a codi fet amb IA


public static class BulletVfx
{
    private const int FxSortingOrder = 10;    // por encima de los personajes (0)
    private const int SmokeSortingOrder = 5;

    private static readonly Color White = Color.white;
    private static readonly Color HotYellow = new Color(1f, 0.93f, 0.55f, 1f);
    private static readonly Color Orange = new Color(1f, 0.55f, 0.15f, 1f);

    // ───────────────────────────── MUZZLE FLASH ─────────────────────────────

    public static void MuzzleFlash(Transform muzzle)
    {
        if (muzzle == null) return;

        // Llama: va pegada al arma (hija del spawn point) para que siga el retroceso.
        var flash = VfxBurst.Create("MuzzleFlash", muzzle.position, muzzle.right, muzzle);

        flash.Add(new VfxParticle
        {
            shape = VfxShape.Circle, pos = new Vector2(0.05f, 0f),
            life = 0.07f, sizeFrom = V(0.38f), sizeTo = V(0.08f),
            colorFrom = White, colorTo = Clear(HotYellow)
        });
        flash.Add(new VfxParticle
        {
            shape = VfxShape.Diamond, pos = new Vector2(0.28f, 0f),
            life = 0.08f, sizeFrom = new Vector2(0.62f, 0.24f), sizeTo = new Vector2(0.2f, 0.04f),
            colorFrom = HotYellow, colorTo = Clear(Orange)
        });
        for (int side = -1; side <= 1; side += 2)
        {
            float a = 42f * side;
            flash.Add(new VfxParticle
            {
                shape = VfxShape.Diamond, pos = Dir(a) * 0.13f, rotation = a,
                life = 0.06f, sizeFrom = new Vector2(0.28f, 0.1f), sizeTo = new Vector2(0.06f, 0.02f),
                colorFrom = HotYellow, colorTo = Clear(Orange)
            });
        }

        // Chispas y humo: en espacio mundo, para que se queden atrás cuando el arma retrocede.
        var debris = VfxBurst.Create("MuzzleDebris", muzzle.position, muzzle.right, null);
        int sparks = Random.Range(3, 5);
        for (int i = 0; i < sparks; i++)
        {
            float a = Random.Range(-25f, 25f);
            debris.Add(Spark(a, Random.Range(4f, 7.5f), Random.Range(0.1f, 0.18f), 0.16f, HotYellow, Orange, gravity: 2f));
        }
        for (int i = 0; i < 2; i++)
        {
            debris.Add(new VfxParticle
            {
                shape = VfxShape.Puff, sortingOrder = SmokeSortingOrder,
                pos = new Vector2(0.2f + i * 0.12f, 0f), vel = Dir(Random.Range(-15f, 15f)) * Random.Range(0.5f, 1f),
                drag = 2.5f, gravity = -0.4f, delay = 0.03f + i * 0.02f,
                life = Random.Range(0.35f, 0.5f), sizeFrom = V(0.14f), sizeTo = V(0.42f),
                colorFrom = new Color(0.9f, 0.88f, 0.85f, 0.45f), colorTo = new Color(0.8f, 0.78f, 0.76f, 0f)
            });
        }
    }

    // ───────────────────────────── IMPACTO EN PARED (180º) ─────────────────────────────

    public static void WallImpact(Vector2 point, Vector2 normal)
    {
        if (normal.sqrMagnitude < 0.0001f) normal = Vector2.up;
        var fx = VfxBurst.Create("WallImpact", point, normal, null);

        fx.Add(new VfxParticle
        {
            shape = VfxShape.Circle, life = 0.08f, sizeFrom = V(0.32f), sizeTo = V(0.05f),
            colorFrom = White, colorTo = Clear(HotYellow)
        });
        fx.Add(new VfxParticle
        {
            shape = VfxShape.HalfRing, life = 0.2f, sizeFrom = V(0.15f), sizeTo = V(0.75f),
            colorFrom = new Color(1f, 0.95f, 0.75f, 0.9f), colorTo = Clear(Orange)
        });

        // Chispas repartidas en abanico de -85º a 85º respecto a la normal (180º).
        const int sparkCount = 7;
        for (int i = 0; i < sparkCount; i++)
        {
            float t = (i + 0.5f) / sparkCount;
            float a = Mathf.Lerp(-85f, 85f, t) + Random.Range(-8f, 8f);
            fx.Add(Spark(a, Random.Range(3f, 7f), Random.Range(0.18f, 0.3f), 0.22f, HotYellow, Orange, gravity: 6f));
        }

        // Polvillo de la pared.
        for (int i = 0; i < 3; i++)
        {
            float a = Random.Range(-60f, 60f);
            fx.Add(new VfxParticle
            {
                shape = VfxShape.Puff, sortingOrder = SmokeSortingOrder,
                pos = Dir(a) * 0.05f, vel = Dir(a) * Random.Range(0.6f, 1.3f), drag = 3f,
                life = Random.Range(0.4f, 0.6f), sizeFrom = V(0.12f), sizeTo = V(0.4f),
                colorFrom = new Color(0.55f, 0.52f, 0.5f, 0.5f), colorTo = new Color(0.6f, 0.58f, 0.56f, 0f)
            });
        }
    }

    // ───────────────────────────── IMPACTO EN OPONENTE (360º) ─────────────────────────────

    public static void BodyImpact(Vector2 point, Vector2 bulletDirection, Color bodyColor)
    {
        if (bulletDirection.sqrMagnitude < 0.0001f) bulletDirection = Vector2.right;
        bodyColor.a = 1f;
        Color lightBody = Color.Lerp(bodyColor, White, 0.45f);
        Color darkBody = Color.Lerp(bodyColor, Color.black, 0.25f);

        var fx = VfxBurst.Create("BodyImpact", point, bulletDirection, null);

        fx.Add(new VfxParticle
        {
            shape = VfxShape.Circle, life = 0.1f, sizeFrom = V(0.5f), sizeTo = V(0.1f),
            colorFrom = White, colorTo = Clear(lightBody)
        });
        fx.Add(new VfxParticle
        {
            shape = VfxShape.Ring, life = 0.24f, sizeFrom = V(0.2f), sizeTo = V(1.1f),
            colorFrom = new Color(1f, 1f, 1f, 0.95f), colorTo = Clear(bodyColor)
        });

        // Chispas en 360º, repartidas uniformemente con algo de variación.
        const int sparkCount = 10;
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < sparkCount; i++)
        {
            float a = offset + i * (360f / sparkCount) + Random.Range(-12f, 12f);
            fx.Add(Spark(a, Random.Range(3f, 6.5f), Random.Range(0.18f, 0.28f), 0.24f, lightBody, bodyColor, gravity: 3f));
        }

        // Trocitos que salen disparados y caen.
        for (int i = 0; i < 4; i++)
        {
            float a = Random.Range(0f, 360f);
            fx.Add(new VfxParticle
            {
                shape = VfxShape.Circle, vel = Dir(a) * Random.Range(1.5f, 3.5f), drag = 1.5f, gravity = 9f,
                life = Random.Range(0.3f, 0.45f), sizeFrom = V(Random.Range(0.08f, 0.14f)), sizeTo = V(0f),
                colorFrom = darkBody, colorTo = darkBody
            });
        }
    }

    // ───────────────────────────── helpers ─────────────────────────────

    private static VfxParticle Spark(float angle, float speed, float life, float length, Color from, Color to, float gravity)
    {
        return new VfxParticle
        {
            shape = VfxShape.Diamond, vel = Dir(angle) * speed, drag = 6f, gravity = gravity,
            alignToVelocity = true, life = life,
            sizeFrom = new Vector2(length, length * 0.25f), sizeTo = new Vector2(0.03f, 0.03f),
            colorFrom = from, colorTo = Clear(to)
        };
    }

    private static Vector2 Dir(float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(r), Mathf.Sin(r));
    }

    private static Vector2 V(float s) => new Vector2(s, s);
    private static Color Clear(Color c) { c.a = 0f; return c; }
}

public enum VfxShape { Circle, Ring, HalfRing, Diamond, Puff }

/// <summary>Una partícula de un efecto. Posición, velocidad y ángulo en el espacio local del efecto (+X = dirección principal).</summary>
public class VfxParticle
{
    public VfxShape shape = VfxShape.Circle;
    public Vector2 pos;
    public Vector2 vel;
    public float rotation;            // grados (si no se alinea con la velocidad)
    public bool alignToVelocity;
    public float drag;                // frenado exponencial
    public float gravity;             // unidades/s² hacia abajo en el mundo
    public float delay;
    public float life = 0.2f;
    public Vector2 sizeFrom = Vector2.one;
    public Vector2 sizeTo = Vector2.zero;
    public Color colorFrom = Color.white;
    public Color colorTo = new Color(1f, 1f, 1f, 0f);
    public int sortingOrder = 10;

    internal float age;
    internal Transform tr;
    internal SpriteRenderer sr;
}

/// <summary>Genera (una sola vez) los sprites blancos que usan los efectos. Cada sprite mide 1x1 unidades.</summary>
public static class VfxSprites
{
    private const int Size = 64;
    private static readonly Dictionary<VfxShape, Sprite> cache = new Dictionary<VfxShape, Sprite>();

    public static Sprite Get(VfxShape shape)
    {
        if (cache.TryGetValue(shape, out var s) && s != null) return s;
        s = Build(shape);
        cache[shape] = s;
        return s;
    }

    private static Sprite Build(VfxShape shape)
    {
        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
        {
            name = "Vfx_" + shape,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.DontSave
        };

        var pixels = new Color32[Size * Size];
        float px = 2f / Size; // tamaño de un píxel en coordenadas [-1, 1]
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Vector2 p = new Vector2((x + 0.5f) * px - 1f, (y + 0.5f) * px - 1f);
                float a = Alpha(shape, p, px);
                pixels[y * Size + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply(false, true);

        var sprite = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size, 0, SpriteMeshType.FullRect);
        sprite.name = tex.name;
        sprite.hideFlags = HideFlags.DontSave;
        return sprite;
    }

    private static float Alpha(VfxShape shape, Vector2 p, float px)
    {
        float r = p.magnitude;
        float d; // distancia con signo al borde (negativa = dentro)
        switch (shape)
        {
            case VfxShape.Circle:
                d = r - 0.95f;
                break;
            case VfxShape.Ring:
                d = Mathf.Abs(r - 0.82f) - 0.12f;
                break;
            case VfxShape.HalfRing:
                // Semianillo abierto hacia +X (la normal de la pared).
                d = Mathf.Max(Mathf.Abs(r - 0.82f) - 0.12f, -p.x);
                break;
            case VfxShape.Diamond:
                d = (Mathf.Abs(p.x) + Mathf.Abs(p.y) - 0.95f) * 0.7071f;
                break;
            case VfxShape.Puff:
                // Círculo de borde suave.
                float k = Mathf.Clamp01((r - 0.35f) / 0.6f);
                return 1f - k * k * (3f - 2f * k);
            default:
                d = 1f;
                break;
        }
        return Mathf.Clamp01(0.5f - d / px);
    }
}
