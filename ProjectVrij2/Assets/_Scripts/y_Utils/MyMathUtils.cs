using UnityEditor;
using UnityEngine;

public static class MyMathUtils
{
    // thx, chatgpt
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if(value < fromMin || value > fromMax) { return -1; }
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    public static float RemapClamped(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float temp = (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        if(temp < toMin) { return toMin; }
        else if (temp > toMax) { return toMax; }
        else { return temp; }
    }

    public static float Remap01(float value, float fromMin, float fromMax)
    {
        if (Mathf.Approximately(fromMax, fromMin))
        {
            Debug.LogWarning("Remap01: fromMax and fromMin are equal. Division by zero avoided.");
            return 0f;
        }

        return Mathf.InverseLerp(fromMin, fromMax, value);
    }
}
