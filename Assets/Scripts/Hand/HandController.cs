using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using Interactables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Interactables;
using UnityEngine.Experimental.Rendering.Universal;


public class HandController : CollidableBase
{
    public static Action OnLevelWrapping;
    public static Action OnLevelCompleted;
    
    public enum MOVE_TYPE
    {
        CLICK,
        AUTO_MOVE
    }

    //Properties
    //====================================================================================================================//
    public bool Follow { get; set; }

    private MOVE_TYPE _currentMoveType;

    [FormerlySerializedAs("openHand")] [SerializeField]
    private Sprite openHandSprite;

    [SerializeField]
    private Sprite closedHand;

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
    private List<Collider2D> _colliders;
    private Transform _collidersContainerTransform;

    private Transform _heldObject;

    private Camera _camera;
    private float _cameraBlendTime;

    private bool _finished;
    private Coroutine _pauseRoutine;
    private Transform _lastCamera;

    //TODO Might need to make these access set
    private LineRenderer LineRenderer
    {
        get
        {
            if (_lineRenderer == null)
                _lineRenderer = GetComponentInChildren<LineRenderer>();
            return _lineRenderer;
        }
    }
    private LineRenderer _lineRenderer;


    //Unity Functions
    //====================================================================================================================//

    #region Unity Functions

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Collect":
                OnLevelWrapping?.Invoke();
                Follow = false;
                _finished = true;
                GrabObject(other.transform);
                StartCoroutine(PlayInReverseCoroutine());
                break;
            case "Obstacle" when _pushingBack == false:
            case "Wall" when _pushingBack == false:
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
        
        if (!Follow)
            return;
        
        switch (_currentMoveType)
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

    private void OnDisable()
    {
        var cineMachineBrain = _camera.GetComponent<CinemachineBrain>();
        cineMachineBrain.m_CameraActivatedEvent.RemoveListener(OnCameraCut);
    }

    private void OnDestroy()
    {
        if(_collidersContainerTransform)
            Destroy(_collidersContainerTransform.gameObject);
    }

    #endregion //Unity Functions

    //Setup
    //====================================================================================================================//

    public void Setup(in Camera camera, 
        in Vector2 startPosition, 
        in Vector2 startDirection, 
        in float moveSpeed,
        in float rotationSpeed)
    {
        _camera = camera;
        var cineMachineBrain = _camera.GetComponent<CinemachineBrain>();
        cineMachineBrain.m_CameraActivatedEvent.AddListener(OnCameraCut);
        _cameraBlendTime = cineMachineBrain.m_DefaultBlend.m_Time;

        LineRenderer.positionCount = 0;
        _positions = new List<Vector3>();
        _colliders = new List<Collider2D>();
        _collidersContainerTransform = new GameObject("Colliders Container").transform;

        SetColliderActive(true);
        SetSprite(openHandSprite);
        SetColor(GameSettings.PlayerColor);

        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;

        _currentMoveType = GameSettings.MoveType;

        transform.position = startPosition;
        transform.up = startDirection;

        switch (_currentMoveType)
        {
            case MOVE_TYPE.CLICK:
                _lastNode = _currentPos = startPosition;
                CreateNewPathNode(_currentPos);
                break;
            case MOVE_TYPE.AUTO_MOVE:
                _lastNode = _currentPos = startPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //TODO I'll need to have some sort of countdown
        Follow = true;
    }


    
    private void OnCameraCut(ICinemachineCamera to, ICinemachineCamera from)
    {
        if (_finished)
            return;

        _lastCamera = to.VirtualCameraGameObject.transform;
        
        if (_pauseRoutine != null)
        {
            StopCoroutine(_pauseRoutine);
            _pauseRoutine = null;
        }

        if (GameSettings.MoveType == MOVE_TYPE.CLICK)
            return;
        
        _pauseRoutine = StartCoroutine(PauseMovementCoroutine(_cameraBlendTime, 
            () =>
            {
                _pauseRoutine = null;
            }));
        
        //Vector3.pro
    }

    private void GrabObject(in Transform holdTransform)
    {
        SetSprite(closedHand);
        
        _heldObject = holdTransform;
        _heldObject.SetParent(transform, true);
        _heldObject.localPosition = Vector3.zero;

        var rigidBody = _heldObject.GetComponent<Rigidbody2D>();
        if (rigidBody != null)
            rigidBody.isKinematic = true;
    }



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
        
        //The Editor and the build versions have different screen space for some reason
#if UNITY_EDITOR
        
        var MousePos = Input.mousePosition / new Vector2(Screen.width, Screen.height);
        var worldPoint = (Vector2)_camera.ViewportToWorldPoint(MousePos);
        
#else

        var MousePos = Input.mousePosition;
        var worldPoint = (Vector2)_camera.ScreenToWorldPoint(MousePos);

#endif

        
        Debug.DrawLine(transform.position, worldPoint, Color.green);
        
        transform.up = (worldPoint - _lastNode).normalized;
        
        if (!Input.GetKey(KeyCode.Mouse0))
            return;

        var currentPosition = transform.position;
        
        //TODO Should go by distance, not by time i think
        
        if (Vector2.Distance(currentPosition, _lastNode) >= newNodeDistance)
        {
            _lastNode = currentPosition;
            CreateNewPathNode(currentPosition);
        }

        transform.position = Vector2.MoveTowards(currentPosition, worldPoint, Speed * Time.deltaTime);
    }

