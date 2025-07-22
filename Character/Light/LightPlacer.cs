using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPlacer : MonoBehaviour
{
    [SerializeField] int _rayCount = 900;
    [SerializeField] float _viewDistance = 32f;
    [SerializeField] float _pointInterval = 5f;
    [SerializeField] float _checkRadius = 5f;
    LayerMask _lightLayerMask;
    LayerMask _castLayerMask;
    List<Vector2> currentFrameLightPositions = new();
    [SerializeField] int framesBetweenScans = 20;
    private void Awake()
    {
        _castLayerMask = 1 << LayerMask.NameToLayer(GameManager.Layer.Ground.ToString());
        _lightLayerMask = 1 << LayerMask.NameToLayer(GameManager.Layer.Light.ToString());
    }

    int _count = 0;
    void Update()
    {
        if (_count >= framesBetweenScans)
        {
            _count = 0;
            CastView();
        }
        _count++;
    }

    public void CastView()
    {
        currentFrameLightPositions.Clear();

        var origin = transform.position;
        var angleStep = 360f / _rayCount;
        for (int i = 0; i < _rayCount; i++)
        {
            var angle = i * angleStep;
            var direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, _viewDistance, _castLayerMask);
            float rayLength = hit.collider != null ? hit.distance : _viewDistance;

            int stepCount = Mathf.CeilToInt(rayLength / _pointInterval);
            for (int j = 0; j < stepCount; j++)
            {
                var point = (Vector2)origin + j * _pointInterval * direction;
                TryAddLightPosition(point);
            }
        }

        foreach (var point in currentFrameLightPositions)
        {
            var light = ResourceManager.Instance.GetOther(ResourceManager.OtherID.Light.ToString());
            light.transform.position = point;
            var lightComponent = light.GetComponent<ILightHandler>();
            lightComponent.OnPlaced();
        }
    }

    void TryAddLightPosition(Vector2 point)
    {
        if (Physics2D.OverlapCircle(point, _checkRadius, _lightLayerMask))
            return;
        foreach (var existingPoint in currentFrameLightPositions)
        {
            if (Vector2.Distance(existingPoint, point) < _checkRadius)
                return;
        }

        currentFrameLightPositions.Add(point);
    }
}

interface ILightHandler
{
    void OnPlaced();
}
