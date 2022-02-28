using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicUnlocker.Models
{
    internal class IntMap
    {
        internal uint Width;
        internal uint Height;
        private PixelLongColors[,] longPixels;

        internal IntMap(uint sizeX, uint sizeY)
        {
            Width = sizeX;
            Height = sizeY;
            var arr = new PixelLongColors[sizeX, sizeY];
        }
        internal record PixelLongColors(Int16 R, Int16 G, Int16 B);

        internal void SetLongPixel(uint x, uint y, PixelLongColors PixelLongData)
        {
            longPixels[x, y] = PixelLongData;
        }

        internal PixelLongColors GetLongPixel(uint x, uint y)
        {
            return longPixels[x, y];
        }

    }
}
