using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebView2.DevTools.Dom;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using static Microsoft.Web.WebView2.Core.DevToolsProtocolExtension.Network;
using System.Threading;
using System.Net;
using System.Security.Policy;
using System.Drawing.Imaging;
using System.IO;
using System.Data.SqlClient;
using System.Globalization;
using CsvHelper;
using System.Collections.Concurrent;
using static Microsoft.Web.WebView2.Core.DevToolsProtocolExtension.DOM;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using Dapper;
using System.Text.RegularExpressions;

namespace Watch_Importer
{
    public partial class Form1 : Form
    {
        bool stat = false;
        string name = "";
        string allN = "", firstI = "", Bran = "", Seri = "", sku = "", desc = "", msrp = "", upc = "", title = "";
        int stck = 7000,globalCount=0;
        ConcurrentQueue<watches> listc = new ConcurrentQueue<watches>();
        List<watch> wtch = new List<watch>();
        List<watches> NoData = new List<watches>();
        public Form1()
        {
            InitializeComponent();
            //https://paradoxfwc.com/?s=GW-8230B-9ACR&post_type=product
            AttachControlEventHandlers(this.WV);
            //WV.Source=new Uri("C:\\home\\mdcdiamonds.com\\wwwroot\\images\\ProductImages\\watches\\");
            //WV.Source=new Uri("https://paradoxfwc.com/?s=GW-8230B-9ACR&post_type=product");
            //WV.Source=new Uri("D:\\Kaiser's Drive\\OneDrive\\VisualStudio\\source\\repos\\Watch Importer\\Watch Importer\\bin\\Debug\\output\\");
        }
        void readCSV()
        {
            try
            {
                using (var reader = new StreamReader("watches.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<watches>();
                    List<watches> watches = new List<watches>();
                    foreach (var record in records)
                    {
                        listc.Enqueue(record);
                    }

                }
                watches reslt;
                listc.TryDequeue(out reslt);
                name = reslt.SKU_Code.Replace("-", "");
                Bran = reslt.Brand;
                desc = reslt.Description;
                msrp = reslt.MSRP;
                upc = reslt.UPC;
                sku = reslt.SKU_Code;
                Seri = reslt.Series;
                WV.Source = new Uri("https://paradoxfwc.com/?s=" + reslt.SKU_Code + "&post_type=product");

            }
            catch (Exception ex)
            {
                textBox2.AppendText(ex.Message);
                textBox2.AppendText(Environment.NewLine);
                MessageBox.Show(ex.Message);
            }
        }
        void loadPage()
        {
            textBox2.AppendText("Items remaining: ");
            textBox2.AppendText(listc.Count.ToString());
            textBox2.AppendText(Environment.NewLine);
            if (listc.Count == 0) 
            {
                inserttoDb(); 
                return; 
            }
            watches reslt;
            listc.TryDequeue(out reslt);
            name = reslt.SKU_Code.Replace("-", "");
            Bran = reslt.Brand;
            desc = reslt.Description;
            msrp = reslt.MSRP;
            upc = reslt.UPC;
            sku = reslt.SKU_Code;
            Seri = reslt.Series;

            WV.Source = new Uri("https://paradoxfwc.com/?s=" + reslt.SKU_Code + "&post_type=product");

        }
        void AttachControlEventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.CoreWebView2InitializationCompleted += WebView2Control_CoreWebView2InitializationCompleted;
            control.NavigationStarting += WebView2Control_NavigationStarting;
            control.NavigationCompleted += WebView2Control_NavigationCompleted;
            control.SourceChanged += WebView2Control_SourceChanged;
            //control.KeyDown += WebView2Control_KeyDown;
            //control.KeyUp += WebView2Control_KeyUp;
        }
        #region Event Handlers
        private void WebView2Control_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            //WV.Visible = false;
            //label1.Visible = true;
            //UpdateTitleWithEvent("NavigationStarting");
        }

