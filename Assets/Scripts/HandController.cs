using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HandController : MonoBehaviour
{
    public enum MOVE_TYPE
    {
        MOUSE,
        CLICK,
        AUTO_MOVE
    }

    //[SerializeField]
    public MOVE_TYPE currentMoveType = MOVE_TYPE.CLICK;
    
    [SerializeField]
    private float newNodeDistance;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotationSpeed;

    private Vector2 _currentPos, _previousPos;

    private Vector2 _lastNode;
    private List<Vector3> _positions;
    private LineRenderer _lineRenderer;

    private Camera _camera;
    private new Transform transform;

    public bool follow;

    //====================================================================================================================//
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
        _camera = Camera.main;
        
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _positions = new List<Vector3>();
        
        //Starting Point Values
        //--------------------------------------------------------------------------------------------------------//

        switch (currentMoveType)
        {
            case MOVE_TYPE.MOUSE:
                /*var mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = _lastNode = _currentPos = mousePoint;
                CreateNewPathNode(mousePoint);
                break;*/
            case MOVE_TYPE.CLICK:
                _lastNode = _currentPos = transform.position ;
                CreateNewPathNode(_currentPos);
                follow = true;
                break;
            case MOVE_TYPE.AUTO_MOVE:
                _lastNode = _currentPos = transform.position ;
                follow = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        

        //--------------------------------------------------------------------------------------------------------//
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.name, other.gameObject);

        if (other.gameObject.CompareTag("Collect"))
        {
            follow = false;
            Destroy(other.gameObject);
            StartCoroutine(PlayInReverseCoroutine());
        }
        else
            StartCoroutine(ReverseNodesCoroutine(7));
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);

        if (Input.GetKeyDown(KeyCode.F5))
        {
            currentMoveType = MOVE_TYPE.CLICK;
            SceneManager.LoadScene(0);
        } 
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            currentMoveType = MOVE_TYPE.AUTO_MOVE;
            SceneManager.LoadScene(0);
        } 
        
        if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
        
        if (!follow)
            return;
        
        switch (currentMoveType)
        {
            case MOVE_TYPE.MOUSE:
                MouseMove();
                break;
            case MOVE_TYPE.CLICK:
                ClickMove();
                break;
            case MOVE_TYPE.AUTO_MOVE:
                AutoMove();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //====================================================================================================================//
    
    private void AutoMove()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.up = Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime) *  transform.up;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.up = Quaternion.Euler(0, 0, -rotationSpeed * Time.deltaTime) *  transform.up;
        }
        
        var currentPosition = transform.position;
        
        //TODO Should go by distance, not by time i think
        if (Vector2.Distance(currentPosition, _lastNode) >= newNodeDistance)
        {
            _lastNode = currentPosition;
            CreateNewPathNode(currentPosition);
        }

        transform.position = Vector2.MoveTowards(currentPosition, currentPosition + transform.up, moveSpeed * Time.deltaTime);
    }
    
    private void ClickMove()
    {
        var mousePos = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (mousePos - _lastNode).normalized;
        
        if (!Input.GetKey(KeyCode.Mouse0))
            return;

        var currentPosition = transform.position;
        
        //TODO Should go by distance, not by time i think
        
        if (Vector2.Distance(currentPosition, _lastNode) >= newNodeDistance)
        {
            _lastNode = currentPosition;
            CreateNewPathNode(currentPosition);
        }

        transform.position = Vector2.MoveTowards(currentPosition, mousePos, moveSpeed * Time.deltaTime);
    }

    private void MouseMove()
    {
        Vector2 AverageDirection()
        {
            const int COUNT = 3;
            var index = Mathf.Clamp(_positions.Count - (COUNT + 1), 0, _positions.Count);
            var count = Mathf.Min(COUNT, _positions.Count);
            
            var pos = _positions.GetRange(index, count);

            //var dirs = new Vector2[3];
            var dir = Vector2.zero;
            for (int i = 1; i < count; i++)
            {
                dir += (Vector2)(pos[i] - pos[i - 1]).normalized;
            }

            dir += (_currentPos - _lastNode).normalized;

            return dir / count;
        }

        //--------------------------------------------------------------------------------------------------------//

        //TODO Should go by distance, not by time i think
        _currentPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        
        if (Vector2.Distance(_currentPos, _lastNode) >= newNodeDistance)
        {
            _lastNode = _currentPos;
            CreateNewPathNode(_currentPos);
        }

        transform.position = _currentPos;
        transform.up = AverageDirection();
    }

    private void CreateNewPathNode(in Vector2 pos)
    {
        _positions.Add(pos);
        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }
    
    //====================================================================================================================//

    private IEnumerator ReverseNodesCoroutine(int count)
    {
        follow = false;
        
        yield return new WaitForSeconds(0.1f);
        
        var speed = 0f;
        
        while (count > 0)
        {
            if (_positions.Count - 2 < 0)
                break;
            
            var startPos = _positions[_positions.Count - 1];
            var endPos = _positions[_positions.Count - 2];

            transform.up = (startPos - endPos).normalized;
            
            while (Vector2.Distance(endPos, transform.position) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, speed += Time.deltaTime / 4);

                yield return null;
            }

            _lineRenderer.positionCount--;
            _positions.RemoveAt(_positions.Count - 1);
            count--;
        }

        yield return new WaitForSeconds(0.4f);

        follow = true;
    }
    
    private IEnumerator PlayInReverseCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        var speed = moveSpeed * Time.deltaTime;
        
        while (_positions.Count > 1)
        {
            var startPos = _positions[_positions.Count - 1];
            var endPos = _positions[_positions.Count - 2];

            transform.up = (startPos - endPos).normalized;
            
            while (Vector2.Distance(endPos, transform.position) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, speed += Time.deltaTime);

                yield return null;
            }

            _lineRenderer.positionCount--;
            _positions.RemoveAt(_positions.Count - 1);
        }

        _lineRenderer.positionCount = 0;
    }

}
