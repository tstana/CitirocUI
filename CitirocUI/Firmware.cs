using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;

namespace CitirocUI
{
    class Firmware
    {
        static FTDI.FT_STATUS ftStatus;

        #region EPCS Operation Code
        private const byte AS_NOP = 0x00;
        private const byte AS_WRITE_ENABLE = 0x06;
        private const byte AS_WRITE_DISABLE = 0x04;
        private const byte AS_READ_STATUS = 0x05;
        private const byte AS_WRITE_STATUS = 0x01;
        private const byte AS_READ_BYTES = 0x03;
        private const byte AS_FAST_READ_BYTES = 0x0B;
        private const byte AS_PAGE_PROGRAM = 0x02;
        private const byte AS_ERASE_SECTOR = 0xD8;
        private const byte AS_ERASE_BULK = 0xC7;
        private const byte AS_READ_SILICON_ID = 0xAB;
        private const byte AS_CHECK_SILICON_ID = 0x9F;
        #endregion

        #region Pin Definition
        private const byte CONF_DONE = 0x80;
        private const byte ASDI = 0x40;
        private const byte DATAOUT = 0x10;
        private const byte NCE = 0x08;
        private const byte NCS = 0x04;
        private const byte NCONFIG = 0x02;
        private const byte DCLK = 0x01;
        private const byte CUR_DATA = 0x00;
        private const byte DEF_VALUE = 0x0E;
        #endregion

