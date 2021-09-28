using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{

    public static Action onMinuteChanged;
    public static Action onHourChange;

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }

    private float minuteToRealTime = 0.05f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        Minute = 0;
        Hour = 10;
        timer = minuteToRealTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;
            onMinuteChanged?.Invoke();
            Debug.Log("Worked2");
            
            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;
              onHourChange?.Invoke();
            }
            timer = minuteToRealTime;
        }
        Debug.Log("Worked4");
        Debug.Log(Hour + ":" + Minute);

    }
}
