using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixelizer
{
    private int maxKernelSize = 5;
    private ComputeShader pixelShader;
    private Vector3 threadGroupSize;
    private int kernelID;
    private CustomRenderTexture rt;
    
    public Pixelizer()
    {
        pixelShader = Resources.Load<ComputeShader>("Pixelizer");
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

        int pixelSizeKernel = Mathf.Clamp(pixelSize, 2, maxKernelSize);
        string pixelKernel = "Pixelizer" + pixelSizeKernel;
        kernelID = pixelShader.FindKernel(pixelKernel);
        
        pixelShader.GetKernelThreadGroupSizes(kernelID, out uint threadGroupSizeX, out uint threadGroupSizeY, out _);
        threadGroupSize.x = Mathf.CeilToInt((float)texture.width / threadGroupSizeX);
        threadGroupSize.y = Mathf.CeilToInt((float)texture.height / threadGroupSizeY);
        
        pixelShader.SetTexture(kernelID, "_Result", rt);
        pixelShader.SetInt("_TextureWidth", texture.width);
        pixelShader.SetInt("_TextureHeight", texture.height);
        pixelShader.SetInt("_PixelSize", pixelSize);
        pixelShader.Dispatch(kernelID, (int)threadGroupSize.x, (int)threadGroupSize.y, 1);

        return rt;
    }
    
    public CustomRenderTexture Pixelize(Texture texture, int pixelSize, int pixelSizeKernel)
    {
        if (rt == null)
        {
            CreateRT(texture.width, texture.height);
        }
        if (rt.width != texture.width || rt.height != texture.height)
        {
            CreateRT(texture.width, texture.height);
        }
        
        //Copies texture to rendertexture
        Graphics.Blit(texture, rt);

        //If pixelsize is smaller then just return the texture
        if (pixelSize <= 1)
        {
            return rt;
        }

        //Gets the kernel ID of the specific pixelSizeKernel
        pixelSizeKernel = Mathf.Clamp(pixelSizeKernel, 2, maxKernelSize);
        string pixelKernel = "Pixelizer" + pixelSizeKernel;
        kernelID = pixelShader.FindKernel(pixelKernel);
        
        //divides texture size by the threadgroup
        pixelShader.GetKernelThreadGroupSizes(kernelID, out uint threadGroupSizeX, out uint threadGroupSizeY, out _);
        threadGroupSize.x = Mathf.CeilToInt((float)texture.width / threadGroupSizeX);
        threadGroupSize.y = Mathf.CeilToInt((float)texture.height / threadGroupSizeY);
        
        //Sets all the variables
        pixelShader.SetTexture(kernelID, "_Result", rt);
        pixelShader.SetInt("_TextureWidth", texture.width);
        pixelShader.SetInt("_TextureHeight", texture.height);
        pixelShader.SetInt("_PixelSize", pixelSize);
        pixelShader.Dispatch(kernelID, (int)threadGroupSize.x, (int)threadGroupSize.y, 1);

        return rt;
    }

    private void CreateRT(int textureWidth, int textureHeight)
    {
        rt = new CustomRenderTexture(textureWidth, textureHeight, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
    }
}
