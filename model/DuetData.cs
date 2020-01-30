using System;
using DuetAPI.Machine;

public interface IDuetData
{
}

public class DuetData : IDuetData
{
    public Boolean connecting = true;
    public Single heater1Temp = 0.0f;
    public HeaterState heater1State = 0;
    public Single heater2Temp = 0.0f;
    public HeaterState heater2State = 0;

    public DuetData()
    {
        
    }
}