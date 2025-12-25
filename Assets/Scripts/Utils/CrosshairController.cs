using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : Singleton<CrosshairController>
{
    [SerializeField] private float _speed = 10f;

    private Camera cam;

    public Transform CrosshairTransform => transform;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();      // Get mouse position
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);        // Convert mouse position to world position
        worldPos.z = -1;    // place it in the most front layer
        // Smoothly move the crosshair
        transform.position = Vector3.Lerp(transform.position, worldPos, _speed * Time.deltaTime);
    }
}