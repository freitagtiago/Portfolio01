using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandlerBuildingSystem: MonoBehaviour
{
    [SerializeField]
    private Camera _sceneCamera;

    private Vector3 _lastPosition;

    [SerializeField]
    private LayerMask _placementLayermask;

    public event Action _OnClicked, _OnExit;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _OnClicked?.Invoke();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = new Vector3(Mouse.current.position.x.ReadValue()
            , Mouse.current.position.y.ReadValue()
            , _sceneCamera.nearClipPlane);

        Ray ray = _sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, 10000, _placementLayermask))
        {
            _lastPosition = hit.point;
        }

        return _lastPosition;
    }
}
