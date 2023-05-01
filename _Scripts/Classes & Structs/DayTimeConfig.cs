using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public struct DayTimeConfig
{
    public float floatMins;
    public int Mins;
    public int Hours;
    public string TimeInString;
    public int Value;

    public int Day;

    public static float MaxDay = 4;
    public static float MaxNight = 6;

    public static int MinsInHour = 60;
    public static int HoursInDayNightCycle = 60;

    public static int DaysInWinter = 92;
    public static int DaysInSpring = 91;
    public static int DaysInSummer = 91;
    public static int DaysInAutumn = 91;

    public static DayTimeConfig SetTime(int _days, int _hours, float _floatMin, out Season season)
    {
        int days = _days;
        int hours = _hours;
        float floatMin = _floatMin;
        int mins = Mathf.CeilToInt(floatMin);

        string minsToString = mins.ToString();
        string hoursToString = hours.ToString();

        if(mins > MinsInHour)
        {
            floatMin = 0;
            mins = 0;
            hours++;
        }
        if(hours > HoursInDayNightCycle)
        {
            hours = 0;
            days++;
        }
        if(days > DaysInWinter + DaysInSpring + DaysInSummer + DaysInAutumn)
        {
            days = 0;
        }

        if (mins < 10)
        {
            minsToString = "0" + minsToString;
        }
        if (hours < 10)
        {
            hoursToString = "0" + hoursToString;
        }
        string stringTime = $"{hoursToString}:{minsToString}";

        //If Hours < MaxDay then that means it is still daytime right???
        stringTime += (Mathf.Min(hours,MaxDay) == hours) ? "AM" : "PM";

        if (days <= DaysInWinter) season = Season.Winter;
        else if (days - DaysInWinter <= DaysInSpring) season = Season.Spring;
        else if (days - DaysInWinter - DaysInSpring <= DaysInSummer) season = Season.Summer;
        else season = Season.Autumn;

        return new DayTimeConfig()
        {
            Day = days,
            floatMins = floatMin,
            Mins = mins,
            Hours = hours,
            TimeInString = stringTime,
            Value = (mins * MinsInHour) + hours
        };
    }
}