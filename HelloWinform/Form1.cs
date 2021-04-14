using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinform
{
    public partial class Form1 : Form
    {
        /*public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(sender.Equals(button1))
            {
                MessageBox.Show("첫 번째 버튼 와우!");
            }
            else
            {
                MessageBox.Show("두 번째 버튼 와우!");
            }
        }*/
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        private ChromeDriver _driver = null;

        public Form1()
        {
            InitializeComponent();

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = tboxID.Text;
            string pw = tboxPW.Text;

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://www.naver.com"); // 웹 사이트에 접속합니다.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var element = _driver.FindElementByXPath("//*[@id='account']/a"); //Main 로그인 버튼
            element.Click();

            Thread.Sleep(3000);

            element = _driver.FindElementByXPath("//*[@id='id']"); // ID 입력창
            element.SendKeys(id);

            element = _driver.FindElementByXPath("//*[@id='pw']"); // PW 입력창
            element.SendKeys(pw);

            element = _driver.FindElementByXPath("//*[@id='log.login']"); // 로그인 버튼
            element.Click();
        }

        List<string> Lsrc = null; // IMG URL
        int i = 0; // 현재 배열 위치

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strURL = "https://www.google.com/search?q=" + tboxSearch.Text + "&source=lnms&tbm=isch";

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl(strURL); // 웹 사이트에 접속합니다.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _driver.ExecuteScript("window.scrollBy(0, 10000)"); // 창을 띄우고 스크롤 진행

            Lsrc = new List<string>();

            foreach (IWebElement item in _driver.FindElementsByClassName("rg_i"))
            {
                if (item.GetAttribute("src") != null)
                    Lsrc.Add(item.GetAttribute("src"));
            }

            lblTotal.Text = "/ " + Lsrc.Count.ToString();

            this.Invoke(new Action(delegate ()
            {
                try
                {
                    foreach (string strsrc in Lsrc)
                    {
                        i++;

                        GetMapimage(Lsrc[i]);
                        tboxNow.Text = i.ToString();
                        Refresh();
                        Thread.Sleep(50);
                    }
                }
                catch { }
            }));
        }

        private void GetMapimage(string base64String)
        {
            try
            {
                var base64Dats = System.Text.RegularExpressions.Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;  // 정규식 검색
                var binData = Convert.FromBase64String(base64Dats);

                using (var streas = new MemoryStream(binData))
                {
                    if(streas.Length == 0)
                    {
                        pboxMain.Load(base64String);
                        tboxNow.Text = i.ToString();
                        tboxUrl.Text = base64String;
                    }
                    else
                    {
                        var image = Image.FromStream(streas);
                        pboxMain.Image = image;
                        tboxUrl.Text = base64String;
                    }
                }
            }
            catch { }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i--;

                GetMapimage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));    
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i = int.Parse(tboxNow.Text);

                GetMapimage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i++;

                GetMapimage(Lsrc[i]);
                tboxNow.Text = i.ToString();
            }));
        }
    }
}
