using UnityEngine;

public class LoopingMovement : MonoBehaviour
{
    [SerializeField] private Transform _entity;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _finishPoint;
    [SerializeField][Min(0)] private float _speed = 5;

    private const float _minSqrMagnitude = 1;

    private void Update()
    {
        Vector3 direction = _finishPoint.position - _entity.position;
        Vector3 step = _speed * Time.deltaTime * direction.normalized;

        _entity.position += step;

        if (direction.sqrMagnitude <= _minSqrMagnitude)
            _entity.position = _startPoint.position;
    }
}