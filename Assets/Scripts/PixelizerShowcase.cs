using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelizerShowcase : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Texture texture;
    [SerializeField] private int pixelSize;
    private Pixelizer pixelizer;
    private DynamicPixelizer dynamicPixelizer;

    private void Awake()
    {
        pixelizer = new Pixelizer();
        dynamicPixelizer = new DynamicPixelizer();
    }

    void Update()
    {
        //mat.SetTexture("_BaseMap", pixelizer.Pixelize(texture, pixelSize));
        mat.SetTexture("_BaseMap", dynamicPixelizer.Pixelize(texture, pixelSize));
    }
}
