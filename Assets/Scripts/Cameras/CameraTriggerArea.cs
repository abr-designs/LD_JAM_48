using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera), typeof(BoxCollider2D))]
public class CameraTriggerArea : CollidableBase
{
    public static Action<CinemachineVirtualCamera> PrioritizeNewCamera;

    private CinemachineVirtualCamera VirtualCamera
    {
        get
        {
            if (_virtualCamera == null)
                _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            return _virtualCamera;
        }
    }
    private CinemachineVirtualCamera _virtualCamera;

    //Unity Functions
    //====================================================================================================================//
    
    private void Start()
    {
        SetColliderTrigger(true);
        
        if(collider is BoxCollider2D boxCollider2D)
            boxCollider2D.size = Vector2.one * VirtualCamera.m_Lens.OrthographicSize * 2f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        PrioritizeNewCamera?.Invoke(VirtualCamera);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(collider is BoxCollider2D boxCollider2D)
            boxCollider2D.size = Vector2.one * VirtualCamera.m_Lens.OrthographicSize * 2f;
    }

    private void OnDrawGizmos()
    {
        if (!(collider is BoxCollider2D boxCollider2D))
            return;
        
        Gizmos.color = Color.green;
        
        Gizmos.DrawWireCube(transform.position, boxCollider2D.size);
    }

#endif


    //====================================================================================================================//
    
}
