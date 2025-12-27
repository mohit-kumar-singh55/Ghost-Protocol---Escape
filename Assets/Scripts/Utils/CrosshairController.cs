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
        Vector2 mousePos = Mouse.current.position.ReadValue();      // マウスの座標を取得
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);        // マウスの座標をワールド座標に変換
        worldPos.z = -1;    // 最前面のレイヤーに配置する
        // クロスヘアを滑らかに移動させる
        transform.position = Vector3.Lerp(transform.position, worldPos, _speed * Time.deltaTime);
    }
}