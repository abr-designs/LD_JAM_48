using UnityEngine;

public class SetRandomScale : MonoBehaviour
{
    [SerializeField]
    private Vector2 scaleRange = Vector2.one;
    
    // Start is called before the first frame update
    private void Start()
    {
        var scale = Random.Range(scaleRange.x, scaleRange.y);
        transform.localScale = new Vector3(scale, scale, 1f);
        
        GetComponent<Rigidbody2D>().WakeUp();
    }
}
