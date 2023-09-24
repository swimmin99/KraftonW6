using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Simulation : MonoBehaviour
{

    public int start_raptor;
    public int start_food;

    public float food_spawn_rate;
    public float raptor_min_speed;
    public float raptor_max_speed;
    public float raptor_min_attackPower;
    public float raptor_max_attackPower;
    public GameObject Raptor;
    public GameObject Meat;
    // private float time;

    public Transform raptorTransform;
    public Transform foodTransform;

    public TextMeshProUGUI statusUI;

    void Start()
    {
        for (int i = 0; i < start_raptor; i++)
        {
            GameObject raptor = Instantiate(Raptor, new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0), raptorTransform);
            raptor.GetComponent<Chicken>().speed = Random.Range(raptor_min_speed, raptor_max_speed);
            float maxAttackPower = Random.Range(raptor_min_attackPower, raptor_max_attackPower);
            raptor.GetComponent<Chicken>().maxAttackPower = maxAttackPower;
            raptor.GetComponent<Creature>().attackPower = maxAttackPower;
            raptor.GetComponent<Creature>().energy = 60f;
        }
        for (int i = 0; i < start_food; i++)
        {
            GameObject temp = Instantiate(Meat, new Vector3(Random.Range(-150f, 150f), 1, Random.Range(-150f, 150f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0), foodTransform);
            temp.GetComponent<Creature>().energy = 1;
        }
    }

    void Update()
    {
        GameObject[] raptorObjects = GameObject.FindGameObjectsWithTag("Raptor");
        statusUI.text = "Raptor Count: " + raptorObjects.Length;
        if (Random.Range(0f, 1f) < food_spawn_rate)
        {
            Instantiate(Meat, new Vector3(Random.Range(-150f, 150f), 1f, Random.Range(-150f, 150f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0), foodTransform);
        }
    }
}
