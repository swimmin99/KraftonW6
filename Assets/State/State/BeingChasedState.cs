using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.AI;

public class BeingChasedState : StateBase
{
    private const string IdleTrigger = "IsIdle";
    private float _idleTime;
    private float _timer;
    private Transform _target;
    private AnimalAIController _controller;
    private AnimalStats _stats;
    private Animator _animator;
    public BeingChasedState(AnimalAIController controller, AnimalStats animalStats, Animator animator, NavMeshAgent navMeshAgent, Transform transform) : base(controller, animalStats, animator, navMeshAgent)
    {
        CurrentStateName = States.BeingChased;
        _controller = controller;
        _target = transform;
    }

    public override void Enter()
    {
        base.Enter();
        InitializeState();
    }

    private void InitializeState()
    {
        currentAnimator.SetBool(IdleTrigger, true);
        _timer = 0f;
    }

    public override void Update()
    {
        if (_controller._animalStats.chased)
        {
            _controller._animalStats.chased = false;
            // Move away from predator
            // if predator not destroyed
            if (_controller._animalStats.predator != null)
            {
                _target.LookAt(_target.position + (_target.position - _controller._animalStats.predator.transform.position).normalized);
                _target.position += _target.position + (_target.position - _controller._animalStats.predator.transform.position * Time.deltaTime * _controller._animalStats.speed);
            }
        }

    }
}