using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DM.DB
{
    public class Segment
    {
        Int32 segmentID;
        double elevationValues_maxDesignz;

        public double ElevationValues_maxDesignz
        {
            get { return elevationValues_maxDesignz; }
            set { elevationValues_maxDesignz = value; }
        }
        double elevationValues_minDesignz;

        public double ElevationValues_minDesignz
        {
            get { return elevationValues_minDesignz; }
            set { elevationValues_minDesignz = value; }
        }
        public Segment() { }
        public bool IsEqual(Segment s)
        {
            return IsEqual(s.UnitID, s.DesignZ, s.SegmentID);
        }
        public bool IsEqual(int partitionID, double elevation, int sID)
        {
            return partitionID == this.UnitID && elevation == this.DesignZ && sID == this.SegmentID;
        }
        string elevationValues;

        public string ElevationValues
        {
            get { return elevationValues; }
            set { 
                elevationValues = value;
                if (ElevationValues!=null)
                {
                    string[] v = elevationValues.Split(',');
                    if (v.Length==2)
                    {
                        elevationValues_minDesignz = Convert.ToSingle(v[0]);
                        elevationValues_maxDesignz = Convert.ToSingle(v[1]);
                    }
                }
            }
        }
        Bitmap elevationImage;
        byte[] elevationImageBytes;

        public byte[] ElevationImageBytes
        {
            get {
                return elevationImageBytes; 
            }
            set { elevationImageBytes = value; }
        }

        public Bitmap ElevationImage
        {
            get { return elevationImage; }
            set { elevationImage = value;
            BitmapData data = elevationImage.LockBits(new Rectangle(0, 0, elevationImage.Width, elevationImage.Height), ImageLockMode.ReadWrite, elevationImage.PixelFormat);
            IntPtr ptr = data.Scan0;
            int tbytes = elevationImage.Width * elevationImage.Height * 4;
            elevationImageBytes = new byte[tbytes];     // Copy the RGB values into the array. 
            System.Runtime.InteropServices.Marshal.Copy(ptr, elevationImageBytes, 0, tbytes);     // Set every red value to 255. 
            elevationImage.UnlockBits(data);
            }
        } 
        byte[] datamap;

        public byte[] Datamap
        {
            get { return datamap; }
            set { datamap = value; }
        }
        public Int32 SegmentID
        {
            get { return segmentID; }
            set { segmentID = value; }
        }
        DamLKK._Model.DeckWorkState workState;

        public DamLKK._Model.DeckWorkState WorkState
        {
            get { return workState; }
            set { workState = value; }
        }
        Int32 unitID;

        public Int32 UnitID
        {
            get { return unitID; }
            set { unitID = value; }
        }
        string unitName;

        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }


        Double designZ;

        public Double DesignZ
        {
            get { return designZ; }
            set { designZ = value; }
        }
        string vertext;

        public string Vertext
        {
            get { return vertext; }
            set { vertext = value; }
        }

        DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        Double maxSpeed;

        public Double MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        Int32 designRollCount;

        public Int32 DesignRollCount
        {
            get { return designRollCount; }
            set { designRollCount = value; }
        }
        Double errorParam;

        public Double ErrorParam
        {
            get { return errorParam; }
            set { errorParam = value; }
        }
        Double spreadZ;

        public Double SpreadZ
        {
            get { return spreadZ; }
            set { spreadZ = value; }
        }
        Double designDepth;

        public Double DesignDepth
        {
            get { return designDepth; }
            set { designDepth = value; }
        }
        string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        string segmentName;

        public string SegmentName
        {
            get { return segmentName; }
            set { segmentName = value; }
        }

        Double startZ;

        public Double StartZ
        {
            get { return startZ; }
            set { startZ = value; }
        }

        Double pop;  // percent of pass
		
		public Double POP
		{
			get{return pop;}
			set{pop = value;}
		}

        string notRollingstring;

        public string NotRollingstring
        {
            get { return notRollingstring; }
            set { notRollingstring = value; }
        }

        string commentNRstring;  

        public string CommentNRstring
        {
          get { return commentNRstring; }
          set { commentNRstring = value; }
        }

        public bool IsWorking()
        {
            return WorkState == DamLKK._Model.DeckWorkState.WORK;
        }
        public bool IsFinished()
        {
            return WorkState == DamLKK._Model.DeckWorkState.END;
        }

        int librateState;

        public int LibrateState
        {
            get { return librateState; }
            set { librateState = value; }
        }
        //         public string getSegmentName()
        //         {
        //             return segmentName;
        //         }
        // 
        //         public void setSegmentName(string segmentName)
        //         {
        //             this.segmentName = segmentName;
        //         }
        // 
        //         public DateTime getDTEnd()
        //         {
        //             return endDate;
        //         }
        // 
        //         public DateTime getDTStart()
        //         {
        //             return startDate;
        //         }
        // 
        //         public Double getDesignZ()
        //         {
        //             return designZ;
        //         }
        //         public void setDesignZ(Double designZ)
        //         {
        //             this.designZ = designZ;
        //         }
        //         public Int32 getSegmentID()
        //         {
        //             return segmentID;
        //         }
        //         public void setSegmentID(Int32 segmentID)
        //         {
        //             this.segmentID = segmentID;
        //         }
        //         public Int32 getWorkState()
        //         {
        //             return workState;
        //         }
        //         public void setWorkState(Int32 workState)
        //         {
        //             this.workState = workState;
        //         }
        //         public Int32 getUnitID()
        //         {
        //             return unitID;
        //         }
        //         public void setUnitID(Int32 unitID)
        //         {
        //             this.unitID = unitID;
        //         }
        //         public Double getMaxSpeed()
        //         {
        //             return maxSpeed;
        //         }
        //         public void setMaxSpeed(Double maxSpeed)
        //         {
        //             this.maxSpeed = maxSpeed;
        //         }
        //         public Int32 getDesignRollCount()
        //         {
        //             return designRollCount;
        //         }
        //         public void setDesignRollCount(Int32 designRollCount)
        //         {
        //             this.designRollCount = designRollCount;
        //         }
        //         public Double getErrorParam()
        //         {
        //             return errorParam;
        //         }
        //         public void setErrorParam(Double errorParam)
        //         {
        //             this.errorParam = errorParam;
        //         }
        //         public Double getSpreadZ()
        //         {
        //             return spreadZ;
        //         }
        //         public void setSpreadZ(Double spreadZ)
        //         {
        //             this.spreadZ = spreadZ;
        //         }
        //         public Double getDesignDepth()
        //         {
        //             return designDepth;
        //         }
        //         public void setDesignDepth(Double designDepth)
        //         {
        //             this.designDepth = designDepth;
        //         }
        // 
        //         public DateTime getStartDate()
        //         {
        //             return startDate;
        //         }
        // 
        //         public void setStartDate(DateTime startDate)
        //         {
        //             this.startDate = startDate;
        //         }
        // 
        //         public DateTime getEndDate()
        //         {
        //             return endDate;
        //         }
        // 
        //         public void setEndDate(DateTime endDate)
        //         {
        //             this.endDate = endDate;
        //         }
        //         
        //         public string getRemark()
        //         {
        //             return remark;
        //         }
        // 
        //         public void setRemark(string remark)
        //         {
        //             this.remark = remark;
        //         }
    }
}
