using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class DelayState : State
{
    public float waitTime;

    private void Start()
    {
        StartCoroutine(IEWaitTime());
    }

    private IEnumerator IEWaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        Next();
    }
}
