using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeManager : SingletonBehaviour<DayTimeManager>
{
    public DayTimeConfig dayTime;

    public static Season Season;

    private void Update()
    {
        dayTime = DayTimeConfig.SetTime(
            dayTime.Day
//            + 1
            ,dayTime.Hours
            , dayTime.floatMins
            + Time.deltaTime * 24 / (DayTimeConfig.MaxNight + DayTimeConfig.MaxDay)
            ,out Season);
    }
}