using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollidableBase : Actor2DBase
{
    protected new Collider2D collider
    {
        get
        {
            if (_collider == null)
                _collider = gameObject.GetComponent<Collider2D>();

            return _collider;
        }
    }

    private Collider2D _collider;

    protected void SetColliderActive(in bool state) => collider.enabled = state;
    protected void SetColliderTrigger(in bool isTrigger) => collider.isTrigger = isTrigger;
}
