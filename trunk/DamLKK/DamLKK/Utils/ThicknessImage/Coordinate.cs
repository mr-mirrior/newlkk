using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK.DB.datamap
{
    class Coordinate
    {
        double x;
        double y;
        public Coordinate(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double getX()
        {
            return x;
        }
        public void setX(double x)
        {
            this.x = x;
        }
        public double getY()
        {
            return y;
        }
        public void setY(double y)
        {
            this.y = y;
        }
    }
}
