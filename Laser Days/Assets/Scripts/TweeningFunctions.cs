﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TweeningFunctions {

    public enum TweenType { Linear, EaseIn, EaseOut, EaseInOut, EaseInCubic, EaseOutCubic, BackAndForth, FiveStep };

    public static float Tween(TweenType type, float R)
    {
        switch (type)

        {
            case TweenType.Linear:
                return Linear(R);
                break;

            case TweenType.EaseIn:
                return EaseIn(R);
                break;


            case TweenType.EaseOut:
                return EaseOut(R);
                break;


            case TweenType.EaseInOut:
                return EaseInOut(R);
                break;


            case TweenType.EaseInCubic:
                return EaseInCubic(R);
                break;


            case TweenType.EaseOutCubic:
                return EaseOutCubic(R);
                break;

            case TweenType.BackAndForth:
                return BackAndForth(R);
                break;

            case TweenType.FiveStep:
                return FiveStep(R);
                break;
              
            default :
                return R;
                break;
        }
    }

    public static float Linear(float R)
    {
        return R;
    }

    //Quad ease in
    public static float EaseIn(float R)
    {
        var r = Mathf.Clamp01(R);
        return r * r;
    }

    //Quad ease out
    public static float EaseOut(float R)
    {
        var r = Mathf.Clamp01(R);
        var s = 1f - r;
        var s2 = s * s;
        return 1f - s2;
    }

    //Quad ease in and out
    public static float EaseInOut(float R)
    {
        var r = Mathf.Clamp01(R);
        return (EaseIn(r) * (1f - r)) + (EaseOut(r) * r);
    }

    //Cubic ease in
    public static float EaseInCubic(float R)
    {
        var r = Mathf.Clamp01(R);
        return r * r * r;
    }

    //Cubic ease out
    public static float EaseOutCubic(float R)
    {
        var r = Mathf.Clamp01(R);
        var s = 1f - r;
        var s2 = s * s * s;
        return 1f - s2;
    }

    //Cubic ease in and out
    public static float EaseInOutCubic(float R)
    {
        var r = Mathf.Clamp01(R);
        return (EaseInCubic(r) * (1f - r)) + (EaseOutCubic(r) * r);
    }

    //Sin ease
    //Return to start
    public static float BackAndForth(float R)
    {
        var r = Mathf.Clamp01(R);
        var cycle = r * Mathf.PI;
        var s = Mathf.Sin(cycle);
        return s;
    }
    
    //Fast at beginning and end
    public static float EaseMiddle(float R)
    {
        float r = Mathf.Clamp01(R);
        return (EaseOut(r) * (1f - r)) + (EaseIn(r) * r);
    }

    public static float FiveStep(float R)
    {
        float r = Mathf.Clamp01(R);
        return Mathf.Floor(r * 5) / 5;
        
    }
}
