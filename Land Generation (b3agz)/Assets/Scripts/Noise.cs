using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float SimpleNoise(int x, int z, int seed, float scale, float amplitude)
    {
        System.Random prng = new System.Random(seed);

        int seedNum = prng.Next(-100000, 100000);

        if (scale < 0)
            scale = 0.0001f;

        float sampleX = (float)x / 64f * scale + seedNum;
        float sampleZ = (float)z / 64f * scale + seedNum;

        float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;

        return perlinValue;

    }
}
