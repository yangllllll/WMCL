using KMCCC.Authentication;
using KMCCC.Launcher;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using WMCL;
using System.Windows.Media.Animation;

namespace WMCL.login
{
    /// <summary>
    /// login.xaml 的交互逻辑
    /// </summary>
    public partial class login : Page
    {
        #region  API函数声明
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filepath);
        /// <summary>
        /// windows api 名称
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        #endregion
        #region 读Ini文件

        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {

                return String.Empty;
            }
        }

        #endregion
        #region 写Ini文件

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
        string filePath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.sys");
        public login()
        {
            string str = System.Environment.CurrentDirectory;
            Environment.SetEnvironmentVariable("WMCL",str, EnvironmentVariableTarget.Machine);
            InitializeComponent();
            List<string> javalist = new List<string>();
            foreach (string i in KMCCC.Tools.SystemTools.FindJava())
            {
                javalist.Add(i);
            }

            javacombo.ItemsSource = javalist;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.Create(filePath1);//创建INI文件
            if (!Directory.Exists("image"))//目录不存在
            {
                Directory.CreateDirectory("image");//创建
            }
            AnimationHelper.SetEasingFunction(_1, new CubicEase() { EasingMode = EasingMode.EaseOut });
            AnimationHelper.SetMarginTo(_1, new Thickness(685, 10, -338, 10));
            AnimationHelper.SetEasingFunction(_2, new CubicEase() { EasingMode = EasingMode.EaseOut });
            AnimationHelper.SetMarginTo(_2, new Thickness(337, 10, 10, 10));
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect =false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "图片(*.jpg*)|*.jpg*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                File.Copy(file, "image/dg.jpg", isrewrite);

               
                image2.ImageSource = new BitmapImage(new Uri("image/dg.jpg", UriKind.Relative));
                WritePrivateProfileString("username", "image", "true", filePath1);
            }
           
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            if (usernamechonged.Text == "")
            {
              MessageBoxX.Show("用户名不能为空", "警告");

            }
            else
            {
                WritePrivateProfileString("username", "login", "离线登录", filePath1);
                WritePrivateProfileString("username", "name", usernamechonged.Text, filePath1);
                AnimationHelper.SetEasingFunction(_2, new CubicEase() { EasingMode = EasingMode.EaseOut });
                AnimationHelper.SetMarginTo(_2, new Thickness(685, 10, -338, 10));
                AnimationHelper.SetEasingFunction(_3, new CubicEase() { EasingMode = EasingMode.EaseOut });
                AnimationHelper.SetMarginTo(_3, new Thickness(330, 10, 10, 10));
            }
            




        }

        private void Button18_Click(object sender, RoutedEventArgs e)
        {
            WritePrivateProfileString("java", "config", javacombo.Text, filePath1);
            WritePrivateProfileString("java", "mix", javamix.Text, filePath1);
            WritePrivateProfileString("java", "max", javamax.Text, filePath1);
            string theam = ReadIniData("theam", "1", "", filePath1);
            if (h.IsChecked == true)
            {
                WritePrivateProfileString("theam", "1", "true", filePath1);
                WritePrivateProfileString("theam", "2", "false", filePath1);

               
            }
            else
            {
                WritePrivateProfileString("theam", "1", "false", filePath1);
                WritePrivateProfileString("theam", "2", "true", filePath1);

               


            }
            AnimationHelper.SetEasingFunction(_3, new CubicEase() { EasingMode = EasingMode.EaseOut });
            AnimationHelper.SetMarginTo(_3, new Thickness(685, 10, -338, 10));
            AnimationHelper.SetEasingFunction(_4, new CubicEase() { EasingMode = EasingMode.EaseOut });
            AnimationHelper.SetMarginTo(_4, new Thickness(330, 10, 10, 10));
        }

        private void Button19_Click(object sender, RoutedEventArgs e)
        {
            WritePrivateProfileString("username", "login", "离线登录", filePath1);
            WritePrivateProfileString("username", "name", usernamechonged.Text, filePath1);
            WritePrivateProfileString("username", "rj", "true", filePath1);
            System.Windows.Forms.Application.Restart();
            Process.GetCurrentProcess().Kill();

        }

       

        private void usernamechonged_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            //选择文件
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择java文件";
            dialog.Filter = "javaw(*.exe*)|*.exe*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                javacombo.Items.Add(file);
            }
        }
      
      
        private void grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void yh1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void yh1_Click(object sender, RoutedEventArgs e)
        {
            if (yh1.IsChecked == true)
            {
                _5.IsEnabled = true;
            }
            else
            {
                _5.IsEnabled = false;
            }
        }
    }
}
