using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Folivora
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //定义API函数
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        const uint ES_CONTINUOUS = 0x80000000;

        //参数的保存与读取
        string iniPath = Environment.CurrentDirectory + "\\config.ini";
        private void GetIniValue()
        {
            if (!File.Exists(iniPath))
            {
                return;
            }
            StreamReader SW = new StreamReader(iniPath, Encoding.Default, false);
            string iniContent = SW.ReadToEnd();

            FileBox.Text = FindKey(iniContent, "FileBox");
            IndexerCheck.IsChecked = StringToBool(FindKey(iniContent, "IndexerCheck"));
            DemuxerCombo.SelectedIndex = StringToInt(FindKey(iniContent, "DemuxerCombo"));
            CropCheck.IsEnabled = StringToBool(FindKey(iniContent, "CropCheck"));
            CropTBox.Text = FindKey(iniContent, "CropTBox");
            CropLBox.Text = FindKey(iniContent, "CropLBox");
            CropBBox.Text = FindKey(iniContent, "CropBBox");
            CropRBox.Text = FindKey(iniContent, "CropRBox");
            ResizeCheck.IsChecked = StringToBool(FindKey(iniContent, "ResizeCheck"));
            ResizeCombo.SelectedIndex = StringToInt(FindKey(iniContent, "ResizeCombo"));
            ResizeWBox.Text = FindKey(iniContent, "ResizeWBox");
            ResizeHBox.Text = FindKey(iniContent, "ResizeHBox");
            FpsCheck.IsChecked = StringToBool(FindKey(iniContent, "FpsCheck"));
            FpsCombo.SelectedIndex = StringToInt(FindKey(iniContent, "FpsCombo"));
            FpsBox.Text = FindKey(iniContent, "FpsBox");
            SubCheck.IsChecked = StringToBool(FindKey(iniContent, "SubCheck"));
            SubBox.Text = FindKey(iniContent, "SubBox");
            DeinterlaceCheck.IsChecked = StringToBool(FindKey(iniContent, "DeinterlaceCheck"));
            DenoiseCheck.IsChecked = StringToBool(FindKey(iniContent, "DenoiseCheck"));
            X264Combo.SelectedIndex = StringToInt(FindKey(iniContent, "X264Combo"));
            AppendCommandVBox.Text = FindKey(iniContent, "AppendCommandVBox");
            X264RBox.Text = FindKey(iniContent, "X264RBox");
            X264QBox.Text = FindKey(iniContent, "X264QBox");
            QaacCombo.SelectedIndex = StringToInt(FindKey(iniContent, "QaacCombo"));
            AppendCommandABox.Text = FindKey(iniContent, "AppendCommandABox");
            QaacRBox.Text = FindKey(iniContent, "QaacRBox");
            QaacQBox.Text = FindKey(iniContent, "QaacQBox");
            ExternalAudioCheck.IsChecked = StringToBool(FindKey(iniContent, "ExternalAudioCheck"));
            ExternalAudioBox.Text = FindKey(iniContent, "ExternalAudioBox");

            SW.Close();
        }

        private bool StringToBool(string key)
        {
            if(key == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int StringToInt(string key)
        {
            int i = 0;
            int.TryParse(key, out i);
            return i;
        }

        private string FindKey(string iniContent, string key)
        {
            int start = iniContent.IndexOf(key);
            if (start != -1)
            {
                int end = iniContent.IndexOf("\n", start + 1);
                if (end != -1)
                {
                    string line = iniContent.Substring(start, end - start - 1);
                    int mark = line.IndexOf("=");
                    return line.Substring(mark + 1).Trim();
                }
                else
                {
                    string line = iniContent.Substring(start);
                    int mark = line.IndexOf("=");
                    return line.Substring(mark + 1).Trim();
                }
            }
            else
            {
                return "";
            }
        }

        private void WriteIniValue()
        {
            StreamWriter SW = new StreamWriter(iniPath, false, Encoding.Default);

            SW.WriteLine("FileBox=" + FileBox.Text);
            SW.WriteLine("IndexerCheck=" + BoolToInt(IndexerCheck.IsChecked));
            SW.WriteLine("DemuxerCombo=" + DemuxerCombo.SelectedIndex);
            SW.WriteLine("CropCheck=" + BoolToInt(CropCheck.IsChecked));
            SW.WriteLine("CropTBox=" + CropTBox.Text);
            SW.WriteLine("CropLBox=" + CropLBox.Text);
            SW.WriteLine("CropBBox=" + CropBBox.Text);
            SW.WriteLine("CropRBox=" + CropRBox.Text);
            SW.WriteLine("ResizeCheck=" + BoolToInt(ResizeCheck.IsChecked));
            SW.WriteLine("ResizeCombo=" + ResizeCombo.SelectedIndex);
            SW.WriteLine("ResizeWBox=" + ResizeWBox.Text);
            SW.WriteLine("ResizeHBox=" + ResizeHBox.Text);
            SW.WriteLine("FpsCheck=" + BoolToInt(FpsCheck.IsChecked));
            SW.WriteLine("FpsCombo=" + FpsCombo.SelectedIndex);
            SW.WriteLine("FpsBox=" + FpsBox.Text);
            SW.WriteLine("SubCheck=" + BoolToInt(SubCheck.IsChecked));
            SW.WriteLine("SubBox=" + SubBox.Text);
            SW.WriteLine("DeinterlaceCheck=" + BoolToInt(DeinterlaceCheck.IsChecked));
            SW.WriteLine("DenoiseCheck=" + BoolToInt(DenoiseCheck.IsChecked));
            SW.WriteLine("X264Combo=" + X264Combo.SelectedIndex);
            SW.WriteLine("AppendCommandVBox=" + AppendCommandVBox.Text);
            SW.WriteLine("X264RBox=" + X264RBox.Text);
            SW.WriteLine("X264QBox=" + X264QBox.Text);
            SW.WriteLine("QaacCombo=" + QaacCombo.SelectedIndex);
            SW.WriteLine("AppendCommandABox=" + AppendCommandABox.Text);
            SW.WriteLine("QaacRBox=" + QaacRBox.Text);
            SW.WriteLine("QaacQBox=" + QaacQBox.Text);
            SW.WriteLine("ExternalAudioCheck=" + BoolToInt(ExternalAudioCheck.IsChecked));
            SW.WriteLine("ExternalAudioBox=" + ExternalAudioBox.Text);

            SW.Close();
        }

        private int BoolToInt(bool? key)
        {
            if (key == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //初始化
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SettingGrid.Visibility = Visibility.Visible;
            ProcessingGrid.Visibility = Visibility.Hidden;
            LoadTheme();

            DemuxerCombo.SelectedIndex = 0;
            ResizeCombo.SelectedIndex = 5;
            FpsCombo.SelectedIndex = 3;
            X264Combo.SelectedIndex = 11;
            QaacCombo.SelectedIndex = 3;

            Thread T = new Thread(delegate ()
            {
                for (double i = 0; i < 1.2; i = i + 0.01)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        PointA.Offset = i - 0.2;
                        PointB.Offset = i;
                    }));
                    Thread.Sleep(4);
                }
            });
            T.Start();

            GetIniValue();
        }

        //UI外观
        private void Window_Activated(object sender, EventArgs e)
        {
            Border_A.Visibility = Visibility.Visible;
            Border_B.Visibility = Visibility.Visible;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Border_A.Visibility = Visibility.Hidden;
            Border_B.Visibility = Visibility.Hidden;
        }

        private void LoadTheme()
        {
            StreamReader ThemeColor = new StreamReader(Environment.CurrentDirectory + "\\Theme\\ThemeColor.ini");
            string ColorINI = ThemeColor.ReadToEnd();
            BrushConverter brushConverter = new BrushConverter();
            if(GetParameterFormINI("MainColor", ColorINI) != null)
            {
                try
                {
                    Blur0.Fill = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MainColor", ColorINI));
                    Blur1.Fill = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MainColor", ColorINI));
                    Blur2.Fill = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MainColor", ColorINI));
                    Blur3.Fill = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MainColor", ColorINI));
                    Blur4.Fill = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MainColor", ColorINI));
                }
                catch { }
            }
            if (GetParameterFormINI("BorderColor", ColorINI) != null)
            {
                try
                {
                    Border_A.BorderBrush = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("BorderColor", ColorINI));
                    Border_B.Background = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("BorderColor", ColorINI));
                }
                catch { }
            }
            if (GetParameterFormINI("BackGround", ColorINI) != null)
            {
                try
                {
                    MainGrid.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("BackGround", ColorINI))) };
                }
                catch { }
            }
            if (GetParameterFormINI("MaskImage", ColorINI) != null)
            {
                try
                {
                    MaskBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MaskImage", ColorINI)));
                }
                catch { }
            }
        }

        private string GetParameterFormINI(string Key, string INI)
        {
            string result = null;
            try
            {
                result = INI.Substring(INI.IndexOf(Key) + Key.Length, INI.IndexOf("\r\n", INI.IndexOf(Key)) - INI.IndexOf(Key) - Key.Length).Replace(" ", "").Replace("=", "");
            }
            catch { }
            return result;
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //关闭
        private void ExitBut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (ProcessingGrid.Visibility == Visibility.Visible)
            {
                if (MessageBox.Show("目前尚有任务在进行，是否终止任务并退出程序？", "任务进行中...", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    AbortCoding(null, null);
                }
                else
                {
                    return;
                }
            }
            WriteIniValue();

            Thread T = new Thread(delegate ()
            {
                for (double i = 0; i < 1.2; i = i + 0.01)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        PointA.Offset = i;
                        PointB.Offset = i - 0.2;
                    }));
                    Thread.Sleep(4);
                }
                Environment.Exit(0);
            });
            T.Start();
        }

        //UI功能
        private void X264Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(X264Combo.SelectedIndex == 0 | X264Combo.SelectedIndex == 1)
            {
                X264RBox.IsEnabled = false;
                X264QBox.IsEnabled = false;

            }
            else if(X264Combo.SelectedIndex == 3 | X264Combo.SelectedIndex == 11)
            {
                X264RBox.IsEnabled = false;
                X264QBox.IsEnabled = true;
            }
            else
            {
                X264RBox.IsEnabled = true;
                X264QBox.IsEnabled = false;
            }
        }

        private void QaacCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QaacCombo.SelectedIndex == 0 | QaacCombo.SelectedIndex == 1)
            {
                QaacRBox.IsEnabled = false;
                QaacQBox.IsEnabled = false;

            }
            else if (QaacCombo.SelectedIndex == 2)
            {
                QaacRBox.IsEnabled = false;
                QaacQBox.IsEnabled = true;
            }
            else
            {
                QaacRBox.IsEnabled = true;
                QaacQBox.IsEnabled = false;
            }
        }

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            //选择文件参数设置
            OFD.Title = "请选择视频文件";
            OFD.Filter = "所有文件 (*.*)|*.*";
            OFD.RestoreDirectory = true;//是否打开上次目录
            //获取选中目录
            if (OFD.ShowDialog() == true)
            {
                FileBox.Clear();
                FileBox.Text = OFD.FileName;
            }
        }

        private void BrowseSub(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            //选择文件参数设置
            OFD.Title = "请选择视频文件";
            OFD.Filter = "所有文件 (*.*)|*.*";
            OFD.RestoreDirectory = true;//是否打开上次目录
            //获取选中目录
            if (OFD.ShowDialog() == true)
            {
                SubBox.Clear();
                SubBox.Text = OFD.FileName;
            }
        }

        private void BrowseAudio(object sender, RoutedEventArgs e)
        {
            if (ExternalAudioCheck.IsChecked == true)
            {
                OpenFileDialog OFD = new OpenFileDialog();
                //选择文件参数设置
                OFD.Title = "请选择视频文件";
                OFD.Filter = "所有文件 (*.*)|*.*";
                OFD.RestoreDirectory = true;//是否打开上次目录
                //获取选择目录
                if (OFD.ShowDialog() == true)
                {
                    ExternalAudioBox.Clear();
                    ExternalAudioBox.Text = OFD.FileName;
                }
                else
                {
                    ExternalAudioBox.Clear();
                    ExternalAudioCheck.IsChecked = false;
                }
            }
            else
            {
                ExternalAudioBox.Clear();
            }
        }

        private void Box_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void Box_PreviewDrop(object sender, DragEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            TB.Text = path;
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);
            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                int num = 0;
                if (!Int32.TryParse(TB.Text, out num))
                {
                    TB.Text = TB.Text.Remove(offset, change[0].AddedLength);
                    TB.Select(offset, 0);
                }
            }
        }

        //准备编码
        List<string> TempList;
        Thread CodingThread;
        MiniWindow MiniWin;
        private void Start(object sender, RoutedEventArgs e)
        {
            TempList = new List<string>();
            if (!File.Exists(FileBox.Text))
            {
                MessageBox.Show("未找到文件");
                return;
            }

            string Apath = FileBox.Text;
            string Vpath = FileBox.Text;
            if (IndexerCheck.IsChecked == true)
            {
                ReturnBox.AppendText("\r\n--------    USE DEMUXER    --------");
                ProgressMark = "--------    USE DEMUXER    --------";
                if (QaacCombo.SelectedIndex > 1)
                {
                    if (ExternalAudioCheck.IsChecked == true)
                    {
                        if (!File.Exists(ExternalAudioBox.Text))
                        {
                            MessageBox.Show("未找到外部音频");
                            return;
                        }
                        TempList.Add(Apath + "_A.avs");
                        MakeAVSA(ExternalAudioBox.Text);
                        Apath = ExternalAudioBox.Text;
                        Apath = Apath + "_A.avs";
                    }
                    else
                    {
                        TempList.Add(Apath + "_A.avs");
                        MakeAVSA(FileBox.Text);
                        Apath = Apath + "_A.avs";
                    }
                }
                if (X264Combo.SelectedIndex > 1)
                {
                    TempList.Add(Vpath + "_V.avs");
                    MakeAVSV(FileBox.Text);
                    Vpath = Vpath + "_V.avs";
                }
            }
            else
            {
                ProgressMark = "";
            }

            if (!File.Exists(Apath) || !File.Exists(Vpath))
            {
                MessageBox.Show("索引生成失败");
                return;
            }
            CodingThread = new Thread(StartCoding);
            CodingParameter CodingPara = new CodingParameter(FileBox.Text ,Apath, Vpath, QaacCombo.SelectedIndex, QaacRBox.Text, QaacQBox.Text, X264Combo.SelectedIndex, X264RBox.Text, X264QBox.Text, IndexerCheck.IsChecked, AppendCommandABox.Text, AppendCommandVBox.Text);
            if (ExternalAudioCheck.IsChecked == true)
            {
                CodingPara.Apath = ExternalAudioBox.Text;
            }

            MiniWin = new MiniWindow();
            MiniWin.MiniHideEvent -= new HideMini(DetailWindow);
            MiniWin.MiniHideEvent += new HideMini(DetailWindow);
            MiniWin.SetMask(MaskBox.Source);
            this.Hide();

            CodingThread.Start(CodingPara);
        }

        //编码参数类
        class CodingParameter
        {
            public CodingParameter(string InFile, string Apath, string Vpath, int QaacIndex, string QaacR, string QaacQ, int X264Index, string X264R, string X264Q, bool? IndexerChecked, string AppendCommandA, string AppendCommandV)
            {
                try
                {
                    this.InFileWithOutEXName = InFile.Substring(0, InFile.LastIndexOf("."));
                }
                catch
                {
                    this.InFileWithOutEXName = InFile;
                }
                this.Apath = Apath;
                this.Vpath = Vpath;
                this.QaacIndex = QaacIndex;
                this.QaacR = QaacR;
                this.QaacQ = QaacQ;
                this.X264Index = X264Index;
                this.X264R = X264R;
                this.X264Q = X264Q;
                this.IndexerChecked = IndexerChecked == true;
                this.AppendCommandA = AppendCommandA;
                this.AppendCommandV = AppendCommandV;
            }
            public string InFileWithOutEXName;
            public string Apath;
            public string Vpath;
            public int QaacIndex;
            public string QaacR;
            public string QaacQ;
            public int X264Index;
            public string X264R;
            public string X264Q;
            public bool IndexerChecked;
            public string AppendCommandA;
            public string AppendCommandV;
        }

        private void MakeAVSA(string path)
        {
            try
            {
                StreamWriter SW = new StreamWriter(path + "_A.avs", false, System.Text.Encoding.Default);
                //写入索引器
                switch (DemuxerCombo.SelectedIndex)
                {
                    case 0:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\FFMS2.dll\")");
                        SW.WriteLine("FFAudioSource(\"" + path + "\")");
                        break;
                    case 1:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\LSMASHSource.dll\")");
                        SW.WriteLine("LWLibavAudioSource(\"" + path + "\")");
                        break;
                    case 2:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\DirectShowSource.dll\")");
                        SW.WriteLine("DirectShowSource(\"" + path + "\", video=False)");
                        break;
                }
                //写入帧率
                if (FpsCheck.IsChecked == true)
                {
                    if (FpsBox.Text != "")
                    {
                        switch (FpsCombo.SelectedIndex)
                        {
                            case 0:
                                SW.WriteLine("AssumeFPS(" + FpsBox.Text + ", 1, false)");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("帧率：请输入数值");
                        return;
                    }
                }
                SW.Flush();
                SW.Close();
            }
            catch
            {
                MessageBox.Show("不能生成avs脚本：文件被占用（请重试）");
                return;
            }
        }

        private void MakeAVSV(string path)
        {
            try
            {
                StreamWriter SW = new StreamWriter(path + "_V.avs", false, System.Text.Encoding.Default);
                //写入索引器
                switch (DemuxerCombo.SelectedIndex)
                {
                    case 0:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\FFMS2.dll\")");
                        SW.WriteLine("FFVideoSource(\"" + path + "\")");
                        break;
                    case 1:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\LSMASHSource.dll\")");
                        SW.WriteLine("LWLibavVideoSource(\"" + path + "\")");
                        break;
                    case 2:
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\DirectShowSource.dll\")");
                        SW.WriteLine("DirectShowSource(\"" + path + "\", audio=False)");
                        break;
                }
                //色彩空间处理
                SW.WriteLine("ConvertToYUY2()");
                //写入反交错
                if (DeinterlaceCheck.IsChecked == true)
                {
                    SW.WriteLine("Bob()");
                }

                //写入裁剪
                if (CropCheck.IsChecked == true)
                {
                    if (CropTBox.Text != "" && CropLBox.Text != "" && CropBBox.Text != "" && CropRBox.Text != "")
                    {
                        SW.WriteLine("Crop(" + CropLBox.Text + "," + CropTBox.Text + ",-" + CropRBox.Text + ",-" + CropBBox.Text + ")");
                    }
                    else
                    {
                        MessageBox.Show("裁剪：请输入数值");
                        return;
                    }
                }
                //写入缩放
                if (ResizeCheck.IsChecked == true)
                {
                    if (ResizeWBox.Text != "" && ResizeHBox.Text != "")
                    {
                        switch (ResizeCombo.SelectedIndex)
                        {
                            case 0:
                                SW.WriteLine("BilinearResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 1:
                                SW.WriteLine("BicubicResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 2:
                                SW.WriteLine("BlackmanResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 3:
                                SW.WriteLine("GaussResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 4:
                                SW.WriteLine("LanczosResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 5:
                                SW.WriteLine("Lanczos4Resize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 6:
                                SW.WriteLine("PointResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 7:
                                SW.WriteLine("Spline16Resize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 8:
                                SW.WriteLine("Spline36Resize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 9:
                                SW.WriteLine("Spline64Resize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                            case 10:
                                SW.WriteLine("SincResize(" + ResizeWBox.Text + "," + ResizeHBox.Text + ")");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("缩放：请输入数值");
                        return;
                    }
                }
                //写入帧率
                if (FpsCheck.IsChecked == true)
                {
                    if (FpsBox.Text != "")
                    {
                        switch (FpsCombo.SelectedIndex)
                        {
                            case 0:
                                SW.WriteLine("AssumeFPS(" + FpsBox.Text + ", 1, false)");
                                break;
                            case 1:
                                SW.WriteLine("AssumeScaledFPS(" + FpsBox.Text + ", 1, false)");
                                break;
                            case 2:
                                SW.WriteLine("ChangeFPS(" + FpsBox.Text + ")");
                                break;
                            case 3:
                                SW.WriteLine("ConvertFPS(" + FpsBox.Text + ", zone=1)");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("帧率：请输入数值");
                        return;
                    }
                }
                //写入降噪
                if (DenoiseCheck.IsChecked == true)
                {
                    SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\UnDot.dll\")");
                    SW.WriteLine("Undot()");
                }
                //写入字幕
                if (SubCheck.IsChecked == true)
                {
                    if (SubBox.Text != "")
                    {
                        SW.WriteLine("LoadPlugin(\"" + Environment.CurrentDirectory + "\\bin\\avsPlugins\\VSFilter.dll\")");
                        SW.WriteLine("TextSub(\"" + SubBox.Text + "\", 1)");
                    }
                    else
                    {
                        MessageBox.Show("字幕：请选择字幕");
                        return;
                    }
                }
                //关闭文件
                SW.Flush();
                SW.Close();
            }
            catch
            {
                MessageBox.Show("不能生成avs脚本：文件被占用（请重试）");
                return;
            }
        }

        //开始编码
        SoundPlayer player = new SoundPlayer();
        private void StartCoding(object CodingPara)
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);  //阻止休眠
            CodingParameter Para = (CodingParameter)CodingPara;
            //核心文件路径
            string[] x264Path = null;
            string[] qaacPath = null;
            string[] mp4boxPath = null;
            string[] avs2pipePath = null;
            try
            {
                x264Path = Directory.GetFiles(Environment.CurrentDirectory + "\\bin\\x264\\", "*.exe", SearchOption.TopDirectoryOnly);
                qaacPath = Directory.GetFiles(Environment.CurrentDirectory + "\\bin\\qaac\\", "*.exe", SearchOption.TopDirectoryOnly);
                mp4boxPath = Directory.GetFiles(Environment.CurrentDirectory + "\\bin\\mp4box\\", "*.exe", SearchOption.TopDirectoryOnly);
                avs2pipePath = Directory.GetFiles(Environment.CurrentDirectory + "\\bin\\avs2pipe\\", "*.exe", SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                MessageBox.Show("核心异常");
                return;
            }

            Dispatcher.Invoke(new Action(() => {
                SettingGrid.Visibility = Visibility.Hidden;
                ProcessingGrid.Visibility = Visibility.Visible;
            }));
            ReadErrOutput -= new DelReadErrOutput(ReadErrOutputAction);
            ReadErrOutput += new DelReadErrOutput(ReadErrOutputAction);

            StreamReader ThemeColor = new StreamReader(Environment.CurrentDirectory + "\\Theme\\ThemeColor.ini");
            string ColorINI = ThemeColor.ReadToEnd();
            BrushConverter brushConverter = new BrushConverter();

            string APipeSource;
            string VPipeSource;
            string AcodeSource;
            string VcodeSource;
            if (Para.IndexerChecked)
            {
                APipeSource = avs2pipePath[0] + " audio \"" + Para.Apath + "\" | ";
                VPipeSource = avs2pipePath[0] + " video \"" + Para.Vpath + "\" | ";
                AcodeSource = "--ignorelength -";
                VcodeSource = "- --demuxer y4m";
            }
            else
            {
                APipeSource = "";
                VPipeSource = "";
                AcodeSource = Para.Apath;
                VcodeSource = Para.Vpath;
            }

            if (Para.QaacIndex > 1)
            {
                if (Para.IndexerChecked)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            ReturnBox.AppendText("\r\n--------    AnalysingAudio    --------");
                            ProgressMark = ProgressMark + "--------    AnalysingAudio    --------";
                            ProgressBarBox.IsIndeterminate = true;
                            ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingAudioColor", ColorINI)));
                            //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingAudioColor", ColorINI));
                            ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("AnalysingAudioImage", ColorINI)));
                            MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingAudioColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("AnalysingAudioImage", ColorINI))));
                        }
                        catch { }
                    }));
                    CodingOn(avs2pipePath[0] + " info \"" + Para.Apath + "\" 1>&2");
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    CodingAudio    --------");
                        ProgressMark = ProgressMark + "--------    CodingAudio    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingAudioColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingAudioColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingAudioImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingAudioColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingAudioImage", ColorINI))));
                    }
                    catch { }
                }));
                TempList.Add(Para.Apath + "_A.m4a");
                switch (Para.QaacIndex)
                {
                    case 2:
                        CodingOn(APipeSource + qaacPath[0] + " " + "--tvbr " + Para.QaacQ + " " + Para.AppendCommandA + " " + AcodeSource  + " -o \"" + Para.Apath + "_A.m4a\"  1>&2");
                        break;
                    case 3:
                        CodingOn(APipeSource + qaacPath[0] + " " + "--cvbr " + Para.QaacR + " " + Para.AppendCommandA + " " + AcodeSource + " -o \"" + Para.Apath + "_A.m4a\"  1>&2");
                        break;
                    case 4:
                        CodingOn(APipeSource + qaacPath[0] + " " + "--abr " + Para.QaacR + " " + Para.AppendCommandA + " " + AcodeSource + " -o \"" + Para.Apath + "_A.m4a\"  1>&2");
                        break;
                    case 5:
                        CodingOn(APipeSource + qaacPath[0] + " " + "--cbr " + Para.QaacR + " " + Para.AppendCommandA + " " + AcodeSource + " -o \"" + Para.Apath + "_A.m4a\"  1>&2");
                        break;
                }
                Para.Apath = Para.Apath + "_A.m4a";
            }

            if (Para.X264Index > 1)
            {
                if (Para.IndexerChecked)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            ReturnBox.AppendText("\r\n--------    AnalysingVideo    --------");
                            ProgressMark = ProgressMark + "--------    AnalysingVideo    --------";
                            ProgressBarBox.IsIndeterminate = true;
                            ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingVideoColor", ColorINI)));
                            //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingVideoColor", ColorINI));
                            ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("AnalysingVideoImage", ColorINI)));
                            MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("AnalysingVideoColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("AnalysingVideoImage", ColorINI))));
                        }
                        catch { }
                    }));
                    CodingOn(avs2pipePath[0] + " info \"" + Para.Vpath + "\" 1>&2");
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    CodingVideo    --------");
                        ProgressMark = ProgressMark + "--------    CodingVideo    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage", ColorINI))));
                    }
                    catch { }
                }));
                TempList.Add(Para.Vpath + "_V.mp4");
                TempList.Add(Para.Vpath + "_V.mp4" + ".stats");
                switch (Para.X264Index)
                {
                    case 2:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --bitrate " + Para.X264R + " " + Para.AppendCommandV + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 3:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --qp " + Para.X264Q + " " + Para.AppendCommandV + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 4:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 1 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 5:
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)));
                                //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI));
                                ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI)));
                                MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI))));
                            }
                            catch { }
                        }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 2 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 6:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 1 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)));
                                //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI));
                                ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI)));
                                MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI))));
                            }
                            catch { }
                        }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 2 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 7:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 1 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 8:
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)));
                                //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI));
                                ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI)));
                                MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI))));
                            }
                            catch { }
                        }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 3 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 9:
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI)));
                                //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI));
                                ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage3", ColorINI)));
                                MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage3", ColorINI))));
                            }
                            catch { }
                        }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 2 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 10:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 1 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                            Dispatcher.Invoke(new Action(() =>
                            {
                                try
                                {
                                    ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)));
                                    //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI));
                                    ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI)));
                                    MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor2", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage2", ColorINI))));
                                }
                                catch { }
                            }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 3 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI)));
                                //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI));
                                ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage3", ColorINI)));
                                MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("CodingVideoColor3", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CodingVideoImage3", ColorINI))));
                            }
                            catch { }
                        }));
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --pass 2 --bitrate " + Para.X264R + " " + Para.AppendCommandV + " --stats " + Para.Vpath + ".stats" + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                    case 11:
                        CodingOn(VPipeSource + x264Path[0] + " " + VcodeSource + " --crf " + Para.X264Q + " " + Para.AppendCommandV + " -o \"" + Para.Vpath + "_V.mp4\"  1>&2");
                        break;
                }
                Para.Vpath = Para.Vpath + "_V.mp4";
            }

            if (Para.QaacIndex > 0 && Para.X264Index > 0)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    Muxing    --------");
                        ProgressMark = ProgressMark + "--------    Muxing    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI))));
                    }
                    catch { }
                }));
                CodingOn(mp4boxPath[0] + " -add \"" + Para.Vpath + "#video\" -add \"" + Para.Apath + "#audio\" video -out \"" + Para.InFileWithOutEXName + "_mixed.mp4\"  1>&2");
            }
            else if(Para.X264Index > 1)
            {
                File.Delete(Para.InFileWithOutEXName + "_mixed.mp4");
                FileInfo FI = new FileInfo(Para.Vpath);
                FI.MoveTo(Para.InFileWithOutEXName + "_mixed.mp4");
            }
            else if (Para.QaacIndex > 1)
            {
                File.Delete(Para.InFileWithOutEXName + "_mixed.m4a");
                FileInfo FI = new FileInfo(Para.Apath);
                FI.MoveTo(Para.InFileWithOutEXName + "_mixed.m4a");

                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    Muxing    --------");
                        ProgressMark = ProgressMark + "--------    Muxing    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI))));
                    }
                    catch { }
                }));
                CodingOn(mp4boxPath[0] + " -add \"" + Para.Vpath + "#video\" -add \"" + Para.Apath + "#audio\" video -out \"" + Para.InFileWithOutEXName + "_mixed.mp4\"  1>&2");

            } else if(Para.X264Index == 1)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    Muxing    --------");
                        ProgressMark = ProgressMark + "--------    Muxing    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI))));
                    }
                    catch { }
                }));
                CodingOn(mp4boxPath[0] + " -add \"" + Para.Vpath + "#video\" video -out \"" + Para.InFileWithOutEXName + "_mixed.mp4\"  1>&2");
            } else if (Para.QaacIndex == 1)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        ReturnBox.AppendText("\r\n--------    Muxing    --------");
                        ProgressMark = ProgressMark + "--------    Muxing    --------";
                        ProgressBarBox.IsIndeterminate = false;
                        ChangeFillColor((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)));
                        //ProgressBarBox.Foreground = (Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI));
                        ImageBox.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI)));
                        MiniWin.Setting((Brush)brushConverter.ConvertFromString(GetParameterFormINI("MuxingColor", ColorINI)), new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("MuxingImage", ColorINI))));
                    }
                    catch { }
                }));
                CodingOn(mp4boxPath[0] + " -add \"" + Para.Apath + "#audio\" video -out \"" + Para.InFileWithOutEXName + "_mixed.m4a\"  1>&2");
            }

            Dispatcher.Invoke(new Action(() => {
                ReturnBox.AppendText("\r\n--------    ALL DOWN    --------");
                ProgressMark = ProgressMark + "--------    ALL DOWN    --------";
                CalculateProgress(null);
                ProgressBarBox.IsIndeterminate = false;
                AbortBut.Content = "完成";
            }));
            try
            {
                player.SoundLocation = Environment.CurrentDirectory + "\\Theme\\" + GetParameterFormINI("CompletedNotify", ColorINI);
                player.Load();
                player.Play();
            }
            catch { }
            SetThreadExecutionState(ES_CONTINUOUS);  //恢复休眠
        }

        private void ChangeFillColor(Brush Color)
        {
            FillPathRight.Fill = Color;
            FillPathLeft.Fill = Color;
            GlowPathRight.Fill = Color;
            GlowPathLeft.Fill = Color;
        }

        //编码线程相关
        private void CodingOn(string CMD)
        {
            CreateProcess("cmd.exe", "/c " + CMD);
        }

        public delegate void DelReadErrOutput(string result);
        public event DelReadErrOutput ReadErrOutput;
        Process CmdProcess;
        private void CreateProcess(string StartFileName, string StartFileArg)
        {
            CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName;
            CmdProcess.StartInfo.Arguments = StartFileArg;

            CmdProcess.StartInfo.CreateNoWindow = true;
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;
            CmdProcess.StartInfo.RedirectStandardError = true;
            CmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
            CmdProcess.EnableRaisingEvents = true;
            CmdProcess.Exited += new EventHandler(CmdProcess_Exited);

            CmdProcess.Start();
            CmdProcess.BeginErrorReadLine();
            CmdProcess.WaitForExit();

        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Dispatcher.Invoke(ReadErrOutput, e.Data);
            }
        }

        private void ReadErrOutputAction(string result)
        {
            if (LogOnCheck.IsChecked == true)
            {
                ReturnBox.AppendText("\r\n" + result);
            }
            if (AutoScrollCheck.IsChecked == true)
            {
                ReturnBox.ScrollToEnd();
            }
            CalculateProgress(result);
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ShowProgress(100);
            }));
        }

        //报文分析相关
        string ProgressMark = "";
        int vFrames = 0, aDuration = 0;
        private void CalculateProgress(string result)
        {
            double per = -1;
            Match M;

            if (ProgressMark.LastIndexOf("--------    ALL DOWN    --------") != -1)
            {
                ProgressMark = "";
                aDuration = 0;
                vFrames = 0;
                per = 100;
            }
            else if (ProgressMark.LastIndexOf("--------    Muxing    --------") != -1)
            {
                try
                {
                    M = Regex.Match(result, "\\([0-9]+/[0-9]+\\)", RegexOptions.RightToLeft);
                    double.TryParse(M.Value.Split('/')[0].Substring(1), out per);
                }
                catch { }
            }
            else if (ProgressMark.LastIndexOf("--------    CodingVideo    --------") != -1)
            {
                try
                {
                    if (ProgressMark.IndexOf("--------    USE DEMUXER    --------") != -1)
                    {
                        double vCurrent = 0;
                        M = Regex.Match(result, "[0-9]+ frames:.+fps.+", RegexOptions.RightToLeft);
                        M = Regex.Match(M.Value, "[0-9]+ frames");
                        double.TryParse(M.Value.Split(' ')[0], out vCurrent);

                        per = vCurrent / vFrames * 100;
                    }
                    else
                    {
                        M = Regex.Match(result, "\\[.+%\\]", RegexOptions.RightToLeft);
                        double.TryParse(M.Value.Substring(1, M.Value.Length - 3), out per);
                    }
                }
                catch { }
            }
            else if (ProgressMark.LastIndexOf("--------    AnalysingVideo    --------") != -1)
            {
                M = Regex.Match(result, "v:frames      [0-9]+");
                M = Regex.Match(M.Value, "[0-9]+");
                if(M.Value != "")
                {
                    int.TryParse(M.Value, out vFrames);
                }

                per = 100;
            }
            else if (ProgressMark.LastIndexOf("--------    CodingAudio    --------") != -1)
            {
                try
                {
                    if (ProgressMark.IndexOf("--------    USE DEMUXER    --------") != -1)
                    {
                        double aCurrent = 0;
                        M = Regex.Match(result, "[0-9]+(:[0-9]+)+\\.[0-9]+ \\(.+\\)", RegexOptions.RightToLeft);
                        string[] aCurrentStrArg = M.Value.Split(' ')[0].Split(':');
                        int index = 0;
                        foreach (string item in aCurrentStrArg)
                        {
                            double num = 0;
                            double.TryParse(item, out num);
                            aCurrent = aCurrent + num * Math.Pow(60, aCurrentStrArg.Length - 1 - index);
                            index++;
                        }

                        per = aCurrent / aDuration * 100;
                    }
                    else
                    {
                        M = Regex.Match(result, "\\[.+%\\]", RegexOptions.RightToLeft);
                        double.TryParse(M.Value.Substring(1, M.Value.Length - 3), out per);
                    }
                }
                catch { }
            }
            else if (ProgressMark.LastIndexOf("--------    AnalysingAudio    --------") != -1)
            {
                M = Regex.Match(result, "a:duration    [0-9]+");
                M = Regex.Match(M.Value, "[0-9]+");
                if(M.Value != "")
                {
                    int.TryParse(M.Value, out aDuration);
                }

                per = 100;
            }

            if (per > 100)
            {
                per = 100;
            }

            ShowProgress(per);
        }

        //终止相关
        private void AbortCoding(object sender, RoutedEventArgs e)
        {
            if (AbortBut.Content.ToString() == "终止")
            {
                CodingThread.Abort();
                KillProcessAndChildren(CmdProcess.Id);
                AbortBut.Content = "完成";
            }
            else
            {
                if(CmdProcess != null)
                {
                    CmdProcess.Close();
                }

                ReturnBox.Clear();
                SettingGrid.Visibility = Visibility.Visible;
                ProcessingGrid.Visibility = Visibility.Hidden;
                AbortBut.Content = "终止";

                TempList.Add(FileBox.Text + ".lwi");
                TempList.Add(FileBox.Text + ".ffindex");
                foreach (string i in TempList)
                {
                    File.Delete(i);
                }
            }

        }
        public void KillProcessAndChildren(int pid)  //终止线程函数
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                //Console.WriteLine(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                /* process already exited */
                ReturnBox.AppendText("\r\n--------    Process Aborted    --------");
            }
            ReturnBox.AppendText("\r\n--------    Aborting Process    --------");

        }

        //进度条渲染相关
        private void ShowProgress(double per)
        {
            if(per == -1)
            {
                return;
            }
            try
            {
                ProgressBox.Text = Math.Round(per, 2) + "%";
                ProgressBarBox.Value = per;
                MiniWin.ShowProgress(per);
            }
            catch (Exception)
            {
                ProgressBox.Text = "ERROR";
                ProgressBarBox.Value = 0;
                MiniWin.ShowProgress(0);
                per = 0;
            }


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

        private void MiniWindow(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            MiniWin.Show();
        }

        private void DetailWindow()
        {
            this.Show();
            MiniWin.Hide();

        }

        private void SelectAll(object sender, MouseButtonEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (!TB.IsFocused)
            {
                TB.Focus();
                TB.SelectAll();
                e.Handled = true;
            }
        }

    }
}
