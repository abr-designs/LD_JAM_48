using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhysicsObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private Vector2 spawnDelayRange;
    private float _spawnDelayTimer;

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
        var temp = Instantiate(prefabSelected, _spawnPosition, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        temp.transform.SetParent(transform, true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
#endif
}
