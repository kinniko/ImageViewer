﻿using System;
using System.Globalization;
using ImageFramework.DirectX;
using ImageFramework.Model.Shader;

namespace ImageFramework.Model.Statistics
{
    /// <summary>
    /// statistics about an image
    /// </summary>
    public class StatisticsModel : IDisposable
    {
        internal GpuBuffer buffer = null;
        private StatisticsShader luminanceShader;
        private StatisticsShader uniformShader;
        private StatisticsShader lumaShader;
        private StatisticsShader lightnessShader;
        private StatisticsShader alphaShader;
        private ReduceShader minReduce;
        private ReduceShader maxReduce;
        private ReduceShader avgReduce;
        // TODO replace with a shared model download buffer
        private DownloadBuffer<float> downloadBuffer = null;

        internal StatisticsShader LuminanceShader => luminanceShader ?? (luminanceShader = new StatisticsShader(StatisticsShader.LuminanceValue));
        internal StatisticsShader UniformShader => uniformShader ?? (uniformShader = new StatisticsShader(StatisticsShader.UniformWeightValue));
        internal StatisticsShader LumaShader => lumaShader ?? (lumaShader = new StatisticsShader(StatisticsShader.LumaValue));
        internal StatisticsShader LightnessShader => lightnessShader ?? (lightnessShader = new StatisticsShader(StatisticsShader.LightnessValue));
        internal StatisticsShader AlphaShader => alphaShader ?? (alphaShader = new StatisticsShader(StatisticsShader.AlphaValue));

        internal ReduceShader MinReduce => minReduce ?? (minReduce = new ReduceShader("min(a,b)", float.MaxValue.ToString(CultureInfo.InvariantCulture)));
        internal ReduceShader MaxReduce => maxReduce ?? (maxReduce = new ReduceShader("max(a,b)", float.MinValue.ToString(CultureInfo.InvariantCulture)));
        internal ReduceShader AvgReduce => avgReduce ?? (avgReduce = new ReduceShader("a+b", "0.0"));
        //public bool HasAlpha => !(Min.Alpha == 1.0f && Max.Alpha == 1.0f);

        internal float GetStats(ITexture texture, int layer, int mipmap, StatisticsShader statShader, ReduceShader redShader, bool normalize)
        {
            // obtain a buffer that is big enough
            int numElements = texture.Size.GetMip(mipmap).Product;
            if(layer == -1) numElements *= texture.NumLayers;

            if (buffer == null || buffer.ElementCount < numElements)
            {
                buffer?.Dispose();
                buffer = new GpuBuffer(4, numElements);
            }

            // copy all values into buffer
            statShader.CopyToBuffer(texture, buffer, layer, mipmap);

            // execute reduce
            redShader.Run(buffer, numElements);

            // extract from buffer[0]
            if (downloadBuffer == null)
            {
                downloadBuffer = new DownloadBuffer<float>();
            }

            downloadBuffer.CopyFrom(buffer);

            var res = downloadBuffer.GetData();
            if (normalize) res /= numElements;
            return res;
        }

        public DefaultStatistics GetStatisticsFor(ITexture texture, int layer = -1, int mipmap = 0)
        {
            return new DefaultStatistics(this, texture, layer, mipmap);
        }

        public void Dispose()
        {
            buffer?.Dispose();
            downloadBuffer?.Dispose();
            luminanceShader?.Dispose();
            uniformShader?.Dispose();
            lumaShader?.Dispose();
            lightnessShader?.Dispose();
            minReduce?.Dispose();
            maxReduce?.Dispose();
            avgReduce?.Dispose();
        }
    }
}