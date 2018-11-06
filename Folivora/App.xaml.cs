using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Folivora
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // 在异常由应用程序引发但未进行处理时发生。主要指的是UI线程。
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            //  当某个异常未被捕获时出现。主要指的是非UI线程
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show("很抱歉，当前应用程序遇到一些无法恢复的问题，程序即将关闭.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Error);
            //MessageBox.Show("非UI线程异常 : \n\n" + string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace));
            System.Environment.Exit(0);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            MessageBox.Show("很抱歉，当前应用程序遇到一些问题，当前操作已终止.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Information);
            //MessageBox.Show("UI线程异常 : \n\n" + string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace));
            e.Handled = true;
        }

        private void CrashLog(string message)
        {
            string Folder = Environment.CurrentDirectory + "\\CrashLog\\";
            //Directory.CreateDirectory(Folder);
            string time = DateTime.Now.ToString().Replace(':', '-').Replace('/', '-');

            int i = 0;
            while (i < 100)
            {
                string filename = time;
                if (i != 0)
                {
                    filename = filename + " (" + i + ")";
                }
                if (!File.Exists(Folder + filename + ".log"))
                {
                    try
                    {
                        StreamWriter SW = new StreamWriter(Folder + filename + ".log", false);
                        SW.WriteLine(message);
                        SW.Close();
                        break;
                    }
                    catch { }
                }
                i++;
            }
        }

    }
}
