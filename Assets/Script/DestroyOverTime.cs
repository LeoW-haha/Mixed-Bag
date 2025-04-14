using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOverTime : MonoBehaviour
{
    public bool callOnStart;
    public float timeToDestroy = 15;
    void Start()
    {
        if(callOnStart) {
            DestroyIn(timeToDestroy);
        }
    }

    public void DestroyIn(float timeToDestroy) {
        StartCoroutine(CountDown(timeToDestroy));
    }

    IEnumerator CountDown(float value) {
        float normalizedTime = 0;
        while(normalizedTime <= 1) {
            normalizedTime += Time.deltaTime/value;
            yield return null;
        }
        Destroy(gameObject);
    }
}
