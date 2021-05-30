using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public static float GetPerlinValue(int x, int z, int seed, float frequency, float amplitude, float scale)
    {
        System.Random prng = new System.Random(seed);

        int seedNum = prng.Next(-100000, 100000);

        float sampleX = x / 64f * scale + seedNum;
        float sampleZ = z / 64f * scale + seedNum;

        float perlinValue = (Mathf.PerlinNoise(sampleX, sampleZ) * frequency) * amplitude;

        return perlinValue;
    }
}
