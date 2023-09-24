using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    public ToggleGroup speedToggleGroup;
    private float currentGameSpeed = 1f;
    private float[] speedOptions = { 1f, 2f, 4f, 0f }; 

    private void Start()
    {
        SetGameSpeed(0);
    }

    public void ChangeGameSpeed(int speedIndex)
    {
        if (speedIndex >= 0 && speedIndex < speedOptions.Length)
        {
            SetGameSpeed(speedIndex);
        }
    }

    private void SetGameSpeed(int speedIndex)
    {
        Time.timeScale = speedOptions[speedIndex];
        currentGameSpeed = speedOptions[speedIndex];
    }
}