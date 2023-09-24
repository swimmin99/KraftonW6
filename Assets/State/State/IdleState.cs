using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateBase
{
    private const string IdleTrigger = "IsIdle";
    private float _idleTime;
    private float _timer;
    private Transform _target;
    private AnimalAIController _controller;
    private AnimalStats _stats;
    private Animator _animator;
    public IdleState(AnimalAIController controller, AnimalStats animalStats, Animator animator, NavMeshAgent navMeshAgent, Transform transform) : base(controller, animalStats, animator, navMeshAgent)
    {
        CurrentStateName = States.Idle;
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
        if (_target.position.x > 50)
        {
            _target.position = new Vector3(50, _target.position.y, _target.position.z);
            // Turn around
            _target.eulerAngles = new Vector3(0, _target.eulerAngles.y + 180, 0);
        }
        else if (_target.position.x < -50)
        {
            _target.position = new Vector3(-50, _target.position.y, _target.position.z);
            _target.eulerAngles = new Vector3(0, _target.eulerAngles.y + 180, 0);
        }
        if (_target.position.z > 50)
        {
            _target.position = new Vector3(_target.position.x, _target.position.y, 50);
            _target.eulerAngles = new Vector3(0, _target.eulerAngles.y + 180, 0);
        }
        else if (_target.position.z < -50)
        {
            _target.position = new Vector3(_target.position.x, _target.position.y, -50);
            _target.eulerAngles = new Vector3(0, _target.eulerAngles.y + 180, 0);
        }

        _controller._animalStats.energy -= Time.deltaTime * _controller._animalStats.energy_depletion * _controller._animalStats.speed;
    }
}
