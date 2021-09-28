using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class TimeManager : MonoBehaviour
{
    PhotonView view;

    public static Action onMinuteChanged;
    public static Action onHourChange;
    public static Action onDayChange;

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }
    
    public static int Day { get; private set; }

    private float minuteToRealTime = 0.05f;
    private float timer;
    

    // Start is called before the first frame update
    void Start()
    {
        Minute = 0;
        Hour = 10;
        timer = minuteToRealTime;
        view = GetComponent<PhotonView>();
    }


    [PunRPC]
    private void updateTime(int hour, int minute)
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            minute++;
            Minute = minute;
            onMinuteChanged?.Invoke();
            Debug.Log("Worked2");
            
            if (minute >= 60)
            {
                hour++;
                Hour = hour;
                minute = 0;
                Minute = 0;
                onHourChange?.Invoke();

                if (hour > 24)
                {
                    Day++;
                    hour = 0;
                    Hour = 0;
                    onDayChange?.Invoke();
                }

            }
            timer = minuteToRealTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("updateTime", RpcTarget.All, Hour, Minute);
        }
    }
    
}
