using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static int coinScore = 0;

    //magnet power
    static bool magnetPower = false;
    static float magnetPowerDuration = 15.0f;
    public static float magnetRadius = 10.0f;
    public static float totalMagnetTime = 0.0f;

    //2x power
    static bool twoXPower = false;
    static float twoXPowerDuration = 10.0f;
    public static float total2XPowerTime = 0.0f;

    //reverse keys disadvantage
    public static bool swapKeys = false;

    //speed of platform (meaning speed of player)
    public static float moveSpeedPlatform = 15.0f;
    //speed of moving player left and right, it increases at the same rate as moving upward
    public static float leftRightSpeed = 8.0f;


    public static void collectCoin()
    {
        if (twoXPower)
        {
            coinScore += 2;
        }
        else
        {
            coinScore += 1;
        }

    }

    public static void enableMagnet()
    {
        magnetPower = true;
        totalMagnetTime += magnetPowerDuration;
    }

    public static void disableMagnet()
    {
        magnetPower = false;
        totalMagnetTime = 0.0f;
    }

    public static void enable2XPower()
    {
        twoXPower = true;
        total2XPowerTime += twoXPowerDuration;
    }

    public static void disable2XPower()
    {
        twoXPower = false;
        total2XPowerTime = 0.0f;
    }

    public static bool has2XPower()
    {
        return (twoXPower && total2XPowerTime > 0.0f);
    }

    public static bool hasMagnetPower()
    {
        return (magnetPower && totalMagnetTime > 0.0f);
    }
}
