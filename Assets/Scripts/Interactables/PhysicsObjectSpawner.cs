using UnityEngine;
using Random = UnityEngine.Random;

public class PhysicsObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private float spawnRadius = 1f;

    [SerializeField]
    private Vector2 spawnDelayRange;
    private float _spawnDelayTimer;
    
    [SerializeField]
    private Vector2 spawnScaleRange = Vector2.one;

    [SerializeField, Space(10f)]
    private bool destroyAfterTime;
    [SerializeField]
    private Vector2 destroyTimeRange = Vector2.zero;

    private Vector2 _spawnPosition;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        _spawnPosition = gameObject.transform.position;

        _spawnDelayTimer = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_spawnDelayTimer > 0f)
        {
            _spawnDelayTimer -= Time.deltaTime;
            return;
        }
        
        _spawnDelayTimer = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
        SpawnPrefab();
    }

    //====================================================================================================================//

    private void SpawnPrefab()
    {
        var prefabSelected = prefabs[Random.Range(0, prefabs.Length)];
        var spawnPosition = _spawnPosition + (Random.insideUnitCircle * spawnRadius);
        var temp = Instantiate(prefabSelected, spawnPosition, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        temp.transform.localScale = Vector3.one * Random.Range(spawnScaleRange.x, spawnScaleRange.y); 
        temp.transform.SetParent(transform, true);

        if (temp.GetComponent<Rigidbody2D>() is Rigidbody2D rigidbody2D && rigidbody2D != null) 
            rigidbody2D.WakeUp();

        if (!destroyAfterTime)
            return;
        
        Destroy(temp, Random.Range(destroyTimeRange.x, destroyTimeRange.y));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}
