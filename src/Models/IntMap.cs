using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicUnlocker.Models
{
    internal record PixelLongColors(int R, int G, int B);
    
    internal class IntMap
    {
        protected int Width;
        protected int Height;
        private PixelLongColors[,] longPixels;

        internal IntMap(int sizeX, int sizeY)
        {
            Width = sizeX;
            Height = sizeY;
            longPixels =  new PixelLongColors[sizeX, sizeY];
        }        

        internal void SetLongPixel(int x, int y, PixelLongColors PixelLongData)
        {
            longPixels[x, y] = PixelLongData;
        }

        internal PixelLongColors GetLongPixel(int x, int y)
        {
            return longPixels[x, y];
        }

    }
}
