using UnityEngine;

namespace Obstacles
{
    public class ObstacleOnTrack : MonoBehaviour
    {
        [SerializeField]
        private Vector2[] localTackPositions;

        private Vector2[] _worldTrackPositions;

        [SerializeField]
        private bool moveClockWise;

        [SerializeField]
        private float startDelay;

        [SerializeField]
        private float arrivalWaitTime;

        [SerializeField]
        private float moveSpeed;
    
        [SerializeField]
        private int startIndex;

        private int _currentIndex;
        private int _targetIndex;

        private new Transform transform;

        private float _waitTime;
        private float _waitTimer;
        private bool _waiting;
    
        //Unity Functions
        //====================================================================================================================//
    
        // Start is called before the first frame update
        private void Start()
        {
            transform = gameObject.transform;

            _currentIndex = startIndex;
            _targetIndex = GetIndexInDirection(moveClockWise ? 1 : -1);

            _worldTrackPositions = new Vector2[localTackPositions.Length];
            for (int i = 0; i < localTackPositions.Length; i++)
            {
                _worldTrackPositions[i] = (Vector2)transform.position + localTackPositions[i];
            }

            if (startDelay > 0)
            {
                _waiting = true;
                _waitTime = startDelay;
            }

            transform.position = _worldTrackPositions[_currentIndex];
        }

        // Update is called once per frame
        private void Update()
        {

            //--------------------------------------------------------------------------------------------------------//
        
            if (_waiting)
            {
                if (_waitTimer >= _waitTime)
                {
                    _waitTimer = 0f;
                    _waiting = false;
                    return;
                }

                _waitTimer += Time.deltaTime;
                return;
            }

            //--------------------------------------------------------------------------------------------------------//
        
            var currentPosition = (Vector2)transform.position;
            var targetPosition = _worldTrackPositions[_targetIndex];
        
            if (Vector2.Distance(targetPosition, currentPosition) <= 0.1f)
            {
                transform.position = targetPosition;
                _currentIndex = _targetIndex;
                _targetIndex = GetIndexInDirection(moveClockWise ? 1 : -1);

                if (arrivalWaitTime > 0f)
                {
                    _waiting = true;
                    _waitTime = arrivalWaitTime;
                }
            
                return;
            }

            //--------------------------------------------------------------------------------------------------------//
        
            Debug.DrawLine(transform.position, targetPosition, Color.red);
        
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
        }


        //====================================================================================================================//

        private int GetIndexInDirection(in int direction)
        {
            var target = _currentIndex + direction;

            if (target >= localTackPositions.Length)
                target = 0;
            else if (target < 0)
                target = localTackPositions.Length - 1;

            return target;
        }

        //Unity Editor
        //====================================================================================================================//

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {

            var currentPos = (Vector2)gameObject.transform.position;

            if (_worldTrackPositions != null)
            {
                Gizmos.color = Color.cyan;

                for (int i = 1; i <= _worldTrackPositions.Length; i++)
                {
                    if (i == _worldTrackPositions.Length)
                    {
                        Gizmos.DrawLine(_worldTrackPositions[0], _worldTrackPositions[i - 1]);
                        break;
                    }
            
                    Gizmos.DrawLine(_worldTrackPositions[i], _worldTrackPositions[i - 1]);
                }
            }
            else
            {
                Gizmos.color = Color.blue;

                for (int i = 1; i <= localTackPositions.Length; i++)
                {
                    if (i == localTackPositions.Length)
                    {
                        Gizmos.DrawLine(currentPos + localTackPositions[0], currentPos + localTackPositions[i - 1]);
                        break;
                    }
            
                    Gizmos.DrawLine(currentPos + localTackPositions[i], currentPos + localTackPositions[i - 1]);
                }
            }
        
        }

#endif
    
    }
}
