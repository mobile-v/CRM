using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace PartionnyAccount.Classes.Function
{
    public class DirQrCodeImage
    {
        //https://habr.com/sandbox/95109/
        //https://www.nuget.org/packages/ZXing.Net
        //Клас с параметрами для ШК
        public class BarSettings
        {
            //Формирование BarCode
            public int Width = 275;
            public int Height = 60;
            //public string BarcodeAlign = "center";
            //public int EncodeType = 5; //public string EncodeType = "EAN-13";
            //public bool GenerateLabel = true;
            //public string RotateFlip = "RotateNoneFlipNone";
            //public string LabelLocation = "BottomCenter";
            public string Data = "";
            //public Color ForeColor = Color.Black;
            //public Color BackColor = Color.White;

            //Формат сохранения BarCode
            public UchetOblakoBar.SaveTypes ImgType = UchetOblakoBar.SaveTypes.PNG;
            //Куда сохранить BarCode
            public string PathFile;
        }

        public string GetBarCodeImageLink(BarSettings barSettings, string BarCodeNumber, string iUsersID, int LocalGenID, string MapPath)
        {
            barSettings.PathFile = MapPath + @"UsersTemp\FileStock\bar_24" + iUsersID + "_" + LocalGenID + ".png";
            //errorProvider1.Clear();
            barSettings.Data = BarCodeNumber;
            int W = barSettings.Width;
            int H = barSettings.Height;

            QRCodeWriter qrEncode = new QRCodeWriter(); //создание QR кода
            //string strRUS = "Привет, мир";  //строка на русском языке

            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");   //добавление в коллекцию кодировки utf-8
            BitMatrix qrMatrix = qrEncode.encode(   //создание матрицы QR
                barSettings.Data,      //кодируемая строка
                BarcodeFormat.QR_CODE, //формат кода, т.к. используется QRCodeWriter применяется QR_CODE
                W,                     //ширина
                H,                     //высота
                hints);                //применение колекции поведений

            BarcodeWriter qrWrite = new BarcodeWriter();    //класс для кодирования QR в растровом файле
            qrWrite.Options.Width = W;
            qrWrite.Options.Height = H;
            Bitmap qrImage = qrWrite.Write(qrMatrix);   //создание изображения
            qrImage.Save(barSettings.PathFile, System.Drawing.Imaging.ImageFormat.Png);//сохранение изображения

            /*
            //Вставляем текст в Qr:
            RectangleF rectf = new RectangleF(70, 90, 90, 50);
            Graphics g = Graphics.FromImage(qrImage);
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(barSettings.Data, new Font("Tahoma", 8), Brushes.Black, rectf);
            g.Flush();
            qrImage.Save(barSettings.PathFile, System.Drawing.Imaging.ImageFormat.Png);//сохранение изображения
            */

            /*
            BarcodeReader qrDecode = new BarcodeReader(); //чтение QR кода
            Result text = qrDecode.Decode((Bitmap)Bitmap.FromFile("1.png")); //декодирование растрового изображения
            Console.WriteLine(text.Text);   //вывод результата
            */

            return "<img src='" + @"..\..\..\UsersTemp\FileStock\bar_24" + iUsersID + "_" + LocalGenID + ".png" + "' alt='" + BarCodeNumber + "' >"; //width='35%' height='35%'

        }

    }
}