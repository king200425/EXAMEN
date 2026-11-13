using System;
using UnityEngine;

public class BorderColliderAdjuster : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D borderCollider;
    [SerializeField]
    private Camera mainCamera;
    void Awake()
    {
        //la orthographic size és el size Y de la camera del centre a la vora superior :^)
        borderCollider.size = new Vector2(
            2.0f * mainCamera.orthographicSize * ((float)Screen.width / (float)Screen.height),
            2.0f * mainCamera.orthographicSize);
    }
}
