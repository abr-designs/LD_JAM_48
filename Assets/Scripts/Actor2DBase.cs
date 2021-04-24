using UnityEngine;

public abstract class Actor2DBase : MonoBehaviour
{
    //Properties
    //====================================================================================================================//

    private new SpriteRenderer renderer
    {
        get
        {
            if (_renderer == null)
                _renderer = gameObject.GetComponent<SpriteRenderer>();

            return _renderer;
        }
    }

    private SpriteRenderer _renderer;

    protected new Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = gameObject.transform;

            return _transform;
        }
    }

    private Transform _transform;

    //Functions
    //====================================================================================================================//

    protected void SetSprite(in Sprite sprite) => renderer.sprite = sprite;

    protected virtual void SetColor(in Color color) => renderer.color = color;
}
