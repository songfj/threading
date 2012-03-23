using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// load.xaml 的交互逻辑
    /// </summary>
    public partial class load : Window
    {
        /*读取函数*/
        [DllImport("calculate_point.dll", EntryPoint = "MainThread")]
        public static extern void MainThread();
        [DllImport("calculate_point.dll", EntryPoint = "New_GetMainPercentage")]
        public static extern float GetMainPercentage();
        [DllImport("calculate_point.dll", EntryPoint = "New_GetChildPercentage")]
        public static extern float GetChildPercentage();

        private Thread thread;
        private bool isLoadDataFinish = false;
        private System.Windows.Threading.DispatcherTimer timer;
        private float step = 2.0f;/*定时器的默认步距*/
        private bool isDestorySubThread = false;

        public load()
        {
            InitializeComponent();

            /*按钮的初始状态*/
            Finish.IsEnabled = false;
          //  Cancel.IsCancel = true;
            /*将计算函数加入线程*/
            thread = new Thread(new ThreadStart(LoadData));
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            thread.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((this.bar0.Value >= 100.0) && (this.bar1.Value >= 100.0))
            {
                if (this.isLoadDataFinish == false)
                {
                    //System.Windows.MessageBox.Show(GetMainPercentage().ToString());
                }
                else
                {
                    timer.Stop();
                    /*按钮的初始状态*/
                    Finish.IsEnabled = true;
                }
            }//end if
            else
            {
                if (this.bar0.Value < 100.0)
                {
                    this.bar0.Value = GetMainPercentage()*100;
                    this.barValue0.Content = this.bar0.Value.ToString("F2");
                }

                if (this.bar1.Value < 100.0)
                {
                    this.bar1.Value = GetChildPercentage()*100;
                    this.barValue1.Content = this.bar1.Value.ToString("F2");
                }
            }//end else
            
        }

        private void LoadData()
        {
            MainThread();
            isLoadDataFinish = true;
        }

        #region
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (isLoadDataFinish == false)
            {
                if (System.Windows.Forms.MessageBox.Show("数据正在加载， 要切断吗？", 
                                                        "询问",
                                                        System.Windows.Forms.MessageBoxButtons.OKCancel,
                                                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    //cut down the thread
                   // isDestorySubThread = true;
                    thread.Interrupt();
                    Thread.Sleep(500);
                    //close the loading page
                    this.Close();
                }//end if
            }//end if
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /*当组件加载好了以后， 就加载线程*/
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(MainThread));
            thread.Start();
        }
        #endregion
    }
}
