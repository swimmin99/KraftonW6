using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMonitor : MonoBehaviour
{
    // Variable to keep track of the active display
    private int activeDisplayIndex = 0; // Start with the primary display (0)

    public void onClickChangeMonitor()
    {
        // Check the number of monitors connected.
        if (Display.displays.Length > 1)
        {
            // Determine the index of the currently active display
            for (int i = 0; i < Display.displays.Length; i++)
            {
                if (Display.displays[i].active)
                {
                    activeDisplayIndex = i;
                    break;
                }
            }

            // Activate the other display
            int targetDisplayIndex = 1 - activeDisplayIndex; // Toggle between 0 and 1
            Display.displays[targetDisplayIndex].Activate();
        }
    }
}