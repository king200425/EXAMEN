using UnityEngine;

public class RotationLocker : MonoBehaviour
{
    [SerializeField]
    private float targetRotation;
    void LateUpdate()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, targetRotation);
    }
}
