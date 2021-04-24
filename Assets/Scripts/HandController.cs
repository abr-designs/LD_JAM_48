using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private float newNodeTime;

    private float _newNodeTimer;
    
    private Vector2 _currentPos, _previousPos;

    private List<Vector3> _positions;
    private LineRenderer _lineRenderer;

    private Camera _camera;
    private new Transform transform;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
        _camera = Camera.main;
        
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _positions = new List<Vector3>();
    }

    // Update is called once per frame
    private void Update()
    {
        //TODO Should go by distance, not by time i think
        _previousPos = _currentPos;
        _currentPos = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (_newNodeTimer >= newNodeTime)
        {
            CreateNewPathNode(_currentPos);
            _newNodeTimer = 0f;
        }
        else
            _newNodeTimer += Time.deltaTime;

        transform.position = _currentPos;
        transform.up = (_currentPos - _previousPos).normalized;
    }

    //====================================================================================================================//

    private void CreateNewPathNode(in Vector2 pos)
    {
        _positions.Add(pos);
        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }
    
    //====================================================================================================================//
    
    
}
