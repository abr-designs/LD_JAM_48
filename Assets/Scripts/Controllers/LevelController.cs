using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static Action OnLevelReset;
    
    [SerializeField]
    private string name = "Level";

    [SerializeField]
    private float cameraSize = 5f;
    [SerializeField]
    private float handSpeed = 3.5f;
    [SerializeField]
    private float rotationSpeed = 150;

    [SerializeField]
    private Transform startOrientationTransform;

    //[SerializeField]
    private Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = FindObjectOfType<Camera>();

            return _camera;
        }
    }
    private Camera _camera;

    //Unity Functions
    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        FindObjectOfType<HandController>().Setup(Camera,
            startOrientationTransform.position, 
            startOrientationTransform.up,
            handSpeed,
            rotationSpeed);
    }

    //Unity Editor
    //====================================================================================================================//
    
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (startOrientationTransform == null)
            return;

        var position = startOrientationTransform.position; 
        Gizmos.color = Color.blue;
        
        Gizmos.DrawWireSphere(position, 1f);
        Gizmos.color = Color.white;

        Gizmos.DrawLine(position, position + startOrientationTransform.up * 1.25f);
    }

#endif
}
