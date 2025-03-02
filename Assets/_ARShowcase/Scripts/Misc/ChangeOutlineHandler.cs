using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class ChangeOutlineHandler : MonoBehaviour
{
    public List<Color> colors;  // Array of materials to cycle through
    private Outline rend;
    private int currentIndex = 0;
    private float changeInterval = 0.6f; // Change material every 0.1 seconds
    private float delay = 0.3f; // Delay after changing more than 4 times
    private int changeCount = 0;
    private float timer = 0f;
    public bool isBlinking;

    void Start()
    {
        rend = GetComponent<Outline>();
        colors.Add(new Color(rend.OutlineColor.r, rend.OutlineColor.g, rend.OutlineColor.b, 255f));
        colors.Add(new Color(rend.OutlineColor.r, rend.OutlineColor.g, rend.OutlineColor.b, 0f));

        if (colors.Count > 0)
        {
            rend.OutlineColor = colors[currentIndex];
        }
    }

    void Update()
    {
        //rend.enabled = isBlinking;
        if (!isBlinking) return;
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            // Change to the next material
            currentIndex = (currentIndex + 1) % colors.Count;
            rend.OutlineColor = colors[currentIndex];
            timer = 0f; // Reset the timer

            changeCount++;

            // Check if it's time to add a delay
            if (changeCount > 3)
            {
                Invoke("ApplyDelay", delay);
                changeCount = 0;
            }
        }
    }

    float speed = 0.1f;
    IEnumerator ChangeColourGradually(Color startColor, Color endColor)
    {
        //float tick = 0f; 
        while (rend.OutlineColor != endColor)
        {
            //tick += Time.deltaTime * speed;
            //rend.OutlineColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.deltaTime * speed, 1));
            rend.OutlineColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.deltaTime * speed, 1));
            yield return null;
        }
    }

    void ApplyDelay()
    {
        // Do nothing for 'delay' seconds (0.3 seconds in this case)
    }

    public void OutlineIsBlinking()
    {
        isBlinking = true;
    }

    public void OutlineNotBlinking()
    {
        isBlinking = false;
    }

    public void DefaultOutline()
    {
        isBlinking = false;
        rend.enabled = true;
    }

    public void DisableOutline()
    {
        rend.enabled = false;
    }
}
