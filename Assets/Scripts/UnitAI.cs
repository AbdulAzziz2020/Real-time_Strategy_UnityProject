using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    public float nearbyEnemyAttackRange;
    public LayerMask unitLayerMask;

    private PlayerAI _playerAI;
    private Unit _unit;

    public void InitializeAI(PlayerAI playerAI, Unit unit)
    {
        this._playerAI = playerAI;
        this._unit = unit;
    }

    private void Start()
    {
        InvokeRepeating("Check", 0.0f, checkRate);
    }

    private void Check()
    {
        // check if we have nearby enemy - if so, attack them
        if (_unit.state != UnitState.Attack && _unit.state != UnitState.MoveToEnemy)
        {
            Unit potentialEnemy = CheckForNearbyEnemies();
            
            if(potentialEnemy != null) _unit.AttackUnit(potentialEnemy);
        }

        // if we're doing nothing, find a new resource
        if (_unit.state == UnitState.Idle) FindNewResource();
        
        // if we're moving to a resource which is destroyed, find a new one
        else if (_unit.state == UnitState.MoveToResource && _unit.curResourceSource == null) FindNewResource();
    }

    void FindNewResource()
    {
        ResourceSource resourceToGet = _playerAI.GetclosestResource(transform.position);

        if (resourceToGet != null)
            _unit.GatherResource(resourceToGet,
                UnitMover.GetUnitDestinationAroundResource(resourceToGet.transform.position));
        else
            PursueEnemy();
    }

    Unit CheckForNearbyEnemies()
    {
        RaycastHit[] hits =
            Physics.SphereCastAll(transform.position, nearbyEnemyAttackRange, Vector3.up, unitLayerMask);

        GameObject closest = null;
        float closestDist = 0.0f;

        for (int i = 0; i < hits.Length; i++)
        {
            // skip if this is us
            if (hits[i].collider.gameObject == gameObject) continue;

            // is this a teammate?
            if (_unit.player.IsMyUnit(hits[i].collider.GetComponent<Unit>())) continue;

            if (!closest || Vector3.Distance(transform.position, hits[i].transform.position) < closestDist)
            {
                closest = hits[i].collider.gameObject;
                closestDist = Vector3.Distance(transform.position, hits[i].transform.position);
            }
        }

        if (closest != null) return closest.GetComponent<Unit>();
        else return null;
    }

    void PursueEnemy()
    {
        Player enemyPlayer = GameManager.instance.GetRandomPlayer(_unit.player);
        
        if(enemyPlayer.units.Count > 0)
            _unit.AttackUnit(enemyPlayer.units[Random.Range(0, enemyPlayer.units.Count)]);
    }
}