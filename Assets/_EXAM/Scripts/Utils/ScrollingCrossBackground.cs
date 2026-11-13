using UnityEngine;

/// <summary>
/// Fondo infinito con un patrón de cruces generado por código (no necesita ningún asset)
/// que se desplaza de forma suave, estilo juego de móvil.
/// Se coloca detrás de todo y sigue a la cámara.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class ScrollingCrossBackground : MonoBehaviour
{
    [Header("Cámara")]
    [SerializeField] private Camera targetCamera;

    [Header("Colores")]
    [SerializeField] private Color backgroundColor = new Color(0.717f, 0.692f, 0.674f, 1f);
    [SerializeField] private Color crossColor = new Color(0.78f, 0.755f, 0.735f, 1f);

    [Header("Patrón")]
    [Tooltip("Tamaño de una celda del patrón en unidades del mundo (cada celda contiene 2 cruces al tresbolillo).")]
    [SerializeField, Min(0.1f)] private float tileWorldSize = 2f;
    [Tooltip("Resolución en píxeles de la textura generada.")]
    [SerializeField, Range(32, 512)] private int textureSize = 128;
    [Tooltip("Longitud de cada brazo de la cruz, relativo a la celda.")]
    [SerializeField, Range(0.02f, 0.25f)] private float crossArmLength = 0.09f;
    [Tooltip("Grosor de la cruz, relativo a la celda.")]
    [SerializeField, Range(0.005f, 0.1f)] private float crossThickness = 0.028f;
    [Tooltip("Redondeo de las esquinas, relativo a la celda.")]
    [SerializeField, Range(0f, 0.05f)] private float cornerRoundness = 0.012f;
    [Tooltip("Gira las cruces 45º (forma de X en lugar de +).")]
    [SerializeField] private bool rotate45 = false;

    [Header("Movimiento")]
    [SerializeField] private Vector2 scrollDirection = new Vector2(1f, 1f);
    [Tooltip("Velocidad en unidades del mundo por segundo.")]
    [SerializeField, Min(0f)] private float scrollSpeed = 0.35f;

    [Header("Render")]
    [SerializeField] private int sortingOrder = -1000;
    [Tooltip("Opcional: material para el SpriteRenderer (p. ej. Sprite-Unlit-Default para que no le afecten las luces 2D).")]
    [SerializeField] private Material overrideMaterial;

    private SpriteRenderer spriteRenderer;
    private Texture2D texture;
    private Sprite sprite;
    private Vector2 scrollOffset;
    private bool dirty = true;

    private void OnEnable()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        dirty = true;
        Rebuild();
    }

    private void OnDisable()
    {
        ReleaseGenerated();
    }

    private void OnValidate()
    {
        // No se pueden crear texturas de forma segura dentro de OnValidate: se regenera en el próximo Update.
        dirty = true;
    }

    private void Update()
    {
        if (dirty) Rebuild();

        if (Application.isPlaying && scrollSpeed > 0f && scrollDirection.sqrMagnitude > 0.0001f)
        {
            scrollOffset += scrollDirection.normalized * (scrollSpeed * Time.deltaTime);
            scrollOffset.x = Mathf.Repeat(scrollOffset.x, tileWorldSize);
            scrollOffset.y = Mathf.Repeat(scrollOffset.y, tileWorldSize);
        }
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null || targetCamera == null) return;

        // Tamaño visible de la cámara + margen de una celda por cada lado para que nunca se vea el borde.
        float viewH = targetCamera.orthographicSize * 2f;
        float viewW = viewH * targetCamera.aspect;
        Vector2 size = new Vector2(
            (Mathf.Ceil(viewW / tileWorldSize) + 2f) * tileWorldSize,
            (Mathf.Ceil(viewH / tileWorldSize) + 2f) * tileWorldSize);
        if (spriteRenderer.size != size) spriteRenderer.size = size;

        // Anclamos el patrón a la rejilla del mundo y le sumamos el desplazamiento animado,
        // así el bucle es perfecto aunque la cámara se mueva.
        Vector3 camPos = targetCamera.transform.position;
        float baseX = Mathf.Floor(camPos.x / tileWorldSize) * tileWorldSize;
        float baseY = Mathf.Floor(camPos.y / tileWorldSize) * tileWorldSize;
        transform.position = new Vector3(baseX + scrollOffset.x, baseY + scrollOffset.y, transform.position.z);
        transform.rotation = Quaternion.identity;
    }

    private void Rebuild()
    {
        dirty = false;
        if (spriteRenderer == null) return;

        ReleaseGenerated();

        texture = GenerateTexture();
        sprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize / tileWorldSize,
            0,
            SpriteMeshType.FullRect); // FullRect es obligatorio para el modo Tiled
        sprite.name = "CrossPattern (generated)";
        sprite.hideFlags = HideFlags.DontSave;

        spriteRenderer.sprite = sprite;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.tileMode = SpriteTileMode.Continuous;
        spriteRenderer.sortingOrder = sortingOrder;
        if (overrideMaterial != null) spriteRenderer.sharedMaterial = overrideMaterial;
    }

    private Texture2D GenerateTexture()
    {
        var tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true)
        {
            name = "CrossPattern (generated)",
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear,
            anisoLevel = 1,
            hideFlags = HideFlags.DontSave
        };

        var pixels = new Color32[textureSize * textureSize];
        float px = 1f / textureSize; // tamaño de un píxel en espacio UV (celda = 1)

        // Dos cruces por celda, al tresbolillo: (0.25, 0.25) y (0.75, 0.75).
        Vector2 c1 = new Vector2(0.25f, 0.25f);
        Vector2 c2 = new Vector2(0.75f, 0.75f);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 uv = new Vector2((x + 0.5f) * px, (y + 0.5f) * px);
                float d = Mathf.Min(CrossDistance(uv, c1), CrossDistance(uv, c2));

                // Antialiasing: cobertura según la distancia al borde (≈1 píxel de suavizado).
                float coverage = Mathf.Clamp01(0.5f - d / px);
                pixels[y * textureSize + x] = Color.Lerp(backgroundColor, crossColor, coverage);
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply(true, false);
        return tex;
    }

    /// <summary>Distancia con signo a una cruz centrada en <paramref name="center"/>, con repetición (wrap) en UV.</summary>
    private float CrossDistance(Vector2 uv, Vector2 center)
    {
        Vector2 p = uv - center;
        // Wrap para que las cruces que tocan el borde de la celda continúen en la siguiente.
        p.x -= Mathf.Round(p.x);
        p.y -= Mathf.Round(p.y);

        if (rotate45)
        {
            const float s = 0.70710678f;
            p = new Vector2(p.x * s - p.y * s, p.x * s + p.y * s);
        }

        float r = Mathf.Min(cornerRoundness, crossThickness);
        Vector2 armA = new Vector2(crossArmLength - r, crossThickness - r);
        Vector2 armB = new Vector2(crossThickness - r, crossArmLength - r);
        return Mathf.Min(BoxDistance(p, armA), BoxDistance(p, armB)) - r;
    }

    private static float BoxDistance(Vector2 p, Vector2 halfSize)
    {
        Vector2 d = new Vector2(Mathf.Abs(p.x) - halfSize.x, Mathf.Abs(p.y) - halfSize.y);
        Vector2 outside = new Vector2(Mathf.Max(d.x, 0f), Mathf.Max(d.y, 0f));
        return outside.magnitude + Mathf.Min(Mathf.Max(d.x, d.y), 0f);
    }

    private void ReleaseGenerated()
    {
        if (spriteRenderer != null && spriteRenderer.sprite == sprite) spriteRenderer.sprite = null;
        DestroyGenerated(sprite);
        DestroyGenerated(texture);
        sprite = null;
        texture = null;
    }

    private static void DestroyGenerated(Object obj)
    {
        if (obj == null) return;
        if (Application.isPlaying) Destroy(obj);
        else DestroyImmediate(obj);
    }
}
