using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] public float energy;
    [SerializeField] public float attackPower;

    protected virtual void Update()
    {
        if (energy <= 0)
        {
            Destroy(gameObject);
        } 
    }
    public virtual void getHurt(float damage)
    {
        energy -= damage;
    }
}
