using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class AnimalAIController : MonoBehaviour
{
    //private const float DistanceOffset = 0.2f;

    [Header("Animal Stats")]
    [SerializeField]
    public AnimalStats _animalStats;

    [Header("Animal Cashed Components")]
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private Animator _animalAnimator;

    List<Ray> rays = new List<Ray>();


    public StateBase _currentState;

    // Initialize this animal controller on a moment of its creation
    public void Start()
    {
        _currentState = new IdleState(this, _animalStats, _animalAnimator, _navMeshAgent, transform);
    }

    private void Update()
    {
        _currentState = _currentState.Process();
        rays.Clear();



        for (int i = 0; i < _animalStats.rayCount; i++)
        {
            // Draw rays around the fox's z axis
            float angle = transform.eulerAngles.y - _animalStats.fov / 2 + _animalStats.fov / _animalStats.rayCount * i;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            Ray ray = new Ray(transform.position, direction);
            rays.Add(ray);
            //Debug.DrawRay(ray.origin, ray.direction * vision, Color.red);

        }
    }


   
}
