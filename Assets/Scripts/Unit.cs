using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum UnitState
{
    Idle,
    Move,
    MoveToResource,
    Gather,
    MoveToEnemy,
    Attack
}

public class Unit : MonoBehaviour
{
    [Header("State")] public UnitState state;

    public int curHp;
    public int maxHp;

    public int minAttackDamage;
    public int maxAttackDamage;

    public float attackRate;
    private float _lastAttackTime;

    public float attackDistance;

    public float pathUpdateRate = 1.0f;
    private float _lastPathUpdateTime;

    public int gatherAmount;
    public float gatherRate;
    private float _lastGatherTime;

    public ResourceSource curResourceSource;
    private Unit _curEnemyTarget;
    
    [Header("Components")]
    public GameObject selectedVisual;
    private NavMeshAgent _navMeshAgent;
    public UnitHealthBar healthBar;

    public Player player;
    
    // Events
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<UnitState> {}
    public StateChangeEvent onStateChange;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        SetState(UnitState.Idle);
    }

    void SetState(UnitState toState)
    {
        state = toState;
        
        // calling the event
        onStateChange?.Invoke(state);

        if (toState == UnitState.Idle)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
    }

    private void Update()
    {
        switch (state)
        {
            case UnitState.Move:
            {
                MoveUpdate();
                break;
            }
            case UnitState.MoveToResource:
            {
                MoveToResourceUpdate();
                break;
            }
            case UnitState.Gather:
            {
                GatherUpdate();
                break;
            }
            case UnitState.MoveToEnemy:
            {
                MoveToEnemyUpdate();
                break;
            }
            case UnitState.Attack:
            {
                AttackUpdate();
                break;
            }
        }
    }

    // called every frame the 'Move' state is active
    void MoveUpdate()
    {
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) == 0.0f)
            SetState(UnitState.Idle);    
    }

    // called every frame the 'MoveToResource' state is active
    void MoveToResourceUpdate()
    {
        if (curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if (Vector3.Distance(transform.position, _navMeshAgent.destination) == 0.0f)
            SetState(UnitState.Gather);
    }

    // called every frame the 'Gather' state is active
    void GatherUpdate()
    {
        if (curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }
        
        LookAt(curResourceSource.transform.position);

        if (Time.time - _lastGatherTime > gatherRate)
        {
            _lastGatherTime = Time.time;
            curResourceSource.GatherResource(gatherAmount, player);
        }
    }

    // called every frame the 'MoveToEnemy' state is active
    void MoveToEnemyUpdate()
    {
        // if our target is dead, go idle
        if (_curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if (Time.time - _lastPathUpdateTime > pathUpdateRate)
        {
            _lastPathUpdateTime = Time.time;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_curEnemyTarget.transform.position);
        }

        if (Vector3.Distance(transform.position, _curEnemyTarget.transform.position) <= attackDistance)
        {
            SetState(UnitState.Attack);
        }
    }

    // called every frame the 'Attack' state is active
    void AttackUpdate()
    {
        // if our target is dead, go idle
        if (_curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        // if we're still moving, stop
        if (!_navMeshAgent.isStopped) _navMeshAgent.isStopped = true;

        // attack every 'attackRate' seconds
        if (Time.time - _lastAttackTime > attackRate)
        {
            _lastAttackTime = Time.time;
            _curEnemyTarget.TakeDamage(Random.Range(minAttackDamage, maxAttackDamage + 1));
        }
        
        // look at the enemy
        LookAt(_curEnemyTarget.transform.position);
        
        // if we're too far away, move toward the enemy
        if(Vector3.Distance(transform.position, _curEnemyTarget.transform.position) > attackDistance)
            SetState(UnitState.MoveToEnemy);
    }

    public void TakeDamage(int damage)
    {
        curHp -= damage;

        if (curHp <= 0) Die();
        
        // update the health bar
        healthBar.UpdateHealthBar(curHp, maxHp);
    }

    // called when our health reaches 0
    void Die()
    {
        player.units.Remove(this);
        
        GameManager.instance.UnitDeathCheck();
        
        Destroy(gameObject);
    }

    // moves the unit to a specific position
    public void MoveToPosition(Vector3 pos)
    {
        SetState(UnitState.Move);
        
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(pos);
    }

    // move to a resource and begin to gather it
    public void GatherResource(ResourceSource resource, Vector3 pos)
    {
        curResourceSource = resource;
        SetState(UnitState.MoveToResource);

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(pos);
    }

    // move to an enemy unit and attack them
    public void AttackUnit(Unit target)
    {
        _curEnemyTarget = target;
        SetState(UnitState.MoveToEnemy);
    }

    // toggle the selection ring around our feet
    public void ToggleSelectionVisual(bool selected)
    {
        if(selectedVisual != null) selectedVisual.SetActive(selected);
    }

    //  rotate to face the given position
    void LookAt(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}