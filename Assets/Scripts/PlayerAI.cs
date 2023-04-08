using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    private ResourceSource[] _resources;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        // find all resources in areas
        _resources = FindObjectsOfType<ResourceSource>();
        InvokeRepeating("Check", 0.0f, checkRate);
    }

    private void Check()
    {
        // if we can create a new unit, do so
        if(_player.food >= _player.unitCost)
            _player.CreateNewUnit();
    }
    
    // called when a new unit is created
    public void OnUnitCreated(Unit unit)
    {
        unit.GetComponent<UnitAI>().InitializeAI(this, unit);
    }

    // get the closest resource to the position (random between nearest 3 for some variance)
    public ResourceSource GetclosestResource(Vector3 pos)
    {
        ResourceSource[] closest = new ResourceSource[3];
        float[] closestDist = new float[3];

        foreach (var resource  in _resources)
        {
            if(resource == null) continue;

            float dist = Vector3.Distance(transform.position, resource.transform.position);

            for (int i = 0; i < closest.Length; i++)
            {
                if (closest[i] == null)
                {
                    closest[i] = resource;
                    closestDist[i] = dist;
                    break;
                }
                else if(dist < closestDist[i])
                {
                    closest[i] = resource;
                    closestDist[i] = dist;
                    break;
                }
            }
        }

        return closest[Random.Range(0, closest.Length)];
    }
}
