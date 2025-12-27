using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraController : Singleton<CameraController>
{
    private CinemachineImpulseSource _impulseSource;

    protected override void Awake()
    {
        base.Awake();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // カメラシェイク
    public void ShakeCamera(float force)
    {
        _impulseSource.GenerateImpulse(force);
    }
}