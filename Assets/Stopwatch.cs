using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    public Text Text;
    public bool play;
    private float Timing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(play == true)
        {
            Timing += Time.deltaTime;
            int min = Mathf.FloorToInt(Timing / 60F);
            int sec = Mathf.FloorToInt(Timing % 60F);
            int millisec = Mathf.FloorToInt((Timing * 100F) % 100F);
            Text.text = "Time: "+ min.ToString("00") + ":" + sec.ToString("00") + ":" + millisec.ToString("00");
        }
    }
}
