using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalStats 
{
    [Header("Animal Time Settings")]
    [SerializeField]
    private float _minIdleTime;
    public float speed = 1f;
    public float energy_depletion = 1f;
    public bool chased = false;
    public GameObject predator;
    public float vision = 1f;
    public float reproduction = 1f;

    public float fov = 90f;
    public int rayCount = 20;

    public float energy = 100f;
    public string state = "idle";

    public Vector3 move = Vector3.zero;
    public Vector3 rotate = Vector3.zero;
    public Vector3 direction = Vector3.zero;

    public GameObject target;
    public float time = 0f;


}
