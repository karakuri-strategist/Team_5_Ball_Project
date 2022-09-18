using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public Color highColor;
    public Color lowColor;
    public Color veryLowColor;
    public float lowTime = 120f;
    public float veryLowTime = 60f;
    public float timeAllowed = 300f;
    private float timeLeft;
    public ResetHandler resetHandler;
    private TMPro.TextMeshProUGUI text;
    
    private void OutOfTime()
    {
        resetHandler.Reset();
    }

    public void OnReset(object sender)
    {
        timeLeft = timeAllowed;
    }

    void Start()
    {
        timeLeft = timeAllowed;
        text = GetComponent<TMPro.TextMeshProUGUI>();
        resetHandler.ResetEvent += OnReset;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss\:ff");
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
        {
            timeLeft = timeAllowed;
            OutOfTime();
        }
    }
}
