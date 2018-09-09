using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
//using System.Windows.Forms;
using System.Windows.Forms;

namespace PartionnyAccount.Classes.Function
{
    public class DirBarCodeImageNew
    {
        //http://www.codeproject.com/Articles/20823/Barcode-Image-Generation-Library
        //Клас с параметрами для ШК
        public class BarSettings
        {
            //Формирование BarCode
            public int Width = 275;
            public int Height = 60;
            public string BarcodeAlign = "center";
            public int EncodeType = 5; //public string EncodeType = "EAN-13";
            public bool GenerateLabel = true;
            public string RotateFlip = "RotateNoneFlipNone";
            public string LabelLocation = "BottomCenter";
            public string Data = "";
            public Color ForeColor = Color.Black;
            public Color BackColor = Color.White;

            //Формат сохранения BarCode
            public UchetOblakoBar.SaveTypes ImgType = UchetOblakoBar.SaveTypes.PNG;
            //Куда сохранить BarCode
            public string PathFile;
        }
        public string GetBarCodeImageLink(BarSettings barSettings, string BarCodeNumber, string iUsersID, int LocalGenID, string MapPath)
        {
            barSettings.Data = BarCodeNumber;

            //barSettings.PathFile = System.Web.HttpContext.Current.Server.MapPath("~/") + @"UsersTemp\FileStock\bar_24" + iUsersID + "_" + LocalGenID + ".png";
            barSettings.PathFile = MapPath + @"UsersTemp\FileStock\bar_24" + iUsersID + "_" + LocalGenID + ".png";

            UchetOblakoBar.Barcode b = new UchetOblakoBar.Barcode();
            GroupBox barcode = new GroupBox();


            //errorProvider1.Clear();
            int W = barSettings.Width; //this.txtWidth
            int H = barSettings.Height; //this.txtHeight
            b.Alignment = UchetOblakoBar.AlignmentPositions.CENTER;

            //barcode alignment
            switch (barSettings.BarcodeAlign.Trim().ToLower())
            {
                case "left": b.Alignment = UchetOblakoBar.AlignmentPositions.LEFT; break;
                case "right": b.Alignment = UchetOblakoBar.AlignmentPositions.RIGHT; break;
                default: b.Alignment = UchetOblakoBar.AlignmentPositions.CENTER; break;
            }//switch

            UchetOblakoBar.TYPE type = UchetOblakoBar.TYPE.UNSPECIFIED;
            switch (barSettings.EncodeType) //switch (barSettings.EncodeType.Trim())
            {
                case 1: type = UchetOblakoBar.TYPE.UPCA; break; //"UPC-A"
                case 2: type = UchetOblakoBar.TYPE.UPCE; break; //"UPC-E"
                case 3: type = UchetOblakoBar.TYPE.UPC_SUPPLEMENTAL_2DIGIT; break; //"UPC 2 Digit Ext."
                case 4: type = UchetOblakoBar.TYPE.UPC_SUPPLEMENTAL_5DIGIT; break; //"UPC 5 Digit Ext."
                case 5: type = UchetOblakoBar.TYPE.EAN13; break; //"EAN-13"
                case 6: type = UchetOblakoBar.TYPE.JAN13; break; //"JAN-13"
                case 7: type = UchetOblakoBar.TYPE.EAN8; break; //"EAN-8"
                case 8: type = UchetOblakoBar.TYPE.ITF14; break; //"ITF-14"
                case 9: type = UchetOblakoBar.TYPE.Codabar; break; //"Codabar"
                case 10: type = UchetOblakoBar.TYPE.PostNet; break; //"PostNet"
                case 11: type = UchetOblakoBar.TYPE.BOOKLAND; break; //"Bookland/ISBN"
                case 12: type = UchetOblakoBar.TYPE.CODE11; break; //"Code 11"
                case 13: type = UchetOblakoBar.TYPE.CODE39; break; //"Code 39"
                case 14: type = UchetOblakoBar.TYPE.CODE39Extended; break; //"Code 39 Extended"
                case 15: type = UchetOblakoBar.TYPE.CODE39_Mod43; break; //"Code 39 Mod 43"
                case 16: type = UchetOblakoBar.TYPE.CODE93; break; //"Code 93"
                case 17: type = UchetOblakoBar.TYPE.LOGMARS; break; //"LOGMARS"
                case 18: type = UchetOblakoBar.TYPE.MSI_Mod10; break; //"MSI"
                case 19: type = UchetOblakoBar.TYPE.Interleaved2of5; break; //"Interleaved 2 of 5"
                case 20: type = UchetOblakoBar.TYPE.Standard2of5; break; //"Standard 2 of 5"
                case 21: type = UchetOblakoBar.TYPE.CODE128; break; //"Code 128"
                case 22: type = UchetOblakoBar.TYPE.CODE128A; break; //"Code 128-A"
                case 23: type = UchetOblakoBar.TYPE.CODE128B; break; //"Code 128-B"
                case 24: type = UchetOblakoBar.TYPE.CODE128C; break; //"Code 128-C"
                case 25: type = UchetOblakoBar.TYPE.TELEPEN; break; //"Telepen"
                case 26: type = UchetOblakoBar.TYPE.FIM; break; //"FIM"
                case 27: type = UchetOblakoBar.TYPE.PHARMACODE; break; //"Pharmacode"
                    //default: MessageBox.Show("Please specify the encoding type."); break;
            }//switch


            if (type != UchetOblakoBar.TYPE.UNSPECIFIED)
            {
                b.IncludeLabel = barSettings.GenerateLabel; //this.chkGenerateLabel.Checked
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), barSettings.RotateFlip.ToString(), true); //this.cbRotateFlip.SelectedItem.ToString()

