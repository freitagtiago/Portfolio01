using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputHandlerBuildingSystem _inputManager;
    [SerializeField] private Grid _grid;

    [SerializeField] private ObjectsDatabaseSO _databaseSO;

    [SerializeField] private GameObject _gridVisualization;

    private GridData _floorData, _furnitureData;

    [SerializeField]
    private PreviewSystem _previewSystem;

    private Vector3Int _lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer _objectPlacer;

    IBuildingState _buildingState;

    private void Start()
    {
        _gridVisualization.SetActive(false);
        _floorData = new();
        _furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        _buildingState = new PlacementState(ID,
                                           _grid,
                                           _previewSystem,
                                           _databaseSO,
                                           _floorData,
                                           _furnitureData,
                                           _objectPlacer);
        _inputManager._OnClicked += PlaceStructure;
        _inputManager._OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        _gridVisualization.SetActive(true) ;
        _buildingState = new RemovingState(_grid, _previewSystem, _floorData, _furnitureData, _objectPlacer);
        _inputManager._OnClicked += PlaceStructure;
        _inputManager._OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(_inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        
        _buildingState.OnAction(gridPosition);

    }

    private void StopPlacement()
    {
        if (_buildingState == null)
        {
            return;
        }
        _gridVisualization.SetActive(false);
        _buildingState.EndState();
        _inputManager._OnClicked -= PlaceStructure;
        _inputManager._OnExit -= StopPlacement;
        _lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }

    private void Update()
    {
        if (_buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        if(_lastDetectedPosition != gridPosition)
        {
            _buildingState.UpdateState(gridPosition);
            _lastDetectedPosition = gridPosition;
        }
        
    }
}
