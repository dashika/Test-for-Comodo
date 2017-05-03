using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using test.Model;

namespace test
{
    class ReadFitmentsDB
    {
        private readonly BackgroundWorker readFirments;
        ObservableCollection<Fitment> fitments;
        int i = 1, j = 0;
        Fitment fitment;
        String str;
        TextBlock statusBar;


        public ReadFitmentsDB(DataGrid dg, TextBlock statusBar)
        {
            this.statusBar = statusBar;
            readFirments = new BackgroundWorker();
            str = dg.Tag.ToString();
            fitments = new ObservableCollection<Fitment>();
            dg.ItemsSource = fitments;
            readFirments.DoWork += new DoWorkEventHandler(readFirments_DoWork);
            readFirments.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readFirments_RunWorkerCompleted);
            if (readFirments.IsBusy != true)
                readFirments.RunWorkerAsync();
        }

        private void readFirments_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusBar.Text = "Ready";
            if (fitment != null)
            {
                fitments.Add(fitment);
                if (readFirments.IsBusy != true)
                {
                    readFirments.RunWorkerAsync();
                    statusBar.Text = "Reading fitments from db";
                }
            }
        }

        private void readFirments_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var db = new ProductDBEntitie())
            {
                fitment = (from c in db.Fitment where c.sku == str orderby c.sku select c).Skip(i * j++).Take(i).FirstOrDefault();
                if (fitment != null)
                {
                    fitment.Make = (from c in db.Make where c.id == fitment.id_make select c).FirstOrDefault();
                    fitment.BrandName = (from c in db.BrandName where c.id == fitment.id_brand_name select c).FirstOrDefault();
                }
            }
        }
    }
}
