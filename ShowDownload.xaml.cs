using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;

namespace WMCL
{
    /// <summary>
    /// ShowDownload.xaml 的交互逻辑
    /// </summary>
    public partial class ShowDownload : Window
    {
        public ShowDownload()
        {
            InitializeComponent();
        }
        List<String> targetURL = new List<string>();//声明用于此窗体的下载列表
        int pointer = 0;//声明数组指针
        String reCall = "";//声明并初始化重调用参数的值
        WebClient wc = new WebClient();//声明下载的对象
        public LauncherReturn plr = new LauncherReturn();//声明代存的量
        /// <summary>
        /// 下载文件主方法
        /// </summary>
        /// <param name="URLList">要下载的文件列表，URL与文件路径之间用:::分隔。</param>
        /// <param name="recaller">反调参数，以,%,%,分隔</param>
        public void DownloadBL(List<string> URLList, String recaller)
        {
            listBox1.Items.Clear();//清空listBox1
            for (int c = 0; c < URLList.Count; c++)//为每个下载项循环
            {
                listBox1.Items.Add("下载" + URLList[c].Replace(":::", "到"));//添加项并向用户显示该项的下载信息
            }
            targetURL = URLList;//赋值给出的下载列表到用于此窗体的下载列表
            reCall = recaller;//赋值给出的参数备用
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(this.NextDownload);//为下载完成事件添加要执行的方法
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            progressBar1.Value = 0;//将进度条设为初始状态
            progperc.Content = "0%";//将显示进度清零
            this.Show();
            NextDownload(this, null);//开始下载任务队列
        }

        public void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            label2.Content = e.ProgressPercentage + "%";
        }

        private void NextDownload(object sender, AsyncCompletedEventArgs e)
        {//下载任务队列执行方法
            String[] uat;//声明公共数组uat
            if (pointer != 0)//如果不是刚开始
            {
                uat = targetURL[pointer - 1].Replace(":::", "|").Split("|".ToCharArray());//处理下载地址和目标文件路径
                if (new FileInfo(uat[1]).Length == 0)//检查上一个下载的文件
                {//如果下载出错
                    File.Delete(uat[1]);//删掉有问题的文件
                }
            }
            if (pointer == targetURL.Count)//如果下载完成
            {
                if (reCall == "restart")//如果要重启
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);//运行
                    System.Windows.Forms.Application.Exit();//退出
                    return;//返回
                }
                if (reCall == "installforge")//如果要安装forge
                {
                    uat = targetURL[pointer - 1].Replace(":::", "|").Split("|".ToCharArray());//处理下载地址和目标文件路径
                    new Launcher().decompress(uat[1], System.Windows.Forms.Application.StartupPath + "\\Forge_Install_Temp");//解压安装文件
                    String fr = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\Forge_Install_Temp\\install_profile.json").Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");//读取install_profile.json
                    int tp = fr.IndexOf("\"versionInfo\":") + "\"versionInfo".Length + 2;//读取versionInfo的值
                    String vi = fr.Substring(tp, fr.LastIndexOf("}") - tp);//同上
                    tp = fr.IndexOf("\"target") + "\"target".Length + 3;//读取版本名
                    if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\.minecraft\\versions\\" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp)))//如果版本文件夹不存在
                    {
                        Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\.minecraft\\versions\\" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp));//创建
                    }
                    if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\.minecraft\\versions\\" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp).Substring(0, fr.Substring(tp, fr.IndexOf("\"", tp) - tp).IndexOf("-"))))//如果安装了纯净版
                    {
                        if (vi.IndexOf("\"jar\":") == -1)//如果文件中未指定jar
                        {
                            vi = "{\"jar\":\"" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp).Substring(0, fr.Substring(tp, fr.IndexOf("\"", tp) - tp).IndexOf("-")) + "\"," + vi.Substring(1);//指定
                        }
                    }
                    else//未安装纯净版
                    {
                        MessageBox.Show("请先安装纯净版！");//提示用户安装
                        this.Close();//关闭窗体
                        return;//返回
                    }
                    File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\.minecraft\\versions\\" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp) + "\\" + fr.Substring(tp, fr.IndexOf("\"", tp) - tp) + ".json", vi);//写入读取到的json值
                    this.Close();//关闭窗体
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);//运行
                    System.Windows.Forms.Application.Exit();//退出
                    return;//返回
                }
                if (reCall != "")//如果调用不为空
                {
                    String[] ra = reCall.Replace(",%,%,", "\"").Split("\"".ToCharArray());//处理参数
                    plr = new Launcher().Launch(ra[0], ra[1], ra[2], ra[3], 0, true);//重新调用Launch方法并传参
                }
                this.Close();//关闭窗体
                return;//退出方法执行
            }
            uat = targetURL[pointer].Replace(":::", "|").Split("|".ToCharArray());//处理下载地址和目标文件路径
            listBox1.SelectedIndex = pointer;//选中正在下载的项，以告诉用户
            String s = uat[1].Replace(System.IO.Path.GetFileName(uat[1]), "");//获得文件所在文件夹路径
            if (!Directory.Exists(s))//如果文件夹不存在
            {
                Directory.CreateDirectory(s);//创建文件夹（在C#里面可能是多余的，但是在其他编程语言中要这么做）
            }
            wc.DownloadFileAsync(new Uri(uat[0]), uat[1]);//下载指定的文件
            pointer = pointer + 1;//指针后退
            int per = pointer * 100 / targetURL.Count;//计算下载完成的百分比
            progressBar1.LargeChange = per - progressBar1.Value;//设置进度条的单次前进值

            progperc.Content = per + "%";//显示最新的进度百分比（已取整）
        }
    }
}
