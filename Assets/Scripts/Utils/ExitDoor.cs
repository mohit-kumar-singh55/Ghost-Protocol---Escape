using System;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public static event Action OnPlayerWinLevel = delegate { };

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            // ** Player Won: player reached the exit door **
            OnPlayerWinLevel?.Invoke();
        }
    }
}