﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamLKK._Model;
using System.Drawing;

namespace DamLKK._Control
{
    public class TrackGPSControl : IDisposable
    {
        TrackGPS tracking = new TrackGPS();
        Roller _OwnerRoller;
        public Roller Owner
        {
            get { return _OwnerRoller; }
            set { _OwnerRoller = value; tracking.OwnerRoller = _OwnerRoller; }
        }

        public TrackGPS Tracking
        {
            get { return tracking; }
            set { tracking= value; }
        }
        public TrackGPSControl() { }
        public void Dispose() { tracking.Dispose(); GC.SuppressFinalize(this); }

        public static bool IsValid(List<Geo.GPSCoord> clst, Geo.GPSCoord c, Deck di, Roller ci)
        {
            if (clst.Count == 0)
                return true;
            if (clst.Count < 1)
                return true;
            c.Z -= ci.GPSHeight;

            TimeSpan ts = c.When - clst.Last().When;
            if (ts.TotalSeconds > _Model.Config.I.BASE_FILTER_SECONDS)
                return true;

            Geo.Vector v = new DamLKK.Geo.Vector(clst.Last(), c);
            if (v.Length() > _Model.Config.I.BASE_FILTER_METERS)
                return false;

            return true;
        }
        //private List<Geo.GPSCoord> Translate(List<DB.TracePoint> pts)
        //{
        //    List<Geo.GPSCoord> lst = new List<DamLKK.Geo.GPSCoord>();
        //    Geo.XYZ xyz;
        //    Geo.BLH blh;
        //    foreach (DB.TracePoint pt in pts)
        //    {
        //        blh = new DamLKK.Geo.BLH(pt.Y, pt.X, pt.Z);
        //        xyz = Geo.Coord84To54.Convert(blh);
        //        Geo.GPSCoord c3d = new DamLKK.Geo.GPSCoord(xyz.y, -xyz.x, xyz.z, (double)pt.V / 100, 0);
        //        c3d.When = pt.Dttrace;
        //        c3d.Z -= owner.Info.GPSHeight;
        //        if (owner.Owner.Owner.RectContains(c3d.Plane) && IsValid(lst, c3d, owner.Owner.DeckInfo, owner.Info))
        //            lst.Add(c3d);
        //    }

        //    return lst;
        //}
        public void LoadDB()
        {
            try
            {
                List<DamLKK.Geo.GPSCoord> lst = DB.TracePointDAO.GetInstance().GetGPSCoordList(
                    _OwnerRoller.Assignment.RollerID,
                    _OwnerRoller.Assignment.DTStart,
                    _OwnerRoller.Assignment.DTEnd);
                List<Geo.GPSCoord> pts = Translate(lst);
                this.Tracking.SetTracking(pts, 0, 0);
            }
            catch
            {

            }
        }

        private List<Geo.GPSCoord> Translate(List<Geo.GPSCoord> pts)
        {
            List<Geo.GPSCoord> lst = new List<Geo.GPSCoord>();
            Geo.XYZ xyz;
            Geo.BLH blh;
            foreach (Geo.GPSCoord pt in pts)
            {
                blh = new Geo.BLH(pt.Plane.Y, pt.Plane.X, pt.Z);
                xyz = Geo.Coord84To54.Convert(blh);
                Geo.GPSCoord c3d = new Geo.GPSCoord(xyz.y, -xyz.x, xyz.z, (double)pt.V / 100, pt.Tag,pt.When,pt.LibratedStatus);
                //Geo.Coord3D c3d = new DM.Geo.Coord3D(xyz.y, -xyz.x, xyz.z, (double)pt.V / 100, 0);
                c3d.When = pt.When;
                c3d.Z -= _OwnerRoller.GPSHeight;
                if (_OwnerRoller.Owner.MyLayer.RectContains(c3d.Plane) && IsValid(lst, c3d, _OwnerRoller.Owner, _OwnerRoller))
                    lst.Add(c3d);
            }

            return lst;
        }

        public void Draw(Graphics g, bool frameonly)
        {
            tracking.Draw(g, frameonly,0);
        }

        public void CheckOverThickness(Geo.GPSCoord c)
        {
            Deck deckinfo = _OwnerRoller.Owner;
            double designdepth = deckinfo.DesignDepth;
            double error = deckinfo.ErrorParam/100;
            double actual = c.Z - deckinfo.StartZ;
            double hi = designdepth * (1 + error);
            
            if (actual > hi)
            {
                Geo.Coord damaxis = c.Plane.ToDamAxisCoord();
                string warning = string.Format("铺层超厚报警：限制：{0:0.00}*(1+{1:P})米, 实际：{2:0.00}米, 位置：{3}", 
                    designdepth, error, actual, damaxis.ToString());
                WarningControl.SendMessage(WarningType.OVERTHICKNESS, Owner.Owner.Unit.ID, warning);
                //System.Diagnostics.Debug.Print(warning);
            }
        }

    }
}
