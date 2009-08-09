using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamLKK._Model;

namespace DamLKK._Control
{
    public class LayerControl
    {
        static LayerControl ctl = new LayerControl();

        public static LayerControl Instance
        {
            get { return LayerControl.ctl; }
        }
        public event EventHandler OnWorkingLayersChange;

        List<DamLKK._Model.Deck> workingLayers = new List<DamLKK._Model.Deck>();

        public List<DamLKK._Model.Deck> WorkingLayers
        {
            get { return workingLayers; }
        }


        public void LoadWorkingLayer()
        {
            WorkingLayers.Clear();
            List<_Model.RollerDis> lst = DB.CarDistributeDAO.GetInstance().GetInusedCarDis();
            if (lst == null)
                return;
            DB.DeckDAO dao = DB.DeckDAO.GetInstance();
            foreach (_Model.RollerDis cd in lst)
            {
                try
                {
                    DamLKK._Model.Deck working = dao.GetDeck(cd.UnitID, cd.Elevation, cd.SegmentID);
                    if (working != null)
                        WorkingLayers.Add(working);
                }
                catch
                {
                    continue;
                }
            }
            if (null != OnWorkingLayersChange)
                OnWorkingLayersChange.Invoke(null, null);
        }

        // 已经打开视图的层列表
        List<Views.LayerView> _Layerviews = new List<Views.LayerView>();

        #region - 查找 层、视图 -
        public DamLKK._Model.Layer FindLayerByPE(DamLKK._Model.Unit unit, _Model.Elevation elevation)
        {
            Views.LayerView view = FindViewByPE(unit, elevation);
            if (view == null)
                return null;
            return view.MyLayer;
        }
        public _Model.Layer FindLayerByPE(string unit, float elevation)
        {
            Views.LayerView view = FindViewByPE(unit, elevation);
            if (view == null)
                return null;
            return view.MyLayer;
        }
        public Views.LayerView FindViewByPE(DamLKK._Model.Unit unit, _Model.Elevation elevation)
        {
            return FindViewByPE(unit.Name, elevation.Height);
        }
        public Views.LayerView FindViewByPE(string unitname, double elevation)
        {
            foreach (Views.LayerView view in _Layerviews)
            {
                if (view.MyLayer.MyUnit.Name.Equals(unitname) &&
                    view.MyLayer.MyElevation.Height == elevation)
                    return view;
            }
            return null;
        }
        // 根据名字查找层，name=Layer.ToString();
        public _Model.Layer FindLayerByName(string name)
        {
            Views.LayerView layer = FindViewByName(name);
            if (layer == null)
                return null;
            return layer.MyLayer;
        }
        // 根据名字查找层视图，name=Layer.ToString();
        public Views.LayerView FindViewByName(string name)
        {
            foreach (Views.LayerView view in _Layerviews)
            {
                if (view.MyLayer.IsEqual(name))
                    return view;
            }
            return null;
        }
        // 根据完整路径名查找层
        private Views.LayerView FindView(Unit unit,Elevation e)
        {
            foreach (Views.LayerView view in _Layerviews)
            {
                if (view.IsEqual(unit,e))
                    return view;
            }
            return null;
        }
        #endregion
        // 返回true：成功

        // 返回false：已打开该层
        private Views.LayerView _Current = null;
        public Views.LayerView OpenLayer(DamLKK._Model.Unit unit,_Model.Elevation elevation)
        {
            if (unit == null || elevation == null)
                return null;

            Views.LayerView view = FindView(unit,elevation);
            if (view != null)
            {
                if (Utils.MB.OKCancelQ("已经打开该层，要转到该层吗？"))
                    _Model.Dam.GetInstance().CurrentUnit.GoLayer(view);
                return null;
            }

            view = Forms.Main.GetInstance.OpenLayer(unit, elevation);
            if (view == null)
                return null;

            _Layerviews.Add(view);
            if (_Current != null)
                _Current.OnLostTab();
            _Current = view;
            view.OnActiveTab();
            return view;
        }
        // 关闭层

        public void CloseLayer(Views.LayerView view)
        {
            if (view == null)
                return;
            if (_Layerviews.IndexOf(view) == -1)
                return;

            if (_Model.Dam.GetInstance().CurrentUnit.CloseWnd(typeof(LayerControl), view))
            {
                _Layerviews.Remove(view);
                view.Dispose();
            }
        }
        public void ChangeCurrentLayer(Views.LayerView view)
        {
            if (view == null)
                return;
            if (_Current != null && _Current != view)
            {
                _Current.OnLostTab();
            }
            _Current = view;
            _Current.OnActiveTab();
        }

        public DamLKK._Model.Deck FindDeck(int carid)
        {
            try
            {
                //RollerDis cd = VehicleControl.FindVehicleInUse(carid);
                //_Model.Deck part = UnitControl.FromID(cd.Blockid);
                //_Model.Elevation elev = new DM.Models.Elevation(cd.DesignZ);

                //List<DamLKK._Model.Deck> seg = DB.DeckDAO.GetInstance().GetSegment(cd.Blockid, cd.DesignZ, cd.Segmentid);
                //if (seg.Count == 0)
                //    return null;
                //return seg.First();
                //Models.Layer layer = this.FindLayerByPE(part, elev);
                //return layer.DeckControl.FindDeckByIndex(cd.Segmentid);
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}
