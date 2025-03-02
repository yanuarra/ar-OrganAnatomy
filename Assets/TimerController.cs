using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField]
    private double _time = 0;
    private double _maxTime = 0;
    private bool _timerExist = false;
    private bool _timerRunning = false;
    [SerializeField]
    private double hour;
    [SerializeField]
    private double minutes;
    [SerializeField]
    private double seconds;

    public enum TimerType
    {
        Timer,
        Stopwatch
    }
    public TimerType type;  
    
    private void Update()
    {
        if (_timerExist && _timerRunning)
        {
            switch (type)
            {
                case TimerType.Timer:
                    _time -= Time.deltaTime;
                    if (_time <= 0)
                    {
                        StopTime();
                    }
                    break;
                case TimerType.Stopwatch:
                    _time += Time.deltaTime;
                    break;
                default:
                    break;
            }
        }
    }

    public void DeleteTimer()
    {
        if (!_timerExist)
        {
            Debug.Log("Error: There is no timer to delete.");
            return;
        }
        _timerExist = false;
        _timerRunning = false;
        _time = 0;
        Debug.Log("Timer deleted.");
    }

    public void CreateTimer(double maxtime)
    {
        _timerExist = true;
        _timerRunning = true;
        _time = maxtime;
    }

    public void BeginStopwatch()
    {
        _timerExist = true;
        _timerRunning = true;
        _time = 0;
        _maxTime = 0;
    }

    public void StopTime()
    {
        _timerExist = true;
        _timerRunning = false;
        OnElapsedTime();
    }

    private void OnElapsedTime()
    {
        seconds = _time % 60;
        minutes = _time / 60;
        hour = _time / 60 / 60;
    }

    public string GetElapsedTimeAsString()
    {
        StopTime();
        //string minuteText = minutes.ToString("F0");
        //string secondText = seconds.ToString("F0");
        return string.Format("{0:00}:{1:00}", minutes.ToString("00"), seconds.ToString("00")); ;
    }
}