                //label alignment and position
                switch (barSettings.LabelLocation.ToString().Trim().ToUpper())
                {
                    case "BOTTOMLEFT": b.LabelPosition = UchetOblakoBar.LabelPositions.BOTTOMLEFT; break;
                    case "BOTTOMRIGHT": b.LabelPosition = UchetOblakoBar.LabelPositions.BOTTOMRIGHT; break;
                    case "TOPCENTER": b.LabelPosition = UchetOblakoBar.LabelPositions.TOPCENTER; break;
                    case "TOPLEFT": b.LabelPosition = UchetOblakoBar.LabelPositions.TOPLEFT; break;
                    case "TOPRIGHT": b.LabelPosition = UchetOblakoBar.LabelPositions.TOPRIGHT; break;
                    default: b.LabelPosition = UchetOblakoBar.LabelPositions.BOTTOMCENTER; break;
                }//switch

                //===== Encoding performed here =====
                barcode.BackgroundImage = b.Encode(type, barSettings.Data.Trim(), barSettings.ForeColor, barSettings.BackColor, W, H); //this.txtData.Text.Trim() //this.btnForeColor.BackColor //this.btnBackColor.BackColor
                //===================================
            }//if

            //reposition the barcode image to the middle
            barcode.Location = new Point((barcode.Location.X + barcode.Width / 2) - barcode.Width / 2, (barcode.Location.Y + barcode.Height / 2) - barcode.Height / 2);


            //Сохранение в файл
            //SaveImg(b);
            b.SaveImage(barSettings.PathFile, barSettings.ImgType);


            return "<img src='" + @"..\..\..\UsersTemp\FileStock\bar_24" + iUsersID + "_" + LocalGenID + ".png" + "' alt='" + BarCodeNumber + "' >"; //width='35%' height='35%'
        }



        // *** *** *** Контрольный разряд *** *** ***
        public string BarCodes_CheckDigit(string Bar)
        {
            string _Bar = Bar;

            if (Bar.Length == 12) _Bar = BarCodes_CheckDigit_Ean13(Bar);
            else if (Bar.Length == 7) _Bar = BarCodes_CheckDigit_Ean8(Bar);

            return _Bar;
        }

        //BCGean13: 
        //d1 + 3×d2 + d3 + 3×d4 + ... + d11 + 3×d12 + d13 = 0 (mod 10).
        //Example:
        //8 9 0 4 0 0 0 2 1 0 0 3
        //8*1 + 9*3 + 0*1 + 4*3 + 0*1 + 0*3 + 0*1 + 2*3 + 1*1 + 0*3 + 0*1 + 3*3
        //8 + 27 + 0 + 12 + 0 + 0 + 0 + 6 + 1 + 0 + 0 + 9 = 63
        //We must add 7 to make 63 evenly divisible by 10 (63 + 7 = 70), therefore the check digit is 7.
        public string BarCodes_CheckDigit_Ean13(string Bar12)
        {
            string Bar13 = Bar12;

            double iBar =
                Convert.ToInt32(Bar12[0].ToString()) + 3 * Convert.ToInt32(Bar12[1].ToString()) + Convert.ToInt32(Bar12[2].ToString()) + 3 * Convert.ToInt32(Bar12[3].ToString()) +
                Convert.ToInt32(Bar12[4].ToString()) + 3 * Convert.ToInt32(Bar12[5].ToString()) + Convert.ToInt32(Bar12[6].ToString()) + 3 * Convert.ToInt32(Bar12[7].ToString()) +
                Convert.ToInt32(Bar12[8].ToString()) + 3 * Convert.ToInt32(Bar12[9].ToString()) + Convert.ToInt32(Bar12[10].ToString()) + 3 * Convert.ToInt32(Bar12[11].ToString());

            for (int i = 0; i < 10; i++)
            {
                double iBar_i = iBar + i;
                if (iBar_i % 10 == 0) { Bar13 += i.ToString(); break; }
            }

            return Bar13;
        }

        //BCGean8: 
        //3×d1 + d2 + 3×d3 + d4 + ... + 3×d7 = 0 (mod 10).
        //Example:
        //5	5 1	2 3 4 5
        //5*3 + 5*1 + 1*3 + 2*1 + 3*3 + 4*1 + 5*3
        //15 + 5 + 3 + 2 + 9 + 4 + 15 = 53
        //We must add 7 to make 53 evenly divisible by 10 (53 + 7 = 70), therefore the check digit is 7.
        public string BarCodes_CheckDigit_Ean8(string Bar7)
        {
            string Bar8 = Bar7;

            double iBar =
                3 * Convert.ToInt32(Bar7[0].ToString()) + Convert.ToInt32(Bar7[1].ToString()) + 3 * Convert.ToInt32(Bar7[2].ToString()) + Convert.ToInt32(Bar7[3].ToString()) +
                3 * Convert.ToInt32(Bar7[4].ToString()) + Convert.ToInt32(Bar7[5].ToString()) + 3 * Convert.ToInt32(Bar7[6].ToString());

            for (int i = 0; i < 10; i++)
            {
                double iBar_i = iBar + i;
                if (iBar_i % 10 == 0) { Bar7 += i.ToString(); break; }
            }

            return Bar8;
        }
    }
}