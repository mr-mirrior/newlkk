using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK.DB.datamap
{
    class Pixel
    {
        long segmentid;
        int rollcount;
        float rollthickness;
        public int getRollcount()
        {
            return rollcount;
        }
        public void setRollcount(int rollcount)
        {
            this.rollcount = rollcount;
        }
        public float getRollthickness()
        {
            return rollthickness;
        }
        public void setRollthickness(float rollthickness)
        {
            this.rollthickness = rollthickness;
        }
        public Pixel(int rollcount, float rollthickness)
        {
            this.rollcount = rollcount;
            this.rollthickness = rollthickness;
        }
        public long getSegmentid()
        {
            return segmentid;
        }
        public void setSegmentid(long l)
        {
            this.segmentid = l;
        }
    }
}
