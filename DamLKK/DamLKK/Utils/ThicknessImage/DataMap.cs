using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK.DB.datamap
{
    class DataMap
    {
        private int width;
        private int height;
        private byte[] bytesData;

        public byte[] getBytesData()
        {
            return bytesData;
        }
        public void setBytesData(byte[] bytesData)
        {
            this.bytesData = bytesData;
        }
        public int getWidth()
        {
            return width;
        }
        public void setWidth(int width)
        {
            this.width = width;
        }
        public int getHeight()
        {
            return height;
        }
        public void setHeight(int height)
        {
            this.height = height;
        }

        public DataMap(byte[] bytes)
        {
            setBytesData(bytes);
            setWidth(BitAccess.READ_C_int(bytesData, 0));
            setHeight(BitAccess.READ_C_int(bytesData, 4));
        }

        public Pixel getPixel(int x, int y)
        {
            int begin_offset = (y * width + x) * 5 + 8;
            if (begin_offset<0)
            {
                return null;
            }
            if(begin_offset>bytesData.Length){//不知道为何会出现这种情况.待研究
                return null;
            }
            int rollcount = BitAccess.READ_byte(bytesData, begin_offset);
            float rollthickness = BitAccess.READ_C_float(bytesData, begin_offset + 1);

            return new Pixel(rollcount, rollthickness);
        }
	
    }
}
