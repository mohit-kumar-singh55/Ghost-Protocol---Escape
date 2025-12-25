using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private bool _rotateClockwise = true;
    [SerializeField] private float _rotationSpeed = 50f;

    void Update()
    {
        float direction = _rotateClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, direction * _rotationSpeed * Time.deltaTime);
    }
}