        private async void WebView2Control_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var devToolsContext = await WV.CoreWebView2.CreateDevToolsContextAsync();
            try
            {
               var html = await WV.ExecuteScriptAsync("document.documentElement.outerHTML;");
                html = Regex.Unescape(html);
                html = html.Remove(0, 1);
                html = html.Remove(html.Length - 1, 1);
                if(html.Contains("Search Results for")) 
                {
                    watches tmp = new watches();
                    tmp.SKU_Code = sku;
                    NoData.Add(tmp);
                    label4.Text = NoData.Count().ToString();
                    textBox2.AppendText(sku);
                    textBox2.AppendText(Environment.NewLine);
                    goto nod;
                }
                else 
                {
                    
                    var nclasses = await devToolsContext.QuerySelectorAllAsync<WebView2.DevTools.Dom.HtmlHeadingElement>(".section-title.section-title--small.color-blue-dark");
                    var nclass = await nclasses[0].GetInnerHtmlAsync();
                    title = nclass.ToString();
                    var classes = await devToolsContext.QuerySelectorAllAsync<HtmlDivElement>(".slider-prod__slide-img");
                    int i = 1;
                    foreach (var classesItem in classes)
                    {
                        string fname = "";
                        var res = await classesItem.GetInnerHtmlAsync();
                        html = res.ToString();
                        var url = RetLine(html, 0, "<img src=\"", "\" alt");
                        if (url == "") goto skp;
                        if (url.Contains("webp")) goto skp;

                        using (WebClient webClient = new WebClient())
                        {
                            byte[] data = webClient.DownloadData(url);

                            using (MemoryStream mem = new MemoryStream(data))
                            {
                                using (var yourImage = System.Drawing.Image.FromStream(mem))
                                {
                                    // If you want it as Png
                                    //yourImage.Save(@"image" + i.ToString(), ImageFormat.Png);

                                    // If you want it as Jpeg
                                    fname = name + "_" + i.ToString() + ".jpg";
                                    allN += fname + ";";
                                    yourImage.Save(textBox1.Text + fname, ImageFormat.Jpeg);
                                }
                            }

                        }
                    skp:
                        if (String.IsNullOrEmpty(firstI)) { firstI = fname; }
                        i++;
                        
                    }
                    allN.Remove(allN.Length - 1, 1);
                    watch tmp = new watch();
                    tmp.Title = String.IsNullOrEmpty(title) ? "" : title;
                    tmp.StockNumber = "MDCW" + stck.ToString();
                    tmp.Brand = String.IsNullOrEmpty(Bran) ? "" : Bran;
                    tmp.Series = String.IsNullOrEmpty(Seri) ? "" : Seri;
                    tmp.Description = String.IsNullOrEmpty(desc) ? "" : desc;
                    tmp.SupplierStock = String.IsNullOrEmpty(sku) ? "" : sku;
                    tmp.UPC = String.IsNullOrEmpty(upc) ? "" : upc;
                    tmp.Price = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(String.IsNullOrEmpty(msrp) ? "0.0" : msrp.Replace(",",""))));
                    tmp.Image1 = String.IsNullOrEmpty(firstI) ? "" : firstI;
                    tmp.Image2 = String.IsNullOrEmpty(allN) ? "" : allN;
                    wtch.Add(tmp);
                    stck += 1;
                }
                
            }
            catch (Exception ex) {
                textBox2.AppendText(ex.Message);
                textBox2.AppendText(Environment.NewLine);
                MessageBox.Show(ex.Message);
            }
        nod:
            allN = "";
            firstI = "";
            await devToolsContext.DisposeAsync();
            globalCount += 1;
            label2.Text = globalCount.ToString();
            loadPage();

            //var result = await WV.ExecuteScriptAsync("var images=[];const collection = document.getElementsByClassName(\"slider-prod__slide-img\");for (let i = 0; i < collection.length; i++){images[i]=collection[i].firstElementChild.src;}return images;");
            //MessageBox.Show(classes);
        }

        private void WebView2Control_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            //txtUrl.Text = WV.Source.AbsoluteUri;
        }

        private void WebView2Control_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WebView2 creation failed with exception = {e.InitializationException}");
                return;
            }

            //this.WV.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            //this.WV.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            //this.WV.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            this.WV.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);

        }


        #endregion
        public string RetLine(string mStr, int sIndex = 0, string fStr = "", string lStr = "", string fndStr = "", bool inclFs = false, bool inclLs = false, bool RetAll = false)
        {
            string result;
            int fs;
            int ls;

            try
            {
                if (fndStr != "")
                {
                    sIndex = mStr.IndexOf(fndStr, sIndex);
                    if (sIndex == -1)
                        throw new Exception();
                }

                if (fStr != "")
                {
                    fs = mStr.IndexOf(fStr, sIndex);
                    if (fs == -1)
                        throw new Exception();

                    if (inclFs == false)
                        fs = fs + fStr.Length;
                }
                else
                    fs = sIndex;

                if (lStr != "")
                {
                    ls = mStr.IndexOf(lStr, fs);
                    if (ls == -1)
                        throw new Exception();

                    if (inclLs == true)
                        ls = ls + lStr.Length;

                    result = mStr.Substring(fs, ls - fs);
                }
                else
                    result = mStr.Substring(fs);
            }
            catch (Exception ex)
            {
                if (RetAll == true)
                    result = mStr;
                else
                    result = "";
            }
            return result;
        }
        private void inserttoDb(){
            try
            {
                //Insert into mdcdiamo.Watches (Title, Brand, Description, Series, StockNumber, Image1, Image2, Price,SupplierStock, UPC) Values (@Title, @Brand, @Description, @Series, @StockNumber, @Image1, @Image2,@Price,@SupplierStock, @UPC)

                string sql = "Insert into mdcdiamo.Watches (Title, Brand, Description, Series, StockNumber, Image1, Image2, Price,SupplierStock, UPC) Values (@Title, @Brand, @Description, @Series, @StockNumber, @Image1, @Image2,@Price,@SupplierStock, @UPC)";
                using (IDbConnection cnn = new SqlConnection(getCon()))
                {
                    cnn.Execute(sql, wtch);
                }
                
            }
            catch(Exception ex)
            {
                textBox2.AppendText(ex.Message);
                textBox2.AppendText(Environment.NewLine);
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            readCSV();
            button1.Enabled = false;
        }
        public static string getCon()
        {
            string StrCon = @"Data Source=MDCDIAMO\SQLEXPRESS;Initial Catalog=mdc;Persist Security Info=True;User ID=mdcdaiamo;Password=MdcSql#4LoGin532";
            if (Environment.MachineName == "KAISER")
                StrCon = @"Data Source=KAISER\SQLEXPRESS;Initial Catalog=mdc;Persist Security Info=True;User ID=mdcdaiamo;Password=MdcSql#4LoGin532";
            return StrCon;
        }
    }
    
}
