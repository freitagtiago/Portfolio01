using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float _smoothSpeed = 8f;
    public Vector3 _offset;

    [SerializeField] private float _maxZoomValue = 8f;
    [SerializeField] private float _minZoomValue = 2f;
    [SerializeField] private float _zoomValue = 5f;
    private float _zoomMultiplier = 4f;
    private float _velocity = 0f;
    private float _zoomSmoothSpeed = 0.25f;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _zoomValue = _camera.orthographicSize;
    }

    void Update()
    {
        ResolveZoom();

        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x + _offset.x, target.position.y + _offset.y, target.position.z + _offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void ResolveZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        _zoomValue -= scroll * _zoomMultiplier;
        _zoomValue = Mathf.Clamp(_zoomValue, _minZoomValue, _maxZoomValue);
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _zoomValue, ref _velocity, _zoomSmoothSpeed);
    }

   
}