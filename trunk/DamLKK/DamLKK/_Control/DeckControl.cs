using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamLKK._Model;
using DamLKK.DB;
using System.Drawing;

namespace DamLKK._Control
{
    public class DeckControl :IDisposable
    {
        public DeckControl(){}
        public DeckControl(Layer _owner) { _Layer = _owner; if (_Layer != null) { unit = _Layer.MyUnit; elevation = _Layer.MyElevation; } }

        List<DamLKK._Model.Deck> _Decks = new List<DamLKK._Model.Deck>();

        public void Dispose()
        {
            foreach (Deck dk in _Decks)
            {
                dk.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public List<DamLKK._Model.Deck> Decks
        {
            get { return _Decks; }
            set { _Decks = value; }
        }

        Layer _Layer = null;

        Unit unit;
        Elevation elevation;

        public Layer Owner
        {
            get { return _Layer; }
            set { _Layer = value; unit = _Layer.MyUnit; elevation = _Layer.MyElevation; }
        }

        Deck _Tobesetvisible = null;
        Forms.Waiting _DlgWaiting;
        public void SetVisibleDeck(DamLKK._Model.Deck deck)
        {
            _Tobesetvisible = deck;
            _DlgWaiting = new Forms.Waiting();
            _DlgWaiting.Start(Forms.Main.GetInstance, "正在计算轨迹，请稍候……", thrdSetVisibleDeck, 1000);
        }

        public Deck GetVisibleDeck()
        {
            foreach (Deck dk in _Decks)
            {
                if (dk.IsVisible)
                    return dk;
            }
            return null;
        }

        private void thrdSetVisibleDeck()
        {
            if (_Tobesetvisible == null)
                return;
            foreach (Deck dk in _Decks)
            {
                if (dk.IsEqual(_Tobesetvisible))
                {
                    dk.IsVisible = true;
                    //LoadDBVehicle(dk);
                }
                else
                {
                    UnvisibleDeck(dk);
                }
            }

            UpdateGraphics();
            ReportRolling(_Tobesetvisible);
            _Tobesetvisible = null;

        }

        /// <summary>
        /// 碾压结束报告
        /// </summary>
        private void ReportRolling(Deck dk)
        {
            if (!_IsStoppingDeck)
                return;
            Forms.Warning warndlg = new Forms.Warning();
            warndlg.UnitName = dk.Unit.Name;
            _IsStoppingDeck = false;

            int[] areas = null;
            Bitmap bmp = dk.CreateRollCountImage(out areas,0);
            bmp.Dispose();

            int expected = dk.NOLibRollCount+dk.LibRollCount;
            double[] area_ratio = _Model.Deck.AreaRatio(areas, dk);
            double ok = 0;
            double nok = 0;
            if (area_ratio == null || area_ratio.Length == 0)
            {
                Utils.MB.Warning("标准遍数百分比计算异常！");
            }
            else
            {
                ok = area_ratio[expected];
                for (int i = 0; i < expected; i++)
                {
                    nok += area_ratio[i];
                }
            }


            if (area_ratio != null && area_ratio.Length != 0)
                dk.POP = ok;
            else
                dk.POP = -1;
            string warning = string.Format("碾压简报：{0}-{1}米 仓面：{2}，碾压标准：{3}遍，碾压合格：{4:P}",
                    dk.Unit.Name,
                    dk.Elevation.Height,
                    dk.Name,
                    dk.NOLibRollCount+dk.LibRollCount,
                    dk.POP);
            DeckDAO.GetInstance().SetDeckPOP(dk.Unit.ID, dk.Elevation.Height, dk.ID, dk.POP);

            _Control.WarningControl.SendMessage(WarningType.ROLLINGLESS, dk.Unit.ID, warning);

            //dk.UpdateName();
            warndlg.UnitName = dk.Unit.Name;
            warndlg.DeckName = dk.Name;
            warndlg.DesignZ = dk.Elevation.Height;
            warndlg.ShortRollerArea = nok;
            warndlg.TotalAreaRatio = nok + ok;
            warndlg.ActualArea = dk.Polygon.ActualArea;
            warndlg.WarningDate = DB.DateUtil.GetDate().Date.ToString("D");
            warndlg.WarningTime = DB.DateUtil.GetDate().ToString("T");
            warndlg.WarningType = WarningType.ROLLINGLESS;
            warndlg.FillForms();
            Forms.Main.GetInstance.ShowWarningDlg(warndlg);
        }

        public static void UnvisibleDeck(DamLKK._Model.Deck dk)
        {
            if (dk == null)
                return;
            dk.IsVisible = false;
            dk.VehicleControl.Dispose();
        }

        /// <summary>
        /// 查找空闲的最小的index
        /// </summary>
        /// <returns></returns>
        private int FindFreeIndex()
        {
            int idx = 0;
            bool occupied = false;
            while (true)
            {
                occupied = false;
                for (int i = 0; i < _Decks.Count; i++)
                {
                    if (_Decks[i].ID == idx)
                    {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied)
                    return idx;
                idx++;
            }
        }

        /// <summary>
        /// 添加仓面
        /// </summary>
        /// <param name="deck"></param>
        public void AddDeck(DamLKK._Model.Deck deck)
        {
            int id = FindFreeIndex();
            deck.ID = id;

            if (ModifyDeck(deck))
            {
                deck.VehicleControl.AssignVehicle(deck);
            }
            else
            {
                if (!Utils.MB.OKCancelQ("新仓面信息未更改，要保存吗？\n\n按<确定>保存，<取消>抛弃"))
                {
                    return;
                }
                _Decks.Add(deck);
                return;
            }
        }

        /// <summary>
        ///添加新仓面或者 修改仓面
        /// </summary>
        public bool ModifyDeck(Deck deck)
        {
            if (_Decks == null)
                _Decks = new List<Deck>();

            int idx = -1;
            Deck dkFound = null;
            foreach (Deck dk in _Decks)
            {
                idx++;
                if (dk.IsEqual(deck))
                {
                    dkFound = dk;
                    break;
                }
            }

            Forms.DeckInfo dlg = new DamLKK.Forms.DeckInfo();

            // 如果是新增仓面

            if (dkFound == null)
            {
                dlg.Deck= deck;
                dlg.IsWorking = false;
            }
            else
            {
                dlg.Deck = _Decks[idx];
                dlg.IsWorking = (dkFound.WorkState== DeckWorkState.WORK || dkFound.WorkState == DeckWorkState.END);
            }
            dlg.BlockName = Owner.MyUnit.Name;

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }

            Deck tobemodified = new Deck(dlg.Deck);
            tobemodified.MyLayer = this.Owner;
            tobemodified.Polygon = deck.Polygon;

            if (dkFound == null || idx == -1)
                _Decks.Add(tobemodified);
            else
                _Decks[idx] = tobemodified;
            bool re = false;
            if (dkFound == null || idx == -1)
                re = SubmitDB(1,deck);
            else
                re = SubmitDB(3,tobemodified);

            if (re && dkFound == null)
            {
                Utils.MB.OKI("添加仓面成功，已保存至数据库");
            }
            else if (!re&& dkFound!=null)
            {
                Utils.MB.Warning("修改仓面信息失败！");
                return false;
            }
            else
            {
                Utils.MB.OKI("修改仓面信息成功");
            }

            LoadDB(deck);
            UpdateGraphics();

            return true;
        }

        /// <summary>
        /// 删除仓面
        /// </summary>
        /// <param name="deck"></param>
        public void DeleteDeck(Deck deck)
        {
            if (deck.WorkState== DeckWorkState.WORK|| deck.WorkState == DeckWorkState.END)
            {
                Utils.MB.Warning("该仓面处于开仓状态或者已经工作完成，无法删除。");
                return;
            }

            if (!Utils.MB.OKCancelQ("确定删除该仓面吗？\n\n" + deck.Name))
                return;
            if (SubmitDB(2,deck))
            {
                Forms.ToolsWindow.GetInstance().UpdateMode();
                Utils.MB.OKI("仓面已从数据库删除");
            }

            deck.VehicleControl.DeleteAll();

            _Decks.Remove(deck);
        }


        /// <summary>
        /// 执行数据库操作 1添加新仓面 2删除仓面 3修改仓面
        /// </summary>
        private bool SubmitDB(int i,Deck deck)
        {
            try
            {
                switch (i)
                {
                    case 1:
                        return DamLKK.DB.DeckDAO.GetInstance().AddDeck(deck);
                    case 2:
                        return DamLKK.DB.DeckDAO.GetInstance().DeleteDeck(deck.Unit.ID, deck.Elevation.Height, deck.ID);
                    case 3:
                        return DamLKK.DB.DeckDAO.GetInstance().ModifyDeck(deck);
                }
            }
            catch 
            {
                return false;
            }
            finally
            {
                //VehicleControl.LoadCarDistribute();
            }
            return false;
        }

        public void LoadDB(Deck old)
        {
            if (_Layer == null)
                return;

            _Decks = DB.DeckDAO.GetInstance().GetDecks(_Layer.MyUnit.ID, _Layer.MyElevation.Height);

            if (_Decks == null)
            {
                return;
            }

            foreach (Deck deck in _Decks)
            {
                deck.MyLayer = this.Owner;
                Polygon vertex = new Polygon();
                string s = deck.Vertex;
                // x1,y1;x2,y2;...
                string[] coord = s.Split(new char[] { ';' });
                if (coord.Length == 0)
                    return;
                List<Geo.Coord> lst = new List<Geo.Coord>();
                foreach (string c in coord)
                {
                    if (c == null || c.Length == 0)
                        continue;
                    string[] d = c.Split(new char[] { ',' });
                    if (d.Length == 2)
                    {
                        lst.Add(new Geo.Coord(double.Parse(d[0]), double.Parse(d[1])));
                    }
                }

                vertex.SetVertex(lst);
                deck.Polygon = vertex;
            }
         

            if (old == null)
                return;

            Deck dk = FindDeck(old);
            if (dk != null)
            {
                dk.DrawingComponent = old.DrawingComponent;
                if (old.IsVisible)
                    SetVisibleDeck(dk);
            }
        }

        private Deck FindDeckByName(string name)
        {
            foreach (Deck dk in _Decks)
            {
                if (dk.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return dk;
            }
            return null;
        }
        public Deck FindDeckByIndex(int id)
        {
            foreach (Deck dk in _Decks)
            {
                if (dk.ID == id)
                    return dk;
            }
            return null;
        }
       
        private Deck FindDeck(Deck deck)
        {
            return FindDeckByName(deck.Name);
        }

        private void UpdateGraphics()
        {
            _Layer.OwnerView.UpdateGraphics();
        }

        // 开仓
        public bool Start(Deck dk)
        {
            if (null == FindDeck(dk))
            {
                Utils.MB.Warning("该仓面不存在");
                return false;
            }
            if (!dk.IsVisible)
            {
                Utils.MB.Warning("该仓面现在不是可见仓面，无法开仓。请设置为可见仓面再试一次。");
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确认要开仓吗？\n仓面信息：" + dk.Name))
                return false;

            //             ThicknessMonitor(dk);

            // 0) 检查操作权限

            // 1) 仓面工作状态检查

            if (dk.WorkState== DeckWorkState.WORK)
            {
                Utils.MB.Warning("该仓面已经在工作中，请关仓后再试一次");
                return false;
            }
            // 2) 检查车辆安排情况

            DamLKK.DB.CarDistributeDAO daoCar = DamLKK.DB.CarDistributeDAO.GetInstance();
            List<Roller> info = daoCar.GetInusedCars();
            List<RollerDis> dist = daoCar.GetCarDisInDeck(unit.ID, elevation.Height, dk.ID);
            int distCount = 0;
            foreach (RollerDis cd in dist)
            {
                if (cd.IsFinished())
                    continue;
                distCount++;
                foreach (Roller inf in info)
                {
                    if (inf== null)
                        continue;
                    if (cd.RollerID == inf.ID)
                    {
                        Utils.MB.Warning("开仓失败：车辆已被占用：\"" + inf.Name + "\"");
                        return false;
                    }
                }
            }
            if (distCount == 0)
            {
                Utils.MB.Warning("开仓失败：尚未安排任何车辆");
                return false;
            }

            // 3) 更新数据库仓面项中的起始时间
            // 4) 更新车辆安排表项中的起始、结束时间

            DamLKK.DB.DeckDAO daoSeg = DamLKK.DB.DeckDAO.GetInstance();
            try
            {
                DeckVehicleResult result = daoSeg.StartDeck(unit.ID, elevation.Height, dk.ID, dk.MaxSpeed, dk.WorkState);
                if (result == DeckVehicleResult.CARS_FAIL)
                    Utils.MB.Warning("开仓失败：车辆错误");
                if (result == DeckVehicleResult.SEGMENT_FAIL)
                    Utils.MB.Warning("开仓失败：仓面错误");
                if (result != DeckVehicleResult.SUCCESS)
                    return false;
            }
            catch
            {
                return false;
            }

            VehicleControl.LoadCarDistribute();
            _Control.LayerControl.Instance.LoadWorkingLayer();
            LoadDB(dk);
            UpdateGraphics();
            GPSServer.OpenDeck();
            Utils.MB.OKI("\"" + dk.Name + "\"" + "已经开仓！");

            return true;
        }

        // 关仓
        public bool Stop(Deck dk)
        {
            if (null == FindDeck(dk))
            {
                Utils.MB.Warning("该仓面不存在");
                return false;
            }
            if (!dk.IsVisible)
            {
                Utils.MB.Warning("该仓面现在不是可见仓面，无法关仓。请设置为可见仓面再试一次。");
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确认要关仓吗？\n仓面信息：" + dk.Name))
                return false;

            // 0) 检查操作权限

            // 1) 仓面工作状态检查

            if (!(dk.WorkState==DeckWorkState.WORK))
            {
                Utils.MB.Warning("该仓面未开仓，请开仓后再试一次");
                return false;
            }
            // 2) 检查车辆安排情况

            // 3) 更新数据库仓面项中的起始时间
            // 4) 更新车辆安排表项中的起始、结束时间

            DB.DeckDAO dao = DB.DeckDAO.GetInstance();

            try
            {
                DeckVehicleResult result = dao.EndDeck(unit.ID, elevation.Height, dk.ID);
                if (result == DeckVehicleResult.CARS_FAIL)
                    Utils.MB.Warning("关仓失败：车辆错误");
                if (result == DeckVehicleResult.SEGMENT_FAIL)
                    Utils.MB.Warning("关仓失败：仓面错误");
                if (result != DeckVehicleResult.SUCCESS)
                    return false;
            }
            catch
            {
                return false;
            }

            _IsStoppingDeck = true;
            dk.VehicleControl.Clear();
            _Control.LayerControl.Instance.LoadWorkingLayer();
            VehicleControl.LoadCarDistribute();
            LoadDB(dk);

            GPSServer.CloseDeck();

            Utils.MB.OKI("\"" + dk.Name + "\"" + "关仓完毕！");

            return true;
        }

        volatile bool _IsStoppingDeck = false;

        /// <summary>
        /// 查看仓面车辆分配历史
        /// </summary>
        /// <param name="deck">要查看的仓面</param>
        /// <returns></returns>
        public bool LookVehicleHistory(Deck deck)
        {
            Forms.VehicleHistory dlg = new Forms.VehicleHistory();
            dlg.Deck = deck;
            dlg.ShowDialog();
            return true;
        }

        /// <summary>
        /// 修改激活仓面边界点
        /// </summary>
        public void ChangVertex(Deck deck)
        {
            DamLKK.Forms.ToolsWindow.GetInstance()._IsTempSelectDeck = true;
            DamLKK.Forms.ToolsWindow.GetInstance().btnDeckPoly_Click(null,null);
        }

        // 计算该仓面某点的碾压次数，按照屏幕坐标
        /// <summary>
        ///  // 计算该仓面某点的碾压次数，按照屏幕坐标
        /// </summary>
        public int[] RollCount(PointF pt)
        {
            int count = 0,countNo=0;
            foreach (Deck dk in _Decks)
            {
                int[] ct=dk.RollCount(pt);
                if(ct==null)
                    continue;
                count += ct[1];
                countNo += ct[0];
            }
            return new int[]{countNo,count};
        }
    }
}
