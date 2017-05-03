using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using test.Model;
using System.Configuration;

namespace test
{
    class BackgroundInitProduct
    {
        private readonly BackgroundWorker readCSV = new BackgroundWorker();
        private DataGrid dt;
        private ObservableCollection<Product> products;
        private Product product;
        private string resource_data;
        private string[] lines;
        private string[] headers;
        private int i = 1;
        private int linesCount;
        private TextBlock statusBar;
        private bool resourceFile;
        private string key = "productProgress";

        public delegate void MyEventHandler(object sender, MyEventArgs e);
        public event MyEventHandler OnComplite;

        private void isBusy()
        {
            if (readCSV.IsBusy != true)
            {
                readCSV.RunWorkerAsync();
                statusBar.Text = "Reading products from Products.csv" + (double)i / linesCount * 100 + "%";
            }
            if (resourceFile)
            {
                try
                {
                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;

                    if (settings[key] == null)
                    {
                        settings.Add(key, i.ToString());
                    }
                    else
                    {
                        settings[key].Value = i.ToString();
                    }
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                }
                catch (ConfigurationErrorsException)
                { }
            }
        }

        public BackgroundInitProduct(ObservableCollection<Product> product, TextBlock statusBar)
        {
            resourceFile = true;
            this.products = product;
            this.statusBar = statusBar;
            resource_data = Properties.Resources.Products;
            lines = resource_data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            headers = lines[0].Split('|');
            linesCount = lines.Count();

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? null;
                if (result != null)
                {
                    i = Convert.ToInt32(result);
                }

            }
            catch (ConfigurationErrorsException)
            { }
            if (i < linesCount)
            {
                readCSV.DoWork += new DoWorkEventHandler(readCSV_DoWork);
                readCSV.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readCSV_RunWorkerCompleted);
                isBusy();
            }
        }

        public BackgroundInitProduct(ObservableCollection<Product> product, TextBlock statusBar, string[] lines)
        {
            resourceFile = false;
            this.products = product;
            this.statusBar = statusBar;
            this.lines = lines;
            headers = lines[0].Split('|');
            linesCount = lines.Count();

            readCSV.DoWork += new DoWorkEventHandler(readCSV_DoWork);
            readCSV.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readCSV_RunWorkerCompleted);
            isBusy();
        }

        private void readCSV_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusBar.Text = "Ready";

            if (product != null)
                this.products.Add(product);
            if (++i < linesCount)
            {
                isBusy();
            }
            else
            {
                OnComplite(this, new MyEventArgs((double)i / linesCount * 100));
            }

        }


        private void init_product()
        {
            product = new Product();
            Additionally0 additionally0 = new Additionally0();
            Additionally additionally = new Additionally();
            ForName forName = new ForName();
            string[] values = lines[i].Split('|');
            for (int j = 0; j < values.Count(); j++)
            {
                if (headers[j].Equals("IMG_1"))
                {
                    Resource res = new Resource();
                    res.img1 = values[j++];
                    try
                    {
                        if (headers[j].Equals("IMG_2"))
                            res.img2 = values[j++];
                        if (headers[j].Equals("IMG_3"))
                            res.img3 = values[j++];
                        if (headers[j].Equals("IMG_4"))
                            res.img4 = values[j++];
                        if (headers[j].Equals("PDF_1"))
                            res.pdf = values[j++];
                        if (headers[j].Equals("Icon"))
                            res.pdf = values[j++];
                    }
                    catch (IndexOutOfRangeException e) { }
                    using (var db = new ProductDBEntitie())
                    {
                        db.Resource.Add(res);
                        db.SaveChanges();
                        additionally.Resource = res;
                        additionally.id_resource = res.id;
                    }

                    continue;
                }


                if (headers[j] == "forname1")
                {
                    forName.forname1 = values[j];
                    forName.forname2 = values[j + 1];
                    using (var db = new ProductDBEntitie())
                    {
                        try
                        {
                            db.ForName.Add(forName);
                            db.SaveChanges();
                        }
                        catch
                        {
                            string str = values[j + 1];
                            forName = (from c in db.ForName where c.forname2.Equals(str) select c).FirstOrDefault();
                        }
                        additionally0.id_forname = forName.id;
                        additionally0.ForName = (ForName)forName;
                    }
                    j++;
                }
                if (headers[j] == "forname2")
                {
                    j++;
                }
                try
                {
                    if (!product.init_product(headers[j], values[j], ref additionally0))
                        product.init_product(headers[j], values[j], ref additionally);
                }
                catch { }
            }
            using (var db = new ProductDBEntitie())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (additionally0.ForName != null)
                        {
                            additionally0.ForName = null;
                            db.Additionally0.Add(additionally0);
                            db.SaveChanges();
                            product.id_additionally0 = additionally0.id;
                            product.Additionally0 = additionally0;
                        }
                    }
                    catch { }

                    try
                    {
                        if (additionally.Resource != null)
                        {

                            Resource res = additionally.Resource;
                            additionally.Resource = null;
                            db.Additionally.Add(additionally);
                            db.SaveChanges();
                            additionally.Resource = res;
                            product.id_additionally = additionally.id;
                            product.Additionally = additionally;
                        }
                    }
                    catch { }

                    try
                    {
                        BrandName drand = product.BrandName;
                        product.BrandName = null;
                        product.Additionally = null;
                        product.Additionally0 = null;
                        db.Product.Add(product);
                        db.SaveChanges();
                        product.BrandName = drand;
                        additionally0.ForName = forName;
                        product.Additionally0 = additionally0;
                        product.Additionally = additionally;
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        product = null;
                        transaction.Rollback();
                    }
                }
            }
        }

        private void readCSV_DoWork(object sender, DoWorkEventArgs e)
        {
            init_product();         

        }
    }
}
