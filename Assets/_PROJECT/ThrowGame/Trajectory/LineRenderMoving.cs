using UnityEngine;

public class LineRenderMoving : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Material _material;
    [SerializeField] private float _speed = 1f;

    private Material _runtimeMaterial;

    private void Start()
    {
        _runtimeMaterial = new Material(_material);
        _lineRenderer.material = _runtimeMaterial;
    }

    private void Update()
    {
        Vector2 currentOffset = _runtimeMaterial.mainTextureOffset;
        currentOffset.x -= _speed * Time.deltaTime;
        _runtimeMaterial.mainTextureOffset = currentOffset;
    }

    private void OnDestroy()
    {
        Destroy(_runtimeMaterial);
    }
}