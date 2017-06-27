using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{

    public int duration = 10;//10 sec default duration
    public int timeRemaining;
    public bool isCountingDown = false;

    public bool wasActive=false;

    public bool WasActive
    {
        get
        {
            return wasActive;
        }

        set
        {
            wasActive = value;
        }
    }

    public void Begin()
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            WasActive = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
        }
    }

    public void Begin(int seconds)
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            WasActive = true;
            timeRemaining = seconds;
            Invoke("_tick", 1f);
        }
    }

    private void _tick()
    {
        timeRemaining--;
        if (timeRemaining > 0)
        {
            Invoke("_tick", 1f);
        }
        else
        {
            isCountingDown = false;
        }
    }


}