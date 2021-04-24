using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Interactables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Interactables;


public class HandController : MonoBehaviour
{
    public enum MOVE_TYPE
    {
        CLICK,
        AUTO_MOVE
    }

    //Properties
    //====================================================================================================================//
    
    public MOVE_TYPE currentMoveType = MOVE_TYPE.CLICK;
    
    [SerializeField]
    private float newNodeDistance;

    private float Speed => moveSpeed;
    
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float rotationSpeed;

    private Vector2 _currentPos;
    private Vector2 _lastNode;
    private List<Vector3> _positions;
    
    public bool follow;


    private Camera _camera;
    private LineRenderer _lineRenderer;
    private new Transform transform;


    //Unity Functions
    //====================================================================================================================//

    #region Unity Functions

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
        switch (other.gameObject.tag)
        {
            case "Collect":
                follow = false;
                Destroy(other.gameObject);
                StartCoroutine(PlayInReverseCoroutine());
                break;
            case "Obstacle":
            case "Wall":
                StartCoroutine(ReverseNodesCoroutine(7));
                break;
            case "Interactable":
                other.gameObject.GetComponent<IInteractable>().Interact();
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
        
        if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
        
        if (!follow)
            return;
        
        switch (currentMoveType)
        {
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

    #endregion //Unity Functions

    //Movement
    //====================================================================================================================//

    #region Movement

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

        transform.position = Vector2.MoveTowards(currentPosition, currentPosition + transform.up, Speed * Time.deltaTime);
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

        transform.position = Vector2.MoveTowards(currentPosition, mousePos, Speed * Time.deltaTime);
    }

    private void CreateNewPathNode(in Vector2 pos)
    {
        _positions.Add(pos);
        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }

    #endregion //Movement
    
    //Coroutines
    //====================================================================================================================//

    #region Coroutines

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
        var speed = 0f;
        
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

    #endregion //Coroutines

    //====================================================================================================================//
    
}
