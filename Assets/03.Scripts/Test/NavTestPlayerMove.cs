using System.Collections.Generic;
using UnityEngine;

public class NavTestPlayerMove : MonoBehaviour
{
    public NavigationTest Nav;
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
        
        var point = _path[_curPointIndex];
        var targetPoint = new Vector3(point.x, 0f, point.y);
        
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPoint,
            MoveSpeed * Time.deltaTime);
        
        if ((transform.position - targetPoint).sqrMagnitude < 0.1f)
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
        var curPos = new Vector2(transform.position.x, transform.position.z);
        _path = Nav.GetPath(curPos, _target);
        _curPointIndex = 0;
        _isMoving = true;
    }

    private void OnDrawGizmos()
    {
        if (!_isMoving) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < _path.Count - 1; i++)
        {
            var p0 = _path[i];
            var p1 = _path[i + 1];
            Gizmos.DrawLine(new Vector3(p0.x, 0, p0.y), new Vector3(p1.x, 0, p1.y));
        }
    }
}
