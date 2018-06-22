using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator {
    private Vector2 origin;
    private float scale;
    private System.Random RNG;
    private int octaveCount;
    private float lacunarity;
    private float persistence;

    public NoiseGenerator(float scale = 1.0f,
        int seed = -1,
        int octaveCount = 1,
        float lacunarity = 2f,
        float persistence = 0.5f) {

        if (seed == -1) {
            RNG = new System.Random();
        } else {
            RNG = new System.Random(seed);
        }

        int offset_x = this.RNG.Next(100000, 2000000);
        int offset_z = this.RNG.Next(100000, 2000000);

        this.scale = scale;
        this.origin = new Vector2(offset_x, offset_z);
        this.origin = new Vector2(0, 0);
        this.octaveCount = octaveCount;
        this.lacunarity = lacunarity;
        this.persistence = persistence;
    }

    /// <summary>
    /// The generate function is responsible for calculating a final noise value.
    /// This one particularly is based on Perlin noise.
    /// </summary>
    /// <param name="x">The X coordinate to sample a point for.</param>
    /// <param name="y">The Y coordinate to sample a point for.</param>
    /// <returns>The noise value which is between 0.0 and 1.0.</returns>
    public float Generate(float x, float y) {
        float frequency = 1;
        float amplitude = 1;
        float value = 0;

        for (int i = 0; i < this.octaveCount; i++) {
            float sampleX = (this.origin.x + x) / this.scale * frequency;
            float sampleY = (this.origin.y + y) / this.scale * frequency;
            float sampleValue = (Mathf.PerlinNoise(sampleX, sampleY) - 1) * amplitude + 1;
            value += Mathf.Clamp(sampleValue, 0, 1);
            frequency *= this.lacunarity;
            amplitude *= this.persistence;
        }

        return value / this.octaveCount;
    }

}
