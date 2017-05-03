using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using test.Model;

namespace test
{
    class BackgroundInitFitment
    {
        private readonly BackgroundWorker readCSV = new BackgroundWorker();
        //private ObservableCollection<Fitment> fitments;
        private Fitment fitment;
        private string resource_data;
        private string[] lines;
        private string[] headers;
        private int i = 1;
        private int linesCount;
        private TextBlock statusBar;
        private bool resourceFile;
        private string key = "fitmentProgress";

        public delegate void MyEventHandler(object sender, MyEventArgs e);
        public event MyEventHandler OnComplite;

        private void isBusy()
        {
            if (readCSV.IsBusy != true)
            {
                readCSV.RunWorkerAsync();
                statusBar.Text = "Reading fitments from Fitments.csv " + (double)i/linesCount*100 + "%";
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

        public BackgroundInitFitment(TextBlock statusBar)
        {
            resourceFile = true;
            this.statusBar = statusBar;
            resource_data = Properties.Resources.Fitments;
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

        public BackgroundInitFitment(TextBlock statusBar, String [] lines)
        {
            resourceFile = false;
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
            if (++i < linesCount)
            {
                isBusy();
            }
            else
            {
                OnComplite(this, new MyEventArgs((double)i / linesCount * 100));
            }
        }


        private void init_fitment()
        {
            fitment = new Fitment();
            Make make = new Make();
            string[] values = lines[i].Split('|');
            for (int j = 0; j < values.Count(); j++)
            {
                if (headers[j] == "Make")
                {
                    make.make1 = values[j];
                }
                else if (headers[j] == "Model")
                {
                    make.model = values[j];
                }
                else if (headers[j] == "ourMake")
                {
                    make.our_make = values[j];
                }
                else if (headers[j] == "ourMake")
                {
                    make.our_make = values[j];
                }
                else if (headers[j] == "ourModel")
                {
                    make.our_model = values[j];
                }
                else if (headers[j] == "_BodyTypeName_")
                {
                    make.body_type_name = values[j];
                }
                else if (headers[j] == "_MfrBodyCodeName_")
                {
                    make.mf_body_code_name = values[j];
                }
                else if (headers[j] == "ourBodyTypeName")
                {
                    make.our_body_type_name = values[j];
                }
                else
                    try
                    {
                        fitment.init_fitment(headers[j], values[j]);
                    }
                    catch { }
            }
            using (var db = new ProductDBEntitie())
            {
                
                    try
                    {
                        var make_ = (from c in db.Make
                                     where c.make1 == make.make1 && c.model == make.model && c.our_make == make.our_make && c.body_type_name == make.body_type_name
                                     select c).ToList();
                        if (make_.Count() == 0)
                        {
                            db.Make.Add(make);
                            db.SaveChanges();
                            fitment.id_make = make.id;
                            fitment.Make = make;
                        }
                        else
                        {
                            fitment.id_make = make_.First().id;
                            fitment.Make = make_.First();
                        }
                    }
                    catch { }

                    fitment.Product = (from c in db.Product where c.sku == fitment.sku select c).FirstOrDefault();
                    try
                    {
                        BrandName brand = fitment.BrandName;
                        fitment.BrandName = null;
                        Make m = fitment.Make;
                        fitment.Make = null;
                        db.Fitment.Add(fitment);
                        db.SaveChanges();
                        fitment.Make = m;
                        fitment.BrandName = brand;
                    }
                    catch (Exception e) {
                       
                    }
                
            }

        }

        private void readCSV_DoWork(object sender, DoWorkEventArgs e)
        {
            init_fitment();
        }
    }
}
