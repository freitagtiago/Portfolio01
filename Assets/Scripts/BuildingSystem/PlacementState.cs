using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PlacementState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    int _id;
    Grid _grid;
    PreviewSystem _previewSystem;
    ObjectsDatabaseSO _databaseSO;
    GridData _floorData;
    GridData _furnitureData;
    ObjectPlacer _objectPlacer;
    Vector3 _offset = new Vector3(0f,0.5f,0.5f);

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        _id = iD;
        this._grid = grid;
        this._previewSystem = previewSystem;
        this._databaseSO = database;
        this._floorData = floorData;
        this._furnitureData = furnitureData;
        this._objectPlacer = objectPlacer;

        _selectedObjectIndex = database._objectsData.FindIndex(data => data.ID == _id);
        if (_selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database._objectsData[_selectedObjectIndex].Prefab,
                database._objectsData[_selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"No object with _id {iD}");
        
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Vector3 cellPos = _grid.CellToWorld(gridPosition);
        cellPos += _offset;

        bool isValid = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (isValid == false)
        {
            return;
        }
        int index = _objectPlacer.PlaceObject(_databaseSO._objectsData[_selectedObjectIndex].Prefab,
           cellPos);

        GridData selectedData = _databaseSO._objectsData[_selectedObjectIndex].ID == 0 ?
            _floorData :
            _furnitureData;
        selectedData.AddObjectAt(gridPosition,
            _databaseSO._objectsData[_selectedObjectIndex].Size,
            _databaseSO._objectsData[_selectedObjectIndex].ID,
            index);

        _previewSystem.UpdatePosition(cellPos, false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = _databaseSO._objectsData[selectedObjectIndex].ID == 0 ?
            _floorData :
            _furnitureData;

        return selectedData.CanPlaceObejctAt(gridPosition, _databaseSO._objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        Vector3 cellPos = _grid.CellToWorld(gridPosition);
        cellPos += _offset;

        _previewSystem.UpdatePosition(cellPos, placementValidity);
    }
}

