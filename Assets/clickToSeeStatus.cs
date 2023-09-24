using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class clickToSeeStatus : MonoBehaviour
{
    public GameObject imageObject;
    private Chicken chicken;
    GameObject statusUI;

    private void Start()
    {
        statusUI = GameObject.FindGameObjectWithTag("StatusUI");
        chicken = GetComponent<Chicken>();
    }

    private void OnMouseEnter()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(true);
        }
        if (statusUI != null)
        {
            statusUI.transform.GetComponentInChildren<TextMeshProUGUI>().text =
                chicken.getStateInfo();
        }
    }


    private void OnMouseExit()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
        if (statusUI != null)
        {
            statusUI.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

    }
}