        private static string IntToBin(int value, int len) // To convert a value from integer to binary representation into a string
        {
            return (len > 1 ? IntToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        public static void sendWord(byte subAd, string word, int usbDevId)
        {
            Int32 wrByte;
            byte[] tempTx = new byte[1];
            tempTx[0] = Convert.ToByte(Convert.ToUInt32(word, 2));
            wrByte = USB.UsbWrt(usbDevId, subAd, tempTx, 1);
        }

        public static void sendWord(byte subAd, byte[] stream, int nbSend, int usbDevId)
        {
            USB.UsbWrt(usbDevId, subAd, stream, nbSend);
        }

        public static string readWord(byte subAd, int usbDevId)
        {
            byte[] tempRx = new byte[1];
            byte rdAdd = 0;
            unsafe
            {
                fixed (byte* arrayRd = tempRx)
                {
                    Int32 rdByte = USB.UsbRd(usbDevId, subAd, arrayRd, 1);
                }
                rdAdd = tempRx[0];
            }

            return IntToBin(Convert.ToInt32(rdAdd), 8);
        }

        public static byte[] readWord(byte subAd, int nbRead, int usbDevId)
        {
            byte[] rdAdd = new byte[nbRead];
            unsafe
            {
                fixed (byte* arrayRd = rdAdd)
                {
                    int rdByte = USB.UsbRd(usbDevId, subAd, arrayRd, nbRead);
                }
            }
            return rdAdd;
        }

        public static string strFtDataSetBit(byte signal, int data)
        {
            string strData = "";
            strData += (char)((byte)((byte)(signal * data)));
            return strData;
        }

        public static string strFtDataMsb(byte signal)
        {
            string strData = "";
            int bit = 0;
            byte lsb = byteReverse(signal);

            if (signal == AS_READ_SILICON_ID)
            {
                for (int i = 0; i < 8; i++)
                {
                    bit = lsb >> i;
                    bit = bit & 0x1;

                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | NCONFIG | (DCLK * 0)));
                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | NCONFIG | (DCLK * 1)));
                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | NCONFIG | (DCLK * 0)));
                }
            }
            else if (signal == AS_READ_BYTES ||
                     signal == AS_WRITE_ENABLE ||
                     signal == AS_ERASE_BULK ||
                     signal == AS_READ_STATUS ||
                     signal == AS_PAGE_PROGRAM)
            {
                for (int i = 0; i < 8; i++)
                {
                    bit = lsb >> i;
                    bit = bit & 0x1;

                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 1)));
                    strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
                }
            }
            else if (signal == AS_NOP)
            {
                for (int i = 0; i < 8; i++)
                {
                    strData += (char)((byte)(NCE | (DCLK * 0)));
                    strData += (char)((byte)(NCE | (DCLK * 1)));
                    strData += (char)((byte)(NCE | (DCLK * 0)));
                }
            }
            return strData;
        }

        public static string strFtDataLsb(byte signal)
        {
            string strData = "";
            int bit = 0;
            byte lsb = signal;

            for (int i = 0; i < 8; i++)
            {
                bit = lsb >> i;
                bit = bit & 0x1;

                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 1)));
                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
            }
            return strData;
        }

        public static string strFtDataEpcs(byte signal)
        {
            string strData = "";
            int bit = 0;
            byte lsb = byteReverse(signal);

            for (int i = 0; i < 8; i++)
            {
                bit = lsb >> i;
                bit = bit & 0x1;

                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 1)));
                strData += (char)((byte)((byte)(bit * ASDI) | NCE | (DCLK * 0)));
            }
            return strData;
        }

        private static FTDI.FT_STATUS ftBitWrite(byte signal, int data, FTDI myFtdiDevice)
        {
            byte mask = (byte)~signal;
            byte[] dataBuffer = new byte[1];
            byte pinState = 0x00;
            uint numByteWr = 0;
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;

            status = myFtdiDevice.GetPinStates(ref pinState);

            dataBuffer[0] = (byte)((pinState & mask) | (data * signal));
            status = myFtdiDevice.Write(dataBuffer, 1, ref numByteWr);
            return status;
        }

        private static FTDI.FT_STATUS ftBitRead(byte signal, ref int data, FTDI myFtdiDevice)
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            byte pinState = 0x00;
            byte[] rdBuf = new byte[1];

            status = myFtdiDevice.GetPinStates(ref pinState);

            if (status == FTDI.FT_STATUS.FT_OK)
                data = pinState & signal;

            return status;
        }

        public static FTDI.FT_STATUS ftByteRead(ref byte data, FTDI myFtdiDevice)
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            int bit = 0;
            int mask = 0x01;

            status = ftBitWrite(DCLK, 0, myFtdiDevice);
            if (status != FTDI.FT_STATUS.FT_OK) return status;

            for (int i = 0; i < 8; i++)
            {
                status = ftBitWrite(DCLK, 1, myFtdiDevice);
                if (status != FTDI.FT_STATUS.FT_OK) return status;
                status = ftBitRead(DATAOUT, ref bit, myFtdiDevice);
                if (bit != 0)
                    data |= (byte)(mask << i);

                status = ftBitWrite(DCLK, 0, myFtdiDevice);
                if (status != FTDI.FT_STATUS.FT_OK) return status;
            }
            return status;
        }

        public static byte byteReverse(byte inByte)
        {
            byte result = 0x00;
            byte mask = 0x00;

            for (mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                result >>= 1;
                byte tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00)
                    result |= 0x80;
            }
            return result;
        }

        public static int eraseEpcsDev(FTDI myFtdiDevice)
        {
            int status = 0;

            // Set FTDI to Bit-bang mode
            ftStatus = myFtdiDevice.SetBitMode(0x4f, 0x1); //0x4f -- 01001111
            ftStatus = myFtdiDevice.SetBaudRate(153600);

            // Reset PINs;
            uint numByteWr = 0;
            byte[] byteWr = new byte[1];
            byteWr[0] = DEF_VALUE;
            ftStatus = myFtdiDevice.Write(byteWr, 1, ref numByteWr);

            //Set NCS to 0
            string strFtWr = strFtDataSetBit(NCS, 0);

            // Send Write enable
            strFtWr += strFtDataMsb(AS_WRITE_ENABLE);

            // Set NCS to 1
            strFtWr += strFtDataSetBit(NCS, 1);

            //Set NCS to 0
            strFtWr += strFtDataSetBit(NCS, 0);

            // Send Erase bulk 
            strFtWr += strFtDataMsb(AS_ERASE_BULK);

            //Set NCS to 1
            strFtWr += strFtDataSetBit(NCS, 1);

            //Set NCS to 0
            strFtWr += strFtDataSetBit(NCS, 0);

            // Send Read status
            strFtWr += strFtDataMsb(AS_READ_STATUS);
            uint numFtWr = 0;
            ftStatus = myFtdiDevice.Write(strFtWr, strFtWr.Length, ref numFtWr);

            // Read status
            byte byteStatusReg = 0;
            ftStatus = ftByteRead(ref byteStatusReg, myFtdiDevice);
            byteStatusReg = byteReverse(byteStatusReg);

            while ((byteStatusReg & 0x01) == 1)
            {
                byteStatusReg = 0;
                ftStatus = ftByteRead(ref byteStatusReg, myFtdiDevice);
                byteStatusReg = byteReverse(byteStatusReg);
            }

            //Set NCS to 1
            ftStatus = myFtdiDevice.Write(strFtDataSetBit(NCS, 1), 1, ref numFtWr);
            
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                status = 0;
            else
                status = -1;

            return status;
        }
    }
}