    private void CreateNewPathNode(in Vector2 pos)
    {
        var circleCollider2D = new GameObject("Arm Collider").AddComponent<CircleCollider2D>();
        circleCollider2D.transform.SetParent(_collidersContainerTransform);
        circleCollider2D.transform.position = pos;
        circleCollider2D.radius = LineRenderer.widthMultiplier / 2;
        _colliders.Add(circleCollider2D);
        
        _positions.Add(pos);
        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }

    #endregion //Movement
    
    //Coroutines
    //====================================================================================================================//

    #region Coroutines

    private IEnumerator PauseMovementCoroutine(float seconds, Action onCompletedCallback)
    {
        //wait a tiny bit before locking movement
        yield return new WaitForSeconds(0.15f);
        
        Follow = false;
        
        yield return new WaitForSeconds(seconds);
        
        Follow = true;
        
        onCompletedCallback?.Invoke();
    }

    private bool _pushingBack;
    private IEnumerator ReverseNodesCoroutine(int count)
    {
        Follow = false;
        _pushingBack = true;
        
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
            
            Destroy(_colliders[_positions.Count - 1].gameObject);
            _colliders.RemoveAt(_positions.Count - 1);
            count--;
        }

        yield return new WaitForSeconds(0.4f);

        Follow = true;
        _pushingBack = false;
    }
    
    private IEnumerator PlayInReverseCoroutine()
    {
        _pushingBack = true;
        yield return new WaitForSeconds(1.5f);
        var speed = 0f;
        
        while (_positions.Count > 1)
        {
            var startPos = _positions[_positions.Count - 1];
            var endPos = _positions[_positions.Count - 2];

            transform.up = (startPos - endPos).normalized;
            
            while (Vector2.Distance(endPos, transform.position) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, endPos, Speed * Time.deltaTime * 3f);
                
                //FIXME If i have time, make this sexy
                if(_lastCamera)
                    _lastCamera.position = new Vector3(transform.position.x,transform.position.y, -5 );
                
                yield return null;
            }

            _lineRenderer.positionCount--;
            _positions.RemoveAt(_positions.Count - 1);
            Destroy(_colliders[_positions.Count - 1].gameObject);
            _colliders.RemoveAt(_positions.Count - 1);
        }

        _lineRenderer.positionCount = 0;
        _pushingBack = false;
        
        OnLevelCompleted?.Invoke();
    }

    #endregion //Coroutines

    //====================================================================================================================//

    protected override void SetColor(in Color color)
    {
        base.SetColor(in color);

        LineRenderer.startColor = LineRenderer.endColor = color;
    }
}
