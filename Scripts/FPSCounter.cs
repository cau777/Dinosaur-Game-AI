using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private int _updateCounter;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(ShowFPS());
    }

    private void Update()
    {
        _updateCounter++;
    }

    private IEnumerator ShowFPS()
    {
        const float waitTime = 0.5f;
        while (true)
        {
            _textMesh.text = $"{Mathf.RoundToInt(_updateCounter / waitTime)} Fps";
            _updateCounter = 0;
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}
