using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class MouseHover : MonoBehaviour
{
    [SerializeField]
    private float waitTime;

    private float _timer;

    [SerializeField]
    private Transform growTarget;

    private bool _hovering;

    [SerializeField]
    private HandController handController;

    private Vector2 _handStartPosition, _startPosition;
    
    // Start is called before the first frame update
    private void Start()
    {
        //if (handController.currentMoveType != HandController.MOVE_TYPE.MOUSE)
        Destroy(gameObject);
        
        growTarget.localScale = Vector3.zero;

        _startPosition = transform.position;
        _handStartPosition = handController.transform.position;

    }

    // Update is called once per frame
    private void Update()
    {
        _timer += _hovering ? Time.deltaTime : -Time.deltaTime;

        var t = _timer / waitTime;
        growTarget.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

        handController.transform.position = Vector3.Lerp(_handStartPosition, _startPosition, t);

        if (t < 1f)
            return;

        handController.follow = true;
        Destroy(gameObject);

    }

    private void OnMouseOver()
    {
        _hovering = true;
    }

    private void OnMouseExit()
    {
        _hovering = false;
    }
}
