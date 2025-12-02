using System.Collections;
using UnityEngine;

public class ThunderStrikeObject : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _strikeDuration = 0.3f;
    [SerializeField] private float _lineWidth = 0.15f;
    [SerializeField] private Color _lightningColor = Color.cyan;
    [SerializeField] private int _segments = 12;
    [SerializeField] private float _zigzagAmount = 0.3f;
    [SerializeField] private float _strikeHeight = 5f;

    private void Awake()
    {
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.sortingLayerName = "Default";
        _lineRenderer.sortingOrder = 100;
    }

    public void Strike(Vector3 targetPosition)
    {
        Vector3 startPos = targetPosition + Vector3.up * _strikeHeight;
        Vector3 endPos = targetPosition;

        StartCoroutine(IEStrike(startPos, endPos));
    }

    private IEnumerator IEStrike(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] positions = GenerateLightningPath(startPos, endPos, _segments);
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.SetPositions(positions);

        float blinkInterval = _strikeDuration / 6f;
        
        for (int i = 0; i < 3; i++)
        {
            _lineRenderer.startColor = _lightningColor;
            _lineRenderer.endColor = _lightningColor;
            yield return new WaitForSeconds(blinkInterval);
            
            Color dimColor = _lightningColor * 0.3f;
            _lineRenderer.startColor = dimColor;
            _lineRenderer.endColor = dimColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        Destroy(gameObject);
    }

    private Vector3[] GenerateLightningPath(Vector3 start, Vector3 end, int segments)
    {
        Vector3[] points = new Vector3[segments];
        Vector3 direction = (end - start) / (segments - 1);
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;

        points[0] = start;
        points[segments - 1] = end;

        for (int i = 1; i < segments - 1; i++)
        {
            Vector3 basePoint = start + direction * i;
            float randomOffset = Random.Range(-_zigzagAmount, _zigzagAmount);
            points[i] = basePoint + perpendicular * randomOffset;
        }

        return points;
    }
}