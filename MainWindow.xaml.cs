
using KMCCC.Authentication;
using KMCCC.Launcher;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;


namespace WMCL
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {


        public static LauncherCore Core = LauncherCore.Create();

        //目前用户头像暂不可用
        private void Move_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
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







        string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.sys");
        WebClient wc = new WebClient();
        public String rtxt = "";

        public MainWindow()
        {

            var versionsb = Core.GetVersions().ToArray();
            ListBox5.ItemsSource = versionsb;//绑定数据源
            List<string> javalist = new List<string>();
            foreach (string i in KMCCC.Tools.SystemTools.FindJava())
            {
                javalist.Add(i);
            }

            comboBox2.ItemsSource = "12";

            if (File.Exists(filePath))
            {
                string user = ReadIniData("username", "name", "", filePath);
                string Java = ReadIniData("Java", "config", "", filePath);
                string Javas = ReadIniData("Java", "mix", "", filePath);
                string Javam = ReadIniData("Java", "max", "", filePath);
                string icon = ReadIniData("username", "icon", "", filePath);
                string login = ReadIniData("username", "login", "", filePath);
                label1.Content = "你好！" + user;
                TextBox1.Text = user;
                comboBox2.Items.Add(Java);
                comboBox2.Text = Java;
                label20.Content = login;
                TextBox2.Text = Javam;
                TextBox3.Text = Javas;
            }
            else
            {

                File.Create(filePath);//创建INI文件
                WritePrivateProfileString("username", "name", "", filePath);
                WritePrivateProfileString("java", "config", "", filePath);
                WritePrivateProfileString("java", "mix", "512", filePath);
                WritePrivateProfileString("java", "max", "1024", filePath);
                WritePrivateProfileString("username", "icon", "", filePath);


            }
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\tmp.json"))//如果临时文件存在
                {
                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp.json");//删掉

                }
                wc.DownloadFile("https://launchermeta.mojang.com/mc/game/version_manifest.json", System.Windows.Forms.Application.StartupPath + "\\tmp.json");//下载版本列表文件
                rtxt = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\tmp.json").Replace("\n", "").Replace(" ", "");//读取版本列表
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp.json");//删除临时文件
                int tmp = rtxt.IndexOf("versions") + "versions".Length + 3;//读取objects数组数据
                String versions_S = rtxt.Substring(tmp, rtxt.LastIndexOf("]", rtxt.Length - 1) - tmp);//同上
                String[] versions = versions_S.Replace("},{", "$").Split("$".ToCharArray());//分割数组
                String tmu = "";//声明并初始化tmu
                ClientList.Items.Clear();//清空列表
                foreach (String vi in versions)
                {//为每个项循环
                    tmp = vi.IndexOf("id") + "id".Length + 3;//读取id
                    tmu = vi.Substring(tmp, vi.IndexOf("\"", tmp) - tmp);//同上
                    //ClientList.Items.Add(tmu);//添加项
                }



            }
            catch (Exception)
            {
                MessageBoxX.Show("无法连接至服务器", "警告", Owner = null, MessageBoxButton.OK);
            }



        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {



        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {


            button7.Margin =
              new Thickness(0, 220, 0, 0);

            R1.Margin =
               new Thickness(209, 0, 0, 0);
            R3.Margin =
             new Thickness(887, 465, -755, -364);
            R2.Margin =
              new Thickness(-736, 134, 868, -34);
            R.Margin =
              new Thickness(845, 71, -713, 30);
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            button7.Margin =
              new Thickness(0, 170, 0, 0);
            R1.Margin =
                new Thickness(845, 71, -713, 30);
            R3.Margin =
             new Thickness(887, 465, -755, -364);
            R2.Margin =
              new Thickness(-736, 134, 868, -34);
            R.Margin =
              new Thickness(209, 0, 0, 0);
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            button7.Margin =
            new Thickness(0, 270, 0, 0);
            R1.Margin =
               new Thickness(845, 71, -713, 30);
            R3.Margin =
             new Thickness(887, 465, -755, -364);
            R2.Margin =
              new Thickness(209, 0, 0, 0);
            R.Margin =
              new Thickness(-736, 134, 868, -34);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            button7.Margin =
            new Thickness(0, 320, 0, 0);
            R1.Margin =
                new Thickness(845, 71, -713, 30);
            R3.Margin =
             new Thickness(209, 0, 0, 0);
            R2.Margin =
              new Thickness(-736, 134, 868, -34);
            R.Margin =
              new Thickness(887, 465, -755, -364);
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        private void Window_loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
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
                comboBox2.Items.Add(file);
            }
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            //string icon = image1.ImageSource;
            WritePrivateProfileString("username", "login", "离线登录", filePath);
            WritePrivateProfileString("username", "name", TextBox1.Text, filePath);
            // WritePrivateProfileString("username", "icon", image1.ImageSource, filePath);
            string user = ReadIniData("username", "name", "", filePath);
            string login = ReadIniData("username", "login", "", filePath);
            string Java = ReadIniData("Java", "config", "", filePath);
            string Javas = ReadIniData("Java", "mix", "", filePath);
            string Javam = ReadIniData("Java", "max", "", filePath);
            string icon = ReadIniData("username", "icon", "", filePath);
            label1.Content = "你好！" + user;
            TextBox1.Text = user;
            label20.Content = login;
        }

        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            WritePrivateProfileString("java", "config", comboBox2.Text, filePath);
            WritePrivateProfileString("java", "mix", TextBox3.Text, filePath);
            WritePrivateProfileString("java", "max", TextBox2.Text, filePath);

        }

        private void ComboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button15_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\tmp.json"))//如果临时文件存在
                {
                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp.json");//删掉

                }
                wc.DownloadFile("https://launchermeta.mojang.com/mc/game/version_manifest.json", System.Windows.Forms.Application.StartupPath + "\\tmp.json");//下载版本列表文件
                rtxt = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\tmp.json").Replace("\n", "").Replace(" ", "");//读取版本列表
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\tmp.json");//删除临时文件
                int tmp = rtxt.IndexOf("versions") + "versions".Length + 3;//读取objects数组数据
                String versions_S = rtxt.Substring(tmp, rtxt.LastIndexOf("]", rtxt.Length - 1) - tmp);//同上
                String[] versions = versions_S.Replace("},{", "$").Split("$".ToCharArray());//分割数组
                String tmu = "";//声明并初始化tmu
                ClientList.Items.Clear();//清空列表
                foreach (String vi in versions)
                {//为每个项循环
                    tmp = vi.IndexOf("id") + "id".Length + 3;//读取id
                    tmu = vi.Substring(tmp, vi.IndexOf("\"", tmp) - tmp);//同上
                    ClientList.Items.Add(tmu);//添加项
                }



            }
            catch (Exception)
            {
                MessageBox.Show("无法连接到服务器，无法获取游戏版本列表", "警告");
            }

        }

        private void Button16_Click(object sender, RoutedEventArgs e)
        {
            if (ClientList.SelectedIndex == -1)
                return;
            else
            {
                string version = ClientList.SelectedItem.ToString();
                String vdp = System.Windows.Forms.Application.StartupPath + "\\.minecraft\\versions\\" + version;//处理下载目标路径
                List<String> cu = new List<string>();//声明下载列表
                cu.Add("https://bmclapi2.bangbang93.com/version/" + version + "/Client" + ":::" + vdp + "\\" + version + ".jar");//加入下载项
                cu.Add("https://bmclapi2.bangbang93.com/version/" + version + "/Client" + ":::" + vdp + "\\" + version + ".json");//加入下载项
                new ShowDownload().DownloadBL(cu, "restart");//执行下载并要求重启启动器
            }
        }

        private void l_loaded(object sender, RoutedEventArgs e)
        {

        }


        private void Button30_Click(object sender, RoutedEventArgs e)
        {
            string user = ReadIniData("username", "name", "", filePath);
            string login = ReadIniData("username", "login", "", filePath);
            string Java = ReadIniData("Java", "config", "", filePath);
            string Javas = ReadIniData("Java", "mix", "", filePath);
            if (ListBox5.SelectedIndex == -1)
                MessageBoxX.Show("必须选择一个游戏核心", "错误", Owner = null, MessageBoxButton.OK);
            else
            {


                if (user == "")
                {
                    MessageBoxX.Show("登录并设置java", "错误", Owner = null, MessageBoxButton.OK);

                }
                else
                {
                    try
                    {

                        button30.Content = "正在启动";
                        button30.IsEnabled = false;
                        int javamm = int.Parse(comboBox2.Text);
                        var ver = (KMCCC.Launcher.Version)ListBox5.SelectedItem;
                        var result = Core.Launch(new LaunchOptions
                        {
                            Version = ver, //Ver为Versions里你要启动的版本名字
                            MaxMemory = javamm, //最大内存，int类型

                            Authenticator = new OfflineAuthenticator(user), //离线启动，ZhaiSoul那儿为你要设置的游戏名
                                                                            //Authenticator = new YggdrasilLogin("邮箱", "密码", true), // 正版启动，最后一个为是否twitch登录
                            Mode = LaunchMode.MCLauncher, //启动模式，这个我会在后面解释有哪几种
                                                          // Server = new ServerInfo { Address = "服务器IP地址", Port = "服务器端口" }, //设置启动游戏后，自动加入指定IP的服务器，可以不要
                                                          //Size = new WindowSize { Height = 768, Width = 1280 } //设置窗口大小，可以不要
                        });
                        this.Close();

                    }
                    catch (Exception)
                    {
                        MessageBoxX.Show("启动失败", "错误", Owner = null, MessageBoxButton.OK);
                        button30.Content = "启动游戏";
                        button30.IsEnabled = true;


                    }
                }


            }


        }


    }
}
