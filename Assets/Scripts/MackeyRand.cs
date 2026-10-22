using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public static class CustomRand
{
    //Seeded results kept seperate========================================================================
    private static uint runningState = 0;
    private static void TransformState(int seed)
    {
        uint effector = (uint)seed;

        runningState ^= effector; //flips
        runningState = runningState * 1664525u + 1013904223u; //mix in range
        runningState ^= runningState >> 16; //shift
        runningState ^= (effector << 13) | (effector >> 19); //remix after bit shift
        runningState *= 2246822519u; //middle point multi

        //buncha random noise
        runningState ^= runningState >> 13; 
        runningState *= 3266489917u;
        runningState ^= runningState >> 16;
        //
    }
   
    public static float Random(float low, float high, int seed)
    {
        if (seed == 0) return low;

        TransformState(seed);

        float normalized = runningState / 4294967296f;

        float toReturn = low + normalized * (high - low);

        return toReturn;
    }

    public static float Random01(int seed)
    {
        return Random(0, 1, seed);
    }
    //=====================================================================================================

    //seedless... dont want seedless effecting seeded results==============================================
    private static uint runningState2 = 0;
    private static void TransformStateSeedless()
    {
        uint effector = (uint)((int)Time.time + DateTime.Now.Ticks + DateTime.Now.Year + DateTime.Now.DayOfYear + Time.deltaTime);

        runningState2 ^= effector; //flips
        runningState2 = runningState2 * 1664525u + 1013904223u; //mix in range
        runningState2 ^= runningState2 >> 16; //shift
        runningState2 ^= (effector << 13) | (effector >> 19); //remix after bit shift
        runningState2 *= 2246822519u; //middle point multi

        //buncha random noise
        runningState2 ^= runningState2 >> 13;
        runningState2 *= 3266489917u;
        runningState2 ^= runningState2 >> 16;
        //
    }

    public static float Random(float low, float high)
    {

        TransformStateSeedless();

        float normalized = runningState2 / 4294967296f;

        float toReturn = low + normalized * (high - low);

        return toReturn;
    }
    public static float Random01()
    {
        return Random(0, 1);
    }
    //=========================================================================================================
}
