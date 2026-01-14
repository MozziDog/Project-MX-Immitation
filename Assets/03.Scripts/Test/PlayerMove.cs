using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public NavigationManager Nav;
    public float MoveSpeed = 1f;
    public Vector2 Target;
    
    private Vector2 _target;
    private int _curPointIndex;
    private bool _isMoving = false;
    private List<Vector2> _path;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMoving) return;
        
        var targetPoint = _path[_curPointIndex];
        
        transform.position = Vector3.MoveTowards(
            transform.position, 
            new Vector3(targetPoint.x, 0, targetPoint.y), 
            MoveSpeed * Time.deltaTime);
        
        if (((Vector2)transform.position - targetPoint).sqrMagnitude < 0.1f)
        {
            _curPointIndex++;
            if (_curPointIndex == _path.Count)
                _isMoving = false;
        }
    }

    [ContextMenu("Move")]
    public void MoveToTarget()
    {
        _target = Target;
        _path = Nav.GetPath(transform.position, _target);
        _curPointIndex = 0;
        _isMoving = true;
    }
}
