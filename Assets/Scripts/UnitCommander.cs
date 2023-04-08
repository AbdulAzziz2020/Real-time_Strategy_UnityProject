using System;
using UnityEngine;

public class UnitCommander : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public LayerMask layerMask;
    
    // component
    private UnitSelection _unitSelection;
    private Camera _camera;

    private void Awake()
    {
        // get components
        _unitSelection = GetComponent<UnitSelection>();
        _camera = Camera.main;
    }

    private void Update()
    {
        // did we press down out right mouse button and do we have units selected?
        if (Input.GetMouseButtonDown(1) && _unitSelection.HasUnitSelected())
        {
            // shoot a raycast from our mouse, to see what we hit
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // cache the selected units in an array
            Unit[] selectionUnits = _unitSelection.GetSelectionUnits();

            // shoot the raycast
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                _unitSelection.RemoveNullUnitsFromSelection();
                
                // are we clicking on the ground?
                if (hit.collider.CompareTag("Ground"))
                {
                    UnitsMoveToPosition(hit.point, selectionUnits);
                    CreateSelectionMarker(hit.point, false);
                }
                
                // did we click on a resource?
                else if (hit.collider.CompareTag("Resource"))
                {
                    UnitsGatherResource(hit.collider.GetComponent<ResourceSource>(), selectionUnits);
                    CreateSelectionMarker(hit.collider.transform.position, true);
                }
                
                // did we click on an enemy?
                else if (hit.collider.CompareTag("Unit"))
                {
                    Unit enemy = hit.collider.gameObject.GetComponent<Unit>();

                    if (!Player.me.IsMyUnit(enemy))
                    {
                        UnitsAttackEnemy(enemy, selectionUnits);
                        CreateSelectionMarker(enemy.transform.position, false);
                    }
                }
            }
        }
    }

    // called when we command units to move somewhere
    private void UnitsMoveToPosition(Vector3 movePos, Unit[] units)
    {
        Vector3[] destinations = UnitMover.GetUnitGroupDestinations(movePos, units.Length, 2);
        
        for (int i = 0; i < units.Length; i++)
        {
            units[i].MoveToPosition(destinations[i]);
        }
    }

    // called when we command units to gather a resource
    private void UnitsGatherResource(ResourceSource resource, Unit[] units)
    {
        // are just selecting 1 unit?
        if (units.Length == 1)
        {
            units[0].GatherResource(resource, UnitMover.GetUnitDestinationAroundResource(resource.transform.position));
        }
        
        // otherwise, calculate the unit group formation
        else
        {
            Vector3[] destinations =
                UnitMover.GetUnitGroupDestinationsAroundResource(resource.transform.position, units.Length);

            for (int i = 0; i < units.Length; i++)
            {
                units[i].GatherResource(resource, destinations[i]);
            }
        }
    }

    // called when we command units to attack an enemy
    void UnitsAttackEnemy(Unit target, Unit[] units)
    {
        for (int i = 0; i < units.Length; i++)
        {
            units[i].AttackUnit(target);
        }
    }

    private void CreateSelectionMarker(Vector3 pos, bool large)
    {
        GameObject marker = Instantiate(selectionMarkerPrefab, new Vector3(pos.x, 0.01f, pos.z), Quaternion.identity);

        if (large)
            marker.transform.localScale= Vector3.one * 3;
    }
}