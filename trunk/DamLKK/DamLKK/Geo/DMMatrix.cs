using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK.Geo
{
    /// <大坝模型>
    /// 大坝模型
    /// </大坝模型>
    public struct DMMatrix
    {
        /// <图形>
        /// 图形
        /// </图形>
        public DMRectangle Boundary;

        /// <summary>
        /// 原点
        /// </summary>
        public Coord Origin;

        /// <summary>
        /// 旋转的角度
        /// </summary>
        public double Degrees;
        /// <summary>
        /// 旋转的基准点
        /// </summary>
        public Coord At;
        // scale
        /// <summary>
        /// 放大的比例
        /// </summary>
        public double Zoom;
        /// <summary>
        /// 偏移坐标
        /// </summary>
        public Coord Offset;

        public DMMatrix(DMMatrix mtx)
        {
            this.Boundary = new DMRectangle(mtx.Boundary);
            this.Origin = new Coord(mtx.Origin);
            this.Degrees = mtx.Degrees;
            this.At = new Coord(mtx.At);
            this.Zoom = mtx.Zoom;
            this.Offset = new Coord(mtx.Offset);
        }
    }
}
