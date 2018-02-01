using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator {
    private Vector2 origin;
    private float scale;
    private System.Random RNG;

    public NoiseGenerator(float scale = 1.0f, int seed = -1) {
        if (seed == -1) {
            RNG = new System.Random();
        } else {
            RNG = new System.Random(seed);
        }

        this.scale = scale;
        this.origin = new Vector2(this.RNG.Next(-100000, 100000), this.RNG.Next());
        this.origin = new Vector2(50, 50);
    }

    public float Generate(float x, float y) {
        float sampleX = (origin.x + x) / this.scale;
        float sampleY = (origin.y + y) / this.scale;
        return Mathf.PerlinNoise(sampleX, sampleY);
    }

}
