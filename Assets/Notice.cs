using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notice : MonoBehaviour
{
    public Vector3 targetPosition;
    public CameraController controller;
    public float speed = 5.0f;
    public float distanceOffset = 5f;
    public TextMeshProUGUI textUGUI;
    private bool isMoving = false;
    private float moveStartTime;

    public void onClickNotice()
    {
        if (!isMoving)
        {
            print(targetPosition);
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        Vector3 currentTargetPosition = targetPosition;
        isMoving = true;
        moveStartTime = Time.time;

        while (Vector3.Distance(controller.newPosition, currentTargetPosition) > distanceOffset)
        {
            controller.newPosition = Vector3.MoveTowards(controller.newPosition, currentTargetPosition, speed * Time.deltaTime);

            if (Time.time - moveStartTime >= 10f)
            {
                Debug.LogWarning("MoveToTarget took too long and was canceled.");
                break;
            }

            yield return null;
        }
        isMoving = false;
    }
}