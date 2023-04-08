using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public RectTransform selectionBox;
    public LayerMask unitLayerMask;

    private List<Unit> _selectedUnits = new List<Unit>();

    // components
    private Camera _camera;
    private Player _player;
    private Vector2 _startPos;

    private void Awake()
    {
        // get component
        _camera = Camera.main;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        // On mouse down
        if (Input.GetMouseButtonDown(0))
        {
            ToggleSelectionVisual(false);
            _selectedUnits = new List<Unit>();

            TrySelect(Input.mousePosition);
            _startPos = Input.mousePosition;
        }

        // On mouse up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        // On mouse held down
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    // called when we click on a unit
    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, unitLayerMask))
        {
            Unit unit = hit.collider.GetComponent<Unit>();

            if (_player.IsMyUnit(unit))
            {
                _selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
    }

    // called when we are creating a selection box
    private void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - _startPos.x;
        float height = curMousePos.y - _startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);
    }

    // called where we release the selection box
    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (var unit in _player.units)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x &&
                screenPos.y > min.y && screenPos.y < max.y)
            {
                _selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
    }

    public void RemoveNullUnitsFromSelection()
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            if (_selectedUnits[i] == null)
            {
                _selectedUnits.RemoveAt(i);
            }
        }
    }

    // toggle the selected units selection visual
    private void ToggleSelectionVisual(bool selected)
    {
        foreach (var unit in _selectedUnits)
        {
            unit.ToggleSelectionVisual(selected);
        }
    }

    public bool HasUnitSelected()
    {
        return _selectedUnits.Count > 0 ? true : false;
    }

    public Unit[] GetSelectionUnits()
    {
        return _selectedUnits.ToArray();
    }
}