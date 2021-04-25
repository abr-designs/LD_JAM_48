using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
