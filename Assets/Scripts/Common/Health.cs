using UnityEngine;

public abstract class Health : MonoBehaviour
{
    #region Serialized Fields
    [Tooltip("Maximum health")]
    [SerializeField] protected int _maxHealth = 5;
    #endregion

    #region Properties
    protected int _currentHealth;
    #endregion

    protected virtual void Start()
    {
        // initial health
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(int damage = 1)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - damage);
    }
}