using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int _gameObjectIndex = -1;
    private PreviewSystem _previewSystem;
    private Grid _grid;
    private GridData _floorData;
    private GridData _furnitureData;
    private ObjectPlacer _objectPlacer;
    private Vector3 _offset = new Vector3(0f, 0.5f, 0.5f);

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer)
    {
        this._grid = grid;
        this._previewSystem = previewSystem;
        this._floorData = floorData;
        this._furnitureData = furnitureData;
        this._objectPlacer = objectPlacer;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if(_furnitureData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = _furnitureData;
        }
        else if(_floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = _floorData;
        }

        if(selectedData != null)
        {
            _gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (_gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);
            _objectPlacer.RemoveObjectAt(_gameObjectIndex);
        }
        Vector3 cellPosition = _grid.CellToWorld(gridPosition);
        cellPosition += _offset;
        _previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(_furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
            _floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = CheckIfSelectionIsValid(gridPosition);

        Vector3 cellPosition = _grid.CellToWorld(gridPosition);
        cellPosition += _offset;

        _previewSystem.UpdatePosition(cellPosition, isValid);
    }
}
