using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    //====================================================================================================================//
    
    [SerializeField]
    private float rotationSpeed;
    
    private new Transform transform;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
    }

    //====================================================================================================================//
    
}
