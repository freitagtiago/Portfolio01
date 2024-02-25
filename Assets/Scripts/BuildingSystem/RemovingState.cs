using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int _gameObjectIndex = -1;
    Grid _grid;
    PreviewSystem _previewSystem;
    GridData _floorData;
    GridData _furnitureData;
    ObjectPlacer _objectPlacer;

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
        if(_furnitureData.CanPlaceObejctAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = _furnitureData;
        }
        else if(_floorData.CanPlaceObejctAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = _floorData;
        }

        if(selectedData != null)
        {
            _gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (_gameObjectIndex == -1)
                return;
            selectedData.RemoveObjectAt(gridPosition);
            _objectPlacer.RemoveObjectAt(_gameObjectIndex);
        }
        Vector3 cellPosition = _grid.CellToWorld(gridPosition);
        _previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(_furnitureData.CanPlaceObejctAt(gridPosition, Vector2Int.one) &&
            _floorData.CanPlaceObejctAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), validity);
    }
}
