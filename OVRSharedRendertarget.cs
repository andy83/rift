using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OculusWrap;
using SwapTextureSet = OculusWrap.GL.SwapTextureSet;
using System;
using System.Diagnostics;

namespace SimpleDemo
{
    public class OvrSharedRendertarget
    {
        private OculusWrap.GL.SwapTextureSet textureSet;

        private int fboId;

        private int width, height;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public OvrSharedRendertarget(int w, int h, Hmd hmd)
        {
            width = w;
            height = h;

            hmd.CreateSwapTextureSetGL((uint)All.Rgba, width, height, out textureSet);

            for (int i = 0; i < textureSet.TextureCount; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, textureSet.Textures[i].TexId);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }
            
            GL.GenFramebuffers(1, out fboId);

            FramebufferErrorCode Status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            bool success = true;
            if (Status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FB error, status: " + Status);
                Console.WriteLine("OvrRT :" + fboId + " CurrentIndex: " + textureSet.CurrentIndex);
                success = false;
            }
        }

        public SwapTextureSet TextureSet
        {
            get { return textureSet; }
        }

        float r = 0f;
        float g = 1f;
        public void Bind(uint depth)
        {
            OVR.GL.GLTextureData tex = textureSet.Textures[textureSet.CurrentIndex];
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, tex.TexId, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depth, 0);

            // Here you can check if framebuffer is complete or not
            FramebufferErrorCode Status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            bool success = true;
            if (Status != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FB error, status: " + Status);
                Console.WriteLine("OvrRT :" + fboId + " CurrentIndex: " + textureSet.CurrentIndex);
                success = false;
            }
        }

        public void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, 0, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, 0, 0);
        }

        public void CleanUp()
        {
            textureSet.Dispose();

            if (fboId != 0)
                GL.DeleteFramebuffers(1, ref fboId);
        }
    }
}
