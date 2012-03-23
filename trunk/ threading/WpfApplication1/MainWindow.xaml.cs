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

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private float lastx, lasty, currentx, currenty;
        private bool isFirst = false;

        public MainWindow()
        {
            InitializeComponent();

        //    load l = new load();
        //    l.ShowDialog();
            LoadPoint();
        }

        private void LoadPoint()
        {
            MySqlConnection conn = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            float rateX = (float)(this.Can.ActualWidth / 100);
            float rateY = (float)(this.Can.ActualHeight / 120);

            try
            {
                conn = new MySqlConnection("Server=localhost;User Id=root;Password='';Persist Security Info=True;Database=point");
                command = conn.CreateCommand();
                string sql = "select *from point1";
                command.CommandText = sql;
                conn.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    currentx = ((float)reader[1] / 2 - 400);
                    currenty = ((float)reader[2] / 2 + 100);
                    if (isFirst)
                    {
                        isFirst = false;
                        lastx = currentx;
                        lasty = currenty;
                    }
                    else
                    {
                        DrawLine(lastx, lasty, currentx, currenty);
                        //System.Threading.Thread.Sleep(1000);
                        lastx = currentx;
                        lasty = currenty;
                    }
                }
            }
            catch (MySqlException se)
            {
                Console.WriteLine("Database operation errors : " + se.StackTrace);
                conn.Close();
                command = null;
                reader.Close();
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
    }
}
