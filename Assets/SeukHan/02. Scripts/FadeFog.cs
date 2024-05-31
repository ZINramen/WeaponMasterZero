using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class FadeFog : MonoBehaviour
{
    private Volume postProcessVolume;

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
    }

    public void MakeFog(float duration)
    {
        StartCoroutine(FadeFogIn(duration));
    }

    private IEnumerator FadeFogIn(float duration)
    {
        while (postProcessVolume.weight < 1f)
        {
            postProcessVolume.weight += duration;
            yield return new WaitForEndOfFrame();
        }
    }
}
