using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class DelayState : State
{
    public float waitTime;

    private Coroutine _waitCoroutine;
    
    private void OnEnable()
    {
        _waitCoroutine = StartCoroutine(IEWaitTime());
    }
    
    private void OnDisable()
    {
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }
    }

    private IEnumerator IEWaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        Next();
    }
}
