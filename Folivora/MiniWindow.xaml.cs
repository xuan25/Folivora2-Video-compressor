using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Folivora
{
    /// <summary>
    /// MiniWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public delegate void HideMini();
    public partial class MiniWindow : Window
    {
        public MiniWindow()
        {
            InitializeComponent();
            //Dispatcher.Invoke(this.Hide);
            this.Topmost = true;
            //this.ShowInTaskbar = false;
            this.Left = SystemParameters.WorkArea.Size.Width - 90;
            this.Top = SystemParameters.WorkArea.Size.Height - 90;
        }

        public void Setting(Brush Color, BitmapImage Image)
        {
            Point BottomRight = new Point(this.Left + this.Width, this.Top + this.Height);

            ImageBox.Source = Image;
            int iWidth = Image.PixelWidth;
            int iHeight = Image.PixelHeight;

            this.Width = iWidth;
            this.Height = iHeight;
            this.Left = BottomRight.X - iWidth;
            this.Top = BottomRight.Y - iHeight;

            FillPathRight.Fill = Color;
            FillPathLeft.Fill = Color;
            GlowPathRight.Fill = Color;
            GlowPathLeft.Fill = Color;
        }

        public void SetMask(ImageSource BI)
        {
            MaskBox.Source = BI;
        }

        public void ShowProgress(double per)
        {
            //定义变量
            Point endPoint = new Point();
            double x, y, angle;
            if (per > 100)
            {
                per = 100;
            }
            if (per < 0)
            {
                per = 0;
            }
            //换算角度
            angle = (per / 100) * (2 * Math.PI);
            //换算节点坐标
            x = Math.Cos(angle - Math.PI / 2);
            y = Math.Sin(angle - Math.PI / 2);
            //判断角度范围
            if (angle <= Math.PI)
            {
                //半圆1
                endPoint.X = x * 25 + 25;
                endPoint.Y = y * 25 + 25;
                FillArcRight.Point = endPoint;
                GlowArcRight.Point = endPoint;
                //半圆2
                endPoint.X = 25;
                endPoint.Y = 50;
                FillArcLeft.Point = endPoint;
                GlowArcLeft.Point = endPoint;
            }
            else
            {
                //半圆1
                endPoint.X = 25;
                endPoint.Y = 50;
                FillArcRight.Point = endPoint;  //颜色图层
                GlowArcRight.Point = endPoint;  //辉光图层
                //半圆2
                endPoint.X = x * 25 + 25;
                endPoint.Y = y * 25 + 25;
                FillArcLeft.Point = endPoint;  //颜色图层
                GlowArcLeft.Point = endPoint;  //辉光图层
            }
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public event HideMini MiniHideEvent;
        private void WindowChange(object sender, MouseButtonEventArgs e)
        {
            MiniHideEvent();
        }
    }
}
