using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset gunActions;
    private InputAction shootInputAction;

    [SerializeField]
    private Gun gun;

    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private DeathTimer deathTimer;

    void Start()
    {
        shootInputAction = gunActions.FindAction("Gun/Shoot", true);
        shootInputAction.Enable();
        healthSystem.OnDeath += delegate {
            deathTimer.enabled = true;
            Destroy(this);
            Destroy(healthSystem);
            GameManager.Instance.NotifyPlayerDeath();
        };
    }

    void Update()
    {
        if (shootInputAction.WasPressedThisFrame())
        {
            gun.Shoot();
            GameManager.Instance.NotifyPlayerShot();
        }
    }
}
