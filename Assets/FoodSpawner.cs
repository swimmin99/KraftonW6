using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public bool isFoodOn = false;
    public LayerMask groundLayerMask;

    void Update()
    {
        if (isFoodOn && !IsPointerOverUIElement())
        {
            if (Input.GetMouseButtonDown(0)) // Change 0 to 1 for right-click, 2 for middle-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
                {
                    Instantiate(foodPrefab, hit.point, Quaternion.identity);
                }
            }
        }
    }

    public void isFoodOnButton()
    {
        if (isFoodOn) isFoodOn = false;
        else isFoodOn = true;
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}