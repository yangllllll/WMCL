using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace WMCL
{

    public class LauncherReturn//返回的公共类
    {
        public String rtv = "";//子值，返回字符串
        public String[] rtvs;//子值，返回字符串数组，在正版登陆时有用
        public List<string> DUList = new List<string>();//子值，返回下载列表
        public Process LP;//子值，返回主进程
        public ShowDownload sdd;
    }

    class Launcher
    {
        private List<string> URLList = new List<string>();//声明总列表
        /// <summary>
        /// 启动主方法
        /// </summary>
        /// <param name="MaxMem">最大内存</param>
        /// <param name="JavaPath">Javaw.exe路径</param>
        /// <param name="UserName">用户名</param>
        /// <param name="VerName">版本名</param>
        /// <param name="returnvalue">返回值的指定。0为不返回，直接执行；1为返回库的完整字符串；2为返回完整的启动命令，3为返回游戏的主进程。默认为0。</param>
        /// <param name="dc">指示是否下载文件。</param>
        public LauncherReturn Launch(string MaxMem, string JavaPath, string UserName, string VerName, int returnvalue = 0, bool dc = false, string ExtraMCArg = "", string ExtraJreArgs = "")
        {
            string rtxt = "";//声明
            int tmp = 0;//声明
            rtxt = File.ReadAllText(Application.StartupPath + "\\.minecraft\\versions\\" + VerName + "\\" + VerName + ".json").Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("\"rules\":[{\"action\":\"allow\"},{\"action\":\"disallow\",\"os\":{\"name\":\"osx\"}}],", "");//读取内容
            tmp = rtxt.IndexOf("mainClass") + "mainClass".Length + 3;//取名称起始位置并赋值到tmp
            String mainClass = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//截取值
            tmp = rtxt.IndexOf("minecraftArguments") + "minecraftArguments".Length + 3;//json读取部分，同上
            String minecraftArguments = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp).Replace("${", " ${").Replace("}", "} ").Replace("cpw", " cpw").Replace("--tweakClassnet.minecraftforge.fml.common.launcher.FMLTweaker", "--tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker ").Replace("--tweakClasscom.mumfrey.liteloader.launch.LiteLoaderTweaker", "--tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker ").Replace("--versionTypeForge", "--versionType Forge ");//json读取部分，同上
            tmp = rtxt.IndexOf("libraries") + "libraries".Length + 3;//json读取部分，同上
            String libraries = rtxt.Substring(tmp + 1, rtxt.LastIndexOf("]") - tmp);//json读取部分，同上
            String natives = Application.StartupPath + "\\.minecraft\\versions\\" + VerName + "\\" + VerName + "-natives";//设置natives路径
            if (!Directory.Exists(natives))//目录不存在
            {
                Directory.CreateDirectory(natives);//创建
            }
            String[] libs = libraries.Replace("${arch}", "64").Replace("},{", "^").Split("^".ToCharArray());//将},{替换为^然后以^进行分割，顺便决定32位系统
            String libp = "";//声明libp
            for (int i = 0; i < libs.Length; i++)//为数组内容循环，也可以考虑foreach
            {
                if (libs[i].IndexOf("name") == -1)//如果没有name值
                {
                    continue;//跳过
                }
                String tml = libs[i];//声明临时字符串变量tml
                if (tml.IndexOf("{") != -1)//如果含有子json对象，则去掉子项
                {
                    do//循环
                    {
                        try//捕获错误
                        {
                            tml = tml.Replace(tml.Substring(tml.LastIndexOf("{") - 1, tml.IndexOf("}", tml.LastIndexOf("{")) - tml.LastIndexOf("{") + 1), "");//去掉子项
                        }
                        catch (ArgumentOutOfRangeException) //如果是干扰的
                        {
                            tml = tml + "," + libs[i + 1];//追加
                            tml = tml.Replace(tml.Substring(tml.LastIndexOf("{") - 1, tml.IndexOf("}", tml.LastIndexOf("{")) - tml.LastIndexOf("{") + 1), "");//再去掉子项
                        }
                    } while (tml.IndexOf("{") != -1);//有子项则仍然循环
                    tml = tml.Replace("{", "").Replace("}", "");//去掉干扰的{}
                }
                tmp = tml.IndexOf("name") + "name".Length + 3;//json读取部分，同上
                string libn = tml.Substring(tmp, length: tml.IndexOf("\"", tmp) - tmp);//json读取部分，同上
                if (libn.IndexOf(":") == -1)//如果name的值不合法
                {
                    continue;//跳过
                }

                String[] tlib = new String[] { libn.Substring(0, libn.IndexOf(":")).Replace(":", ""), libn.Substring(libn.IndexOf(":") + 1, libn.IndexOf(":", libn.IndexOf(":") + 1) - libn.IndexOf(":")).Replace(":", ""), libn.Substring(libn.IndexOf(":", libn.IndexOf(":") + 1)).Replace(":", "") };//将读取的name值转成路径

                String tpath = Application.StartupPath + "\\.minecraft\\libraries\\" + tlib[0].Replace(".", "\\") + "\\" + tlib[1] + "\\" + tlib[2] + "\\" + tlib[1] + "-" + tlib[2] + ".jar";//同上
                if (libs[i].IndexOf("natives") != -1 && libs[i].IndexOf("windows") != -1)//如果有natives指定
                {
                    tmp = libs[i].IndexOf("windows") + "windows".Length + 3;//json读取部分，同上
                    tpath = tpath.Replace(".jar", "") + "-" + libs[i].Substring(tmp, libs[i].IndexOf("\"", tmp) - tmp) + ".jar";//json读取部分，同上
                }
                if (File.Exists(tpath))//检查文件是否存在
                {
                    libp = libp + tpath + ";";//存在就加录
                }
                else//如果不存在
                {
                    if (libs[i].IndexOf("url") == -1)//判断是否存在url
                    {//不存在
                        URLList.Add("https://libraries.minecraft.net" + tpath.Replace(Application.StartupPath + "\\.minecraft\\libraries", "").Replace("\\", "/") + ":::" + tpath);//加入到下载列表
                    }
                    else//存在
                    {
                        tmp = libs[i].IndexOf("url") + "url".Length + 3;//json读取部分，同上
                        String du = libs[i].Substring(tmp, libs[i].IndexOf("\"", tmp) - tmp);//读取到url
                        if (du == "http://files.minecraftforge.net/maven/")//如果是forge的
                        {
                            String tmpp = tpath;//声明临时url
                            if (tpath.IndexOf(@"net\minecraftforge\forge\") != -1)//如果是forge主文件
                            {
                                tmpp = tpath.Replace(".jar", "") + "-universal.jar";//加上universal后缀
                            }
                            URLList.Add(du.Substring(0, du.Length - 1) + tmpp.Replace(Application.StartupPath + "\\.minecraft\\libraries", "").Replace("\\", "/") + ":::" + tpath);//加入到下载列表
                        }
                        else//不是Forge的
                        {
                            if (du.Contains("https://libraries.minecraft.net"))
                            {
                                URLList.Add("https://libraries.minecraft.net" + tpath.Replace(Application.StartupPath + "\\.minecraft\\libraries", "").Replace("\\", "/") + ":::" + tpath);//加入到下载列表
                            }
                            else
                            {
                                if (libs[i].Contains("LiteLoaderFileName"))
                                {
                                    String Newtpath = "";
                                    tmp = libs[i].IndexOf("\"LiteLoaderFileName\":\"") + "\"LiteLoaderFileName\":\"".Length;
                                    String nfn = libs[i].Substring(tmp, libs[i].IndexOf("\"", tmp) - tmp);
                                    tmp = tpath.LastIndexOf("\\") + 1;
                                    Newtpath = tpath.Substring(0, tmp) + nfn;
                                    URLList.Add(du + Newtpath.Replace(Application.StartupPath + "\\.minecraft\\libraries\\", "").Replace("\\", "/") + ":::" + tpath);//加入到下载列表
                                }
                                else
                                {
                                    URLList.Add(du + tpath.Replace(Application.StartupPath + "\\.minecraft\\libraries\\", "").Replace("\\", "/") + ":::" + tpath);//加入到下载列表
                                }
                            }
                        }
                    }
                }
                if (libs[i].IndexOf("extract") != -1)//如果要提取natives
                {
                    decompress(tpath, natives);//提取
                }
            }
            String oldVerName = VerName;//转储版本号
            String recallar = MaxMem + ",%,%," + JavaPath + ",%,%," + UserName + ",%,%," + oldVerName;
            tmp = rtxt.IndexOf("assets\":") + "assets".Length + 3;//json读取部分，同上
            String assetIndex = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//json读取部分，同上
            if (rtxt.IndexOf("inheritsFrom\":") != -1)//如果是继承的json数据
            {
                tmp = rtxt.IndexOf("inheritsFrom\":") + "inheritsFrom".Length + 3;//json读取部分，同上
                String newVerName = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//json读取部分，同上
                LauncherReturn lr = Launch("", "", "", newVerName, 1);
                libp += lr.rtv;//读入父json数据生成的库，解压父json数据要求的natives
                tmp = rtxt.IndexOf("jar\":") + "jar".Length + 3;//json读取部分，同上
                VerName = newVerName;//转储
                natives = Application.StartupPath + "\\.minecraft\\versions\\" + newVerName + "\\" + newVerName + "-natives";//重新指定natives文件夹
                assetIndex = lr.rtvs[0];
            }
            if (rtxt.IndexOf("jar\":") != -1)//如果指出了主jar文件位置
            {
                tmp = rtxt.IndexOf("jar\":") + "jar".Length + 3;//json读取部分，同上
                VerName = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//修改版本变量备用
            }
            String assets = Application.StartupPath + "\\.minecraft\\assets";//设置资源文件夹路径
            if (rtxt.IndexOf("assetIndex\":") != -1 && returnvalue != 3 && returnvalue != 2)
            {
                tmp = rtxt.IndexOf("assetIndex\":") + "assetIndex".Length + 3;//json读取部分，同上
                String ajrurl = rtxt.Substring(tmp, rtxt.IndexOf("}", tmp) - tmp);//json读取部分，同上
                tmp = ajrurl.IndexOf("url\":") + "url".Length + 3;//json读取部分，同上
                String ajurl = ajrurl.Substring(tmp, ajrurl.IndexOf("\"", tmp) - tmp);//json读取部分，同上
                tmp = ajrurl.IndexOf("id\":") + "id".Length + 3;//json读取部分，同上
                String ai = ajrurl.Substring(tmp, ajrurl.IndexOf("\"", tmp) - tmp);//json读取部分，同上
                LauncherReturn lr = new LauncherReturn();//声明临时返回
                lr = dealWithAssets(ai, ajurl, 1);//处理资源
                if (lr.rtv == "资源文件不完整！" & dc == false)//如果资源不完整且尚未执行下载
                {
                    if (MessageBox.Show("资源文件不完整，是否补全？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)//弹窗提示用户
                    {//如果用户选择“是”
                        dealWithAssets(ai, ajurl);//添加要下载的资源文件
                    }
                }
                if (lr.rtv == "资源文件完整！" & File.ReadAllText(Application.StartupPath + "\\.minecraft\\assets\\indexes\\" + ai + ".json").Replace(" ", "").IndexOf("\"virtual\":true") != -1)//如果json文件要求virtual为真
                {
                    if (Directory.Exists(Application.StartupPath + "\\.minecraft\\assets\\virtual\\" + ai) == false)//虚拟文件夹不存在
                    {
                        Directory.CreateDirectory(Application.StartupPath + "\\.minecraft\\assets\\virtual\\" + ai);//为游戏创建虚拟文件夹
                        dealWithAssets(ai, ajurl, 2);//执行复制操作
                    }
                    assets = Application.StartupPath + "\\.minecraft\\assets\\virtual\\" + ai;//重设资源文件夹路径
                }
            }
            if (returnvalue == 1)//如果要返回库路径
            {
                LauncherReturn lr = new LauncherReturn();//声明临时返回
                lr.rtv = libp;//赋值
                lr.rtvs = new String[] { assetIndex };//赋值
                return lr;//返回并退出方法
            }
            if (URLList.Count != 0 && dc == false && returnvalue != 3 && returnvalue != 2)//如果下载列表不为空且尚未执行下载
            {
                ShowDownload sd = new ShowDownload();//引用下载进度窗体到实例
                sd.DownloadBL(URLList, recallar);//调用窗体中的下载方法并传参，但使用的是原版本号
                LauncherReturn lr = new LauncherReturn();//声明临时返回
                lr.sdd = sd;//寄存
                lr.LP = null;
                return lr;//返回空值
            }
            libp = libp + Application.StartupPath + "\\.minecraft\\versions\\" + VerName + "\\" + VerName + ".jar";//整合字符串
            String gameDir = Application.StartupPath + "\\.minecraft";//设置游戏路径
            if (Application.StartupPath.IndexOf(" ") != -1)//如果路径有空格
            {
                natives = "\"" + natives + "\"";//加上引号
                libp = "\"" + libp + "\"";//加上引号
                assets = "\"" + assets + "\"";//加上引号
                gameDir = "\"" + gameDir + "\"";//加上引号
            }
            if (!File.Exists(Application.StartupPath + "\\" + UserName + ".authL"))//如果没有离线数据
            {
                File.WriteAllText(Application.StartupPath + "\\" + UserName + ".authL", Guid.NewGuid().ToString().Replace("-", ""));//生成离线uuid储存
            }
            String uat = File.ReadAllText(Application.StartupPath + "\\" + UserName + ".authL");//读取凭据
            String[] auths = new String[] { UserName, uat, uat, "Legacy" };//声明默认登录凭据

            minecraftArguments = minecraftArguments.Replace("${auth_player_name}", auths[0]).Replace("${version_Type}", "JuicyLauncher_1.0").Replace("${version_name}", "JuicyLauncher_1.0").Replace("${game_directory}", gameDir).Replace("${game_assets}", assets).Replace("${assets_root}", assets).Replace("${assets_index_name}", assetIndex).Replace("${user_properties}", "{}").Replace("${user_type}", auths[3]).Replace("${auth_uuid}", auths[2]).Replace("${auth_access_token}", auths[1]) + ExtraMCArg;//读取额外参数
            //启动参数拼接
            String RunComm = "";//声明RunComm
            RunComm = ExtraJreArgs + "-Xms128m -Xmx" + MaxMem + "m -Djava.library.path=" + natives + " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -cp " + libp + " " + mainClass + " " + minecraftArguments;//拼接参数
            if (returnvalue == 2)//如果要返回完整的启动命令
            {
                LauncherReturn lr = new LauncherReturn();//声明临时返回
                lr.rtv = "\"" + JavaPath + "\" " + RunComm;//赋值
                return lr;//返回并退出
            }
            Process mjp = new Process();//运行部分
            ProcessStartInfo psi = new ProcessStartInfo(JavaPath, RunComm);//运行部分
            psi.UseShellExecute = false;//运行部分
            psi.WorkingDirectory = Application.StartupPath + "\\.minecraft";//运行部分
            psi.RedirectStandardOutput = true;//设置重定向
            mjp.StartInfo = psi;//运行部分
            mjp.Start();//运行部分
            File.CreateText(Application.StartupPath + "\\.minecraft\\versions\\" + oldVerName + "\\JLSelVer.sym").Close();//存储选择的版本
            if (returnvalue == 3)
            {
                LauncherReturn lr = new LauncherReturn();//声明临时返回
                lr.LP = mjp;//赋值
                return lr;//返回
            }
            Application.Exit();//退出
            return null;//最终返回空值
        }

        public void decompress(String inputFileName, String outputDirName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();//释放7z.exe和7z.dll部分
            Stream stream = assembly.GetManifestResourceStream("CustomLauncher.7z.exe");//释放7z.exe和7z.dll部分
            byte[] bytes = new byte[stream.Length];//释放7z.exe和7z.dll部分
            stream.Read(bytes, 0, int.Parse(stream.Length.ToString()));//释放7z.exe和7z.dll部分
            File.WriteAllBytes(Application.StartupPath + "\\7z.exe", bytes);//释放7z.exe和7z.dll部分
            assembly = Assembly.GetExecutingAssembly();//释放7z.exe和7z.dll部分
            stream = assembly.GetManifestResourceStream("CustomLauncher.7z.dll");//释放7z.exe和7z.dll部分
            bytes = new byte[stream.Length];//释放7z.exe和7z.dll部分
            stream.Read(bytes, 0, int.Parse(stream.Length.ToString()));//释放7z.exe和7z.dll部分
            File.WriteAllBytes(Application.StartupPath + "\\7z.dll", bytes); //释放7z.exe和7z.dll部分
            Process sz = new Process();//运行7z.exe解压部分
            ProcessStartInfo psi = new ProcessStartInfo(Application.StartupPath + "\\7z.exe", "x \"" + inputFileName + "\" -o\"" + outputDirName + "\" -y");//运行7z.exe解压部分
            psi.UseShellExecute = false;//运行7z.exe解压部分
            psi.WindowStyle = ProcessWindowStyle.Hidden;//设置不显示
            psi.CreateNoWindow = true;//设置不显示
            sz.StartInfo = psi;//运行7z.exe解压部分
            sz.Start();//运行7z.exe解压部分
            sz.WaitForExit();//等待退出
            File.Delete(Application.StartupPath + "\\7z.exe");//删除7z.exe
            File.Delete(Application.StartupPath + "\\7z.dll");//删除7z.dll
        }

        public LauncherReturn dealWithAssets(String AssetIndex, String JSONDownloadURL, int dealWay = 0)//声明资源文件处理公共函数过程
        {
            LauncherReturn lr = new LauncherReturn();//声明返回值
            String asset_JSON = Application.StartupPath + "\\.minecraft\\assets\\indexes\\" + AssetIndex + ".json";//准备json文件路径
            if (File.Exists(asset_JSON) == false && SystemInformation.Network == true)//如果数据文件不存在且连接到网络
            {
                try//尝试
                {
                    if (!Directory.Exists(asset_JSON.Replace(Path.GetFileName(asset_JSON), "")))
                    {
                        Directory.CreateDirectory(asset_JSON.Replace(Path.GetFileName(asset_JSON), ""));
                    }
                    new WebClient().DownloadFile(JSONDownloadURL, asset_JSON);//下载数据资源文件
                    do//反复等待
                    {
                        System.Threading.Thread.Sleep(10);//等待10毫秒
                    } while (!File.Exists(asset_JSON));//直到数据文件下载完成
                }
                catch (Exception)//出错
                {
                    return null;//返回
                }
            }
            string rtxt = "";//声明
            int tmp = 0;//声明
            rtxt = File.ReadAllText(asset_JSON).Replace(" ", "");//读取内容
            tmp = rtxt.IndexOf("objects") + "objects".Length + 3;//读取objects数组数据
            String objects_S = rtxt.Substring(tmp, rtxt.LastIndexOf("}", rtxt.Length - 1) - tmp);//同上
            String[] objects = objects_S.Replace("},", "$").Split("$".ToCharArray());//分割数组
            String tmu = "";//声明并初始化tmu
            switch (dealWay)//判断处理方式
            {
                case 0://为零
                    foreach (string oi in objects)//为每个项循环
                    {
                        tmp = oi.IndexOf("hash") + "hash".Length + 3;//读取hash值
                        tmu = oi.Substring(tmp, oi.IndexOf("\"", tmp) - tmp);//同上
                        tmu = Application.StartupPath + "\\.minecraft\\assets\\objects\\" + tmu.Substring(0, 2) + "\\" + tmu;//生成完整路径
                        tmp = oi.IndexOf("size") + "size".Length + 2;//读取文件大小
                        if ((!File.Exists(tmu)) || new FileInfo(tmu).Length < int.Parse(oi.Substring(tmp, oi.IndexOf("\n", tmp) - tmp)))//如果发现文件不完整
                        {
                            URLList.Add(tmu.Replace(Application.StartupPath + "\\.minecraft\\assets\\objects\\", "http://resources.download.minecraft.net/").Replace("\\", "/") + ":::" + tmu);//添加下载地址到列表备用
                        }
                    }
                    lr.rtv = "下载完成！";//返回
                    return lr;//返回
                case 1:
                    foreach (string oi in objects)//为每个项循环
                    {
                        tmp = oi.IndexOf("hash") + "hash".Length + 3;//读取hash值
                        tmu = oi.Substring(tmp, oi.IndexOf("\"", tmp) - tmp);//同上
                        tmu = Application.StartupPath + "\\.minecraft\\assets\\objects\\" + tmu.Substring(0, 2) + "\\" + tmu;//生成完整路径
                        tmp = oi.IndexOf("size") + "size".Length + 2;//读取文件大小
                        if ((!File.Exists(tmu)) || new FileInfo(tmu).Length < int.Parse(oi.Substring(tmp, oi.IndexOf("\n", tmp) - tmp)))//如果发现文件不完整
                        {
                            lr.rtv = "资源文件不完整！";//返回
                            return lr;//返回
                        }
                    }
                    lr.rtv = "资源文件完整！";//返回
                    return lr;//返回
                case 2:
                    foreach (string oi in objects)//为每个项循环
                    {
                        tmp = oi.IndexOf("hash") + "hash".Length + 3;//读取hash值
                        tmu = oi.Substring(tmp, oi.IndexOf("\"", tmp) - tmp);//同上
                        tmu = Application.StartupPath + "\\.minecraft\\assets\\objects\\" + tmu.Substring(0, 2) + "\\" + tmu;//生成完整路径
                        String fp = Application.StartupPath + "\\.minecraft\\assets\\virtual\\" + AssetIndex + "\\" + oi.Substring(2, oi.IndexOf("\"", 2) - 2).Replace("/", "\\");//生成目标文件完整路径
                        if (!Directory.Exists(fp.Replace(Path.GetFileName(fp), "")))//如果目标文件所在文件夹不存在
                        {
                            Directory.CreateDirectory(fp.Replace(Path.GetFileName(fp), ""));//创建
                        }
                        File.Copy(tmu, fp);//复制文件
                    }
                    return lr;//返回
            }
            return null;//返回空值
        }
        public LauncherReturn YggLogin(String Email, String Password)
        {
            WebRequest wr = WebRequest.Create("https://authserver.mojang.com/authenticate");//创建请求对象
            wr.ContentType = "application/json";//定义Content-Type
            wr.Method = "post";//定义请求类型
            String uls = "{    \"agent\": {        \"name\": \"Minecraft\",        \"version\": 1 },\"username\": \"" + Email + "\",     \"password\": \"" + Password + "\"}";//处理数据
            byte[] bs = Encoding.UTF8.GetBytes(uls);//上传数据部分
            wr.ContentLength = bs.Length;//上传数据部分
            Stream sw = wr.GetRequestStream();//上传数据部分
            sw.Write(bs, 0, bs.Length);//上传数据部分
            sw.Flush();//上传数据部分
            sw.Close();//上传数据部分
            LauncherReturn lr = new LauncherReturn();//定义返回量
            StreamReader sr = new StreamReader(wr.GetResponse().GetResponseStream());//读取返回数据部分
            string rtxt = sr.ReadToEnd();//读取返回数据部分
            int tmp = rtxt.IndexOf("accessToken") + "accessToken".Length + 3;//读取json部分
            String accessToken = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//读取json部分
            tmp = rtxt.IndexOf("id") + "id".Length + 3;//读取json部分
            String uuid = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//读取json部分
            tmp = rtxt.IndexOf("name") + "name".Length + 3;//读取json部分
            String UserName = rtxt.Substring(tmp, rtxt.IndexOf("\"", tmp) - tmp);//读取json部分
            String legacy = "Legacy";//定义Legacy
            if (rtxt.IndexOf("legacy") == -1)
            {//如果有说明是Legacy
                legacy = "mojang";//设置
            }
            lr.rtvs = new String[] { UserName, accessToken, uuid, legacy };//赋值
            return lr;//返回
        }
    }
}
