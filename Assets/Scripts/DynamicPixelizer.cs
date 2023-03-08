using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DynamicPixelizer
{
    private int maxKernelSize = 5;
    private ComputeShader pixelShader;
    private Vector3 threadGroupSize;
    private int pixelizeKernelID;
    private int readPixelizedKernelID;
    private CustomRenderTexture rt;
    private CustomRenderTexture colorR;
    private CustomRenderTexture colorG;
    private CustomRenderTexture colorB;
    
    public DynamicPixelizer()
    {
        pixelShader = Resources.Load<ComputeShader>("DynamicPixelizer");
        pixelizeKernelID = pixelShader.FindKernel("Pixelize");
        readPixelizedKernelID = pixelShader.FindKernel("ReadPixelized");
    }

    public CustomRenderTexture Pixelize(Texture texture, int pixelSize)
    {
        if (rt == null)
        {
            CreateRT(texture.width, texture.height);
        }
        if (rt.width != texture.width || rt.height != texture.height)
        {
            CreateRT(texture.width, texture.height);
        }
        
        Graphics.Blit(texture, rt);

        if (pixelSize <= 1)
        {
            return rt;
        }
        
        colorR.Release();
        colorG.Release();
        colorB.Release();

        pixelShader.GetKernelThreadGroupSizes(pixelizeKernelID, out uint threadGroupSizeX, out uint threadGroupSizeY, out _);
        threadGroupSize.x = Mathf.CeilToInt((float)texture.width / threadGroupSizeX);
        threadGroupSize.y = Mathf.CeilToInt((float)texture.height / threadGroupSizeY);
        
        pixelShader.SetInt("_PixelSize", pixelSize);
        
        pixelShader.SetTexture(pixelizeKernelID, "_Source", rt);
        pixelShader.SetTexture(pixelizeKernelID, "_ResultR", colorR);
        pixelShader.SetTexture(pixelizeKernelID, "_ResultG", colorG);
        pixelShader.SetTexture(pixelizeKernelID, "_ResultB", colorB);
        pixelShader.Dispatch(pixelizeKernelID, (int)threadGroupSize.x, (int)threadGroupSize.y, 1);
        
        pixelShader.SetTexture(readPixelizedKernelID, "_Source", rt);
        pixelShader.SetTexture(readPixelizedKernelID, "_ResultR", colorR);
        pixelShader.SetTexture(readPixelizedKernelID, "_ResultG", colorG);
        pixelShader.SetTexture(readPixelizedKernelID, "_ResultB", colorB);
        pixelShader.Dispatch(readPixelizedKernelID, (int)threadGroupSize.x, (int)threadGroupSize.y, 1);

        return rt;
    }
    private void CreateRT(int textureWidth, int textureHeight)
    {
        rt = new CustomRenderTexture(textureWidth, textureHeight, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        
        colorR = new CustomRenderTexture(textureWidth, textureHeight, RenderTextureFormat.RInt,
            RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        colorG = new CustomRenderTexture(textureWidth, textureHeight, RenderTextureFormat.RInt,
            RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        colorB = new CustomRenderTexture(textureWidth, textureHeight, RenderTextureFormat.RInt,
            RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
    }
}
