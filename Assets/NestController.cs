using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour
{
    [SerializeField] private float hatchTime;
    public int babyCount;
    public GameObject[] parents;
    
    void Start()
    {
        StartCoroutine(nameof(IE_incubating));
    }

    IEnumerator IE_incubating()
    {
        yield return new WaitForSeconds(hatchTime);
        for (int i = 0; i < babyCount; i++)
        {
            GameObject child = Instantiate(parents[0], transform.position, Quaternion.identity);
            child.GetComponent<Creature>().attackPower = Random.Range(parents[0].GetComponent<Creature>().attackPower,
                parents[1].GetComponent<Creature>().attackPower);
        }
        Destroy(gameObject);
    }
}
