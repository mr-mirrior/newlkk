using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK.DB.datamap
{
    class BitAccess
    {
        public static  int ZERO = 0;
	public static  int ONE  = 1;
	
	public static  byte[]	BITS	= new byte[]{
		(byte)0x80,(byte)0x40,(byte)0x20,(byte)0x10,(byte)0x08,(byte)0x04,(byte)0x02,(byte)0x01
	};
	
	//public abstract byte[] TransformToBitData(File file, int columnSize, int rowSize);
	
	/**
	 * 1->true,0->false
	 * 
	 * @param data
	 * @param index
	 * @return
	 * @throws Exception 
	 */
	public static bool readBitFromByteArray(byte[] data, int index){
		if(data == null){
			throw new Exception("Null byte array");
		}
		if(index<0 || index>=data.Length*8){
			throw new Exception("Out of boundary");
		}
		int	position	= index/8;	//计算出是第几个字节
		int	offset		= index%8;	//计算出偏移量
		return (data[position]&BITS[offset])!=0 ? true : false;
	}
	
	
	public static int getBitInByte(byte byte1,int index) {
		if(index<0||index>=8){
			throw new Exception("out of boundary");
		}
		return (byte1&BITS[index]) != 0 ? ONE : ZERO;
	}
	
	/*
    public static String getString(byte b) throws Exception{
		StringBuffer ret = new StringBuffer();
		for(int i=0;i<8;i++){
			ret.append(getBitInByte(b,i));
		}
		return ret.toString();
	}*/
	
	/**
	 * C double 8 Bytes
	 * @param data
	 * @param offset
	 * @return
	 */
// 	public static double READ_C_double(byte[] data, int offset){
// 		long	_long	= 0;
// 		byte[]	tmp	= new byte[8];
// 		for(int i=0; i<8; i++){
// 			tmp[i]	= data[i+offset];
// 		}
// 		_long = tmp[0];
// 		_long &= 0xff;
// 		_long |= ((long) tmp[1] << 8);
// 		_long &= 0xffff;
// 		_long |= ((long) tmp[2] << 16);
// 		_long &= 0xffffff;
// 		_long |= ((long) tmp[3] << 24);
// 		_long &= 0xffffffffL;
// 		_long |= ((long) tmp[4] << 32);
// 		_long &= 0xffffffffffL;
// 		_long |= ((long) tmp[5] << 40);
// 		_long &= 0xffffffffffffL;
// 		_long |= ((long) tmp[6] << 48);
// 		_long &= 0xffffffffffffffL;
// 		_long |= ((long) tmp[7] << 56);
// 		return Double.longBitsToDouble(_long);
// 	}
	
	/**
	 * C float 4 Bytes
	 * @param data
	 * @param offset
	 * @return
	 */
	public static float READ_C_float(byte[] data, int offset){
		//int	_int	= 0;
		byte[]	tmp	= new byte[8];
		for(int i=0; i<4; i++){
			tmp[i]	= data[i+offset];
			/*try {
				System.out.print(getString(tmp[i])+" ");
			} catch (Exception e) {
				e.printStackTrace();
			}	*/		
		}		
		/*System.out.print("\n");*/		
// 		_int = tmp[0];
// 		_int &= 0xff;
// 		_int |= (tmp[1] << 8);
// 		_int &= 0xffff;
// 		_int |= (tmp[2] << 16);
// 		_int &= 0xffffff;
// 		_int |= (tmp[3] << 24);
        //rem by yzs ,can?
		return BitConverter.ToSingle(tmp,0);
	}
	
	/**
	 * C integer 4 Bytes
	 * @param data
	 * @param offset
	 * @return
	 */
	public static int READ_C_int(byte[] data, int offset){
		int	_int	= 0;
		byte[]	tmp	= new byte[4];
		for(int i=0; i<4; i++){
			tmp[i]	= data[i+offset];
		}
		_int = tmp[0];
		_int &= 0xff;
		_int |= (tmp[1] << 8);
		_int &= 0xffff;
		_int |= (tmp[2] << 16);
		_int &= 0xffffff;
		_int |= (tmp[3] << 24);
		return _int;
	}
	//C short 2 Bytes
	public static int READ_C_short(byte[] data, int offset){
		int	_int	= 0;
		byte[]	tmp	= new byte[2];
		for(int i=0; i<2; i++){
			tmp[i]	= data[i+offset];
		}
		_int = tmp[0];
		_int &= 0xff;
		_int |= (tmp[1] << 8);
		return _int;
	}
	
	//1 byte to int
	public static int READ_byte(byte[] data, int offset){
		int	_int	= 0;
		byte tmp = data[offset];
/*		try {
			System.out.print(getString(tmp)+" ");
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}*/
		_int = tmp;
		_int &= 0xff;
		
		
		return _int;
	}
    }
}
