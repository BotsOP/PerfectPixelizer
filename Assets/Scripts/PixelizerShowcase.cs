using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelizerShowcase : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Texture texture;
    [SerializeField, Range(0, 1.0f)] private float amountPixels;
    [SerializeField] private int pixelSize;
    private Pixelizer pixelizer;

    private void Awake()
    {
        pixelizer = new Pixelizer();
    }

    void Update()
    {
        mat.SetTexture("_BaseMap", pixelizer.Pixelize(texture, pixelSize));
    }
}
