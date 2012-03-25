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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Windows.Threading;
using System.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private float lastx1, lasty1, currentx1, currenty1;
        private float lastx2, lasty2, currentx2, currenty2;
        private bool isFirst1 = true;
        private bool isFirst2 = true;
        private MySqlConnection conn1 = null;
        private MySqlCommand command1 = null;
        private MySqlDataReader reader1 = null;
        private MySqlConnection conn2 = null;
        private MySqlCommand command2 = null;
        private MySqlDataReader reader2 = null;
        private DispatcherTimer timer1 = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            OpenDB1();
            OpenDB2();
            timer1.Interval = TimeSpan.FromMilliseconds(10);
            timer1.Tick +=new EventHandler(timer_Tick1);
            timer2.Interval = TimeSpan.FromMilliseconds(10);
            timer2.Tick +=new EventHandler(timer_Tick2);
        }

        private void timer_Tick1(object sender, EventArgs args)
        {
            LoadPoint1();
        }

        private void timer_Tick2(object sender, EventArgs args)
        {
            LoadPoint2();
        }

        private void LoadPoint1()
        {
            try
            {
                if (!reader1.Read()) 
                {
                    timer1.Stop();
                    CloseDB1();
                }
                currentx1 = (float)reader1[1];
                currenty1 = (float)reader1[2];
                if (isFirst1)
                {
                    isFirst1 = false;
                    lastx1 = currentx1;
                    lasty1 = currenty1;
                }
                else
                {
                    DrawLine(lastx1, lasty1, currentx1, currenty1);
                       
                    lastx1 = currentx1;
                    lasty1 = currenty1;
                }
        }
            catch (MySqlException se)
            {
                Console.WriteLine("Database operation errors : " + se.StackTrace);
                timer1.Stop();
                CloseDB1();
            }
        }

        private void LoadPoint2()
        {
            try
            {
                if (!reader2.Read())
                {
                    timer2.Stop();
                    CloseDB2();
                }
                currentx2 = (float)reader2[1];
                currenty2 = (float)reader2[2];
                if (isFirst2)
                {
                    isFirst2 = false;
                    lastx2 = currentx2;
                    lasty2 = currenty2;
                }
                else
                {
                    DrawLine(lastx2, lasty2, currentx2, currenty2);
                    lastx2 = currentx2;
                    lasty2 = currenty2;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void DrawLine(float startx, float starty, float endx, float endy)
        {
            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(startx, starty);
            line.EndPoint = new Point(endx, endy);

            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = line;
            Can.Children.Add(myPath); 
        }

        private void OpenDB1()
        {
            conn1 = new MySqlConnection("Server=localhost;User Id=root;Password='';Persist Security Info=True;Database=point");
            command1 = conn1.CreateCommand();
            string sql = "select *from point1";
            command1.CommandText = sql;
            conn1.Open();
            reader1 = command1.ExecuteReader();
        }

        private void OpenDB2()
        {
            conn2 = new MySqlConnection("Server=localhost;User Id=root;Password='';Persist Security Info=True;Database=point");
            command2 = conn2.CreateCommand();
            string sql = "select *from point2";
            command2.CommandText = sql;
            conn2.Open();
            reader2 = command2.ExecuteReader();
        }

        private void CloseDB1()
        {
            conn1.Close();
            command1 = null;
            reader1.Close();
        }

        private void CloseDB2()
        {
            conn2.Close();
            command2 = null;
            reader2.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            load loadData = new load();
            loadData.ShowDialog();

            status.Content = "程序正在演示.....";
            System.Threading.Thread sub = new System.Threading.Thread(new ThreadStart(startup1));
            sub.Start();
        }

        /*启动函数*/
        private void startup1()
        {
            timer1.Start();
            System.Threading.Thread sub = new System.Threading.Thread(new ThreadStart(startup2));
            sub.Start();
        }

        private void startup2()
        {
            timer2.Start();
        }

        private void start_MouseEnter(object sender, MouseEventArgs e)
        {
            tip.Content = "点击开始进行演示";
        }

        private void end_MouseEnter(object sender, MouseEventArgs e)
        {
            tip.Content = "点击终止演示";
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            conn1.Close();
            conn2.Close();
            Can.Children.Clear();
        }

        private void start_MouseLeave(object sender, MouseEventArgs e)
        {
            tip.Content = "";
        }

        private void end_MouseLeave(object sender, MouseEventArgs e)
        {
            tip.Content = "";
        }
    }
}
