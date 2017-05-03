
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using test.Model;

namespace test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Product> products;
        private List<String> deleteProduct = new List<string>();
        private readonly BackgroundWorker readProduct;
        private int counter = 0;
        private int skip = 10;
        private int canCloseApp=0;
        private bool clickedClose = false;

        public MainWindow()
        {
            InitializeComponent();

            products = new ObservableCollection<Product>();
            DataTable.ItemsSource = products;

            readProduct = new BackgroundWorker();
           // BackgroundInitProduct biProduct = new BackgroundInitProduct(products, statusBar);
           // BackgroundInitFitment bi = new BackgroundInitFitment(statusBar);

            readProduct.DoWork += new DoWorkEventHandler(readProduct_DoWork);
            readProduct.RunWorkerCompleted += new RunWorkerCompletedEventHandler(readProduct_RunWorkerCompleted);
            if (readProduct.IsBusy != true)
            {
                readProduct.RunWorkerAsync();
                statusBar.Text = "Read from customs files products and fitments";
            }
        }

        private void dataGrid_XML_Initialized(object sender, EventArgs e)
        {
            if ((sender as DataGrid).Tag != null)
            {
                statusBar.Text = "Reading fitments from db";
                new ReadFitmentsDB((sender as DataGrid), statusBar);
            }
        }

        private void readProduct_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusBar.Text = "Ready";
            try {
                var res = (e.Result as List<Product>);
                if (res.Count > 0)
                {
                    foreach (var p in res)
                    {
                        products.Add(p);
                    }
                    if (readProduct.IsBusy != true)
                    {
                        readProduct.RunWorkerAsync();
                        statusBar.Text = "Reading product from db";
                    }
                }
            }catch(System.Reflection.TargetInvocationException ex)
            { }
        }

        private void readProduct_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var db = new ProductDBEntitie())
            {
                try
                {
                    var products = (from c in db.Product orderby c.sku select c).Skip(skip * counter++).Take(skip).ToList();
                    foreach (var p in products)
                    {
                        p.BrandName = (from c in db.BrandName where p.id_brand_name == c.id select c).FirstOrDefault();
                        p.Additionally = (from c in db.Additionally where p.id_additionally == c.id select c).FirstOrDefault();
                        p.Additionally0 = (from c in db.Additionally0 where p.id_additionally0 == c.id select c).FirstOrDefault();
                        if (p.Additionally != null)
                            p.Additionally.Resource = (from c in db.Resource where p.Additionally.id == c.id select c).FirstOrDefault();
                        if (p.Additionally0 != null)
                            p.Additionally0.ForName = (from c in db.ForName where p.Additionally0.id_forname == c.id select c).FirstOrDefault();
                    }
                    e.Result = products;
                }
                catch (System.Data.SqlClient.SqlException ex) {
                    MessageBox.Show("SQLServer maybe stoped. \n" + ex.Message.ToString());
                }
            }
        }

        #region delete
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete selected product?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var item in deleteProduct)
                {
                    using (var db = new ProductDBEntitie())
                    {
                        db.Product.Remove((from c in db.Product where c.sku == item select c).FirstOrDefault());
                        db.SaveChanges();
                        products.Remove((from c in products where c.sku == item select c).FirstOrDefault());
                    }
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
       where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        private void DeleteFitment_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete selected fitments?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new ProductDBEntitie())
                {
                    DataGridRow row = (DataGridRow)(DataTable.ItemContainerGenerator.ContainerFromItem(DataTable.SelectedItem));
                    DataGridDetailsPresenter presenter = FindVisualChild<DataGridDetailsPresenter>(row);
                    DataTemplate template = presenter.ContentTemplate;
                    DataGrid dg = (DataGrid)template.FindName("datagridf", presenter);
                    DataGridCheckBoxColumn cb = (DataGridCheckBoxColumn)template.FindName("NOf", presenter);
                    List<int> deleteFitment = new List<int>();
                    foreach (Fitment f in dg.ItemsSource)
                    {
                        try
                        {
                            if (((CheckBox)cb.GetCellContent(f)).IsChecked == true)
                            {
                                deleteFitment.Add(f.id);
                                Fitment fitment = (from c in db.Fitment where c.id == f.id select c).FirstOrDefault();
                                if (fitment != null)
                                {
                                    db.Fitment.Remove(fitment);
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch { }
                    }


                    new ReadFitmentsDB(dg, statusBar);
                }

            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)e.OriginalSource;
            deleteProduct.Add((checkBox.DataContext as Product).sku);
            e.Handled = true;
        }

        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)e.OriginalSource;
            deleteProduct.Remove((checkBox.DataContext as Product).sku);
            e.Handled = true;
        }
        #endregion

      
        #region load from file
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV Files|*.csv";
            openFileDialog1.Title = "Select a CSV File";
           
            if (openFileDialog1.ShowDialog() ?? false)
            {
                var lines = File.ReadAllLines(openFileDialog1.FileName);
                canCloseApp++;
                BackgroundInitProduct biProduct = new BackgroundInitProduct(products, statusBar, lines);
                biProduct.OnComplite += BiProduct_OnComplite;
            }
        }

        private void Load_Fitment_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV Files|*.csv";
            openFileDialog1.Title = "Select a CSV File";

            if (openFileDialog1.ShowDialog() ?? false)
            {
                var lines = File.ReadAllLines(openFileDialog1.FileName);
                canCloseApp++;
                BackgroundInitFitment biFitment = new BackgroundInitFitment(statusBar, lines);
                biFitment.OnComplite += BiFitment_OnComplite;
            }
        }
        #endregion
        #region close app
        private bool CloseApp()
        {
            if (canCloseApp <= 0)
                if (clickedClose)
                {
                    if (MessageBox.Show("Do you want to exit?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                        return true;
                    }
                    else clickedClose = false;
                }
            return false;
        }

        private void BiFitment_OnComplite(object sender, MyEventArgs e)
        {
            canCloseApp--;
            CloseApp();
        }

        private void BiProduct_OnComplite(object sender, MyEventArgs e)
        {
            canCloseApp--;
            CloseApp();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            clickedClose = true;
            e.Cancel = !CloseApp();
            if(canCloseApp > 0)
            if (MessageBox.Show("Loading file not comlite all. Do you want to exit?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
                e.Cancel = false;
            }
            else clickedClose = false;
        }
        #endregion

        private void DataTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TextBox t = e.EditingElement as TextBox;
            String value = t.Text.ToString();
            String header  = e.Column.Header.ToString();
            var sku = (e.Row.DataContext as Product).sku;
            /// additionally additionally0
            /// 
            if (sku != null)
            {
                using (var db = new ProductDBEntitie())
                {
                    var product = (from c in db.Product where c.sku == sku select c).FirstOrDefault();
                    if (product != null)
                    {
                        var additionally = (from c in db.Additionally where c.id == product.id_additionally select c).FirstOrDefault();
                        var additionally0 = (from c in db.Additionally0 where c.id == product.id_additionally0 select c).FirstOrDefault();

                        if (additionally == null)
                        {
                            additionally = new Additionally();
                            db.Additionally.Add(additionally);
                            db.SaveChanges();
                            product.id_additionally = additionally.id;
                            db.SaveChanges();
                        }
                            Resource res = (from c in db.Resource where c.id == additionally.id_resource select c).FirstOrDefault();

                            if (header.Equals("IMG_1"))
                                res.img1 = value;
                            if (header.Equals("IMG_2"))
                                res.img2 = value;
                            if (header.Equals("IMG_3"))
                                res.img3 = value;
                            if (header.Equals("IMG_4"))
                                res.img4 = value;
                            if (header.Equals("PDF_1"))
                                res.pdf = value;
                            if (header.Equals("Icon"))
                                res.pdf = value;

                            db.Resource.Add(res);
                            db.SaveChanges();
                            additionally.Resource = res;
                            additionally.id_resource = res.id;
                            db.SaveChanges();


                        if (additionally0 == null)
                        {
                            additionally0 = new Additionally0();
                            db.Additionally0.Add(additionally0);
                            db.SaveChanges();
                            product.id_additionally0 = additionally0.id;
                            db.SaveChanges();
                        }
                        var forName = (from c in db.ForName where c.id == additionally0.id_forname select c).FirstOrDefault();
                        if (header == "FORNAME1")
                        {
                            forName.forname1 = value;
                            try
                            {
                                db.ForName.Add(forName);
                                db.SaveChanges();
                            }
                            catch
                            {
                                string str = value;
                                forName = (from c in db.ForName where c.forname2.Equals(str) select c).FirstOrDefault();
                            }
                            additionally0.id_forname = forName.id;
                            additionally0.ForName = (ForName)forName;
                            db.SaveChanges();
                            return;
                        }
                        if (header == "FORNAME2")
                        {
                            forName.forname2 = value;
                            try
                            {
                                db.ForName.Add(forName);
                                db.SaveChanges();
                            }
                            catch
                            {
                                string str = value;
                                forName = (from c in db.ForName where c.forname2.Equals(str) select c).FirstOrDefault();
                            }
                            additionally0.id_forname = forName.id;
                            additionally0.ForName = (ForName)forName;
                            db.SaveChanges();
                            return;
                        }

                        try
                        {
                            if(!product.init_product(header, t.Text.ToString(), ref additionally0))
                            {
                                product.init_product(header, value, ref additionally);
                            }
                        }
                        catch(Exception ee) { MessageBox.Show(ee.Message); }
                        BrandName b = product.BrandName;
                        product.BrandName = null;
                        db.SaveChanges();
                        product.BrandName = b;
                    }
                }
            }
            else
            {
                SetSKU setSKU = new SetSKU(products);
                setSKU.ShowDialog();
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            SetSKU setSKU = new SetSKU(products);
            setSKU.ShowDialog();
            var pr = products[products.Count - 1];
            DataTable.ScrollIntoView(pr);
            DataTable.SelectedItem = pr;
        }

        private void datagridf_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridRow row = (DataGridRow)(DataTable.ItemContainerGenerator.ContainerFromItem(DataTable.SelectedItem));
            DataGridDetailsPresenter presenter = FindVisualChild<DataGridDetailsPresenter>(row);
            DataTemplate template = presenter.ContentTemplate;
            DataGrid dg = (DataGrid)template.FindName("datagridf", presenter);
            ObservableCollection<Fitment> fitments = new ObservableCollection<Fitment>();
            foreach (Fitment c in dg.ItemsSource)
            {
                fitments.Add(c);
            }
            TextBox t = e.EditingElement as TextBox;
            String value = t.Text.ToString();
            String header = e.Column.Header.ToString().ToUpper().Replace(" ", "");
            var id = (e.Row.DataContext as Fitment).id;
            if (id == 0)
            {
                using (var db = new ProductDBEntitie())
                {
                    Fitment f = new Fitment();
                    f.sku = (DataTable.SelectedItem as Product).sku;
                    db.Fitment.Add(f);
                    db.SaveChanges();
                    fitments.Add(f);
                    id = f.id;
                }
            }
            //mAKE
            using (var db = new ProductDBEntitie())
            {
                var fitment = (from c in db.Fitment where c.id == id select c).FirstOrDefault();
                var make = (from c in db.Make where fitment.id_make == c.id select c).FirstOrDefault();
                if(make==null)
                {
                    make = new Make();
                    db.Make.Add(make);
                    db.SaveChanges();
                    fitment.id_make = make.id;
                    db.SaveChanges();
                }
                if (header == "MAKE")
                {
                    make.make1 = value;
                }
                else if (header == "MODEL")
                {
                    make.model = value;
                }
                else if (header == "OURMAKE")
                {
                    make.our_make = value;
                }
                else if (header == "OURMODEL")
                {
                    make.our_model = value;
                }
                else if (header == "BODYTYPENAME")
                {
                    make.body_type_name = value;
                }
                else if (header == "_MFRBODYCODENAME_")
                {
                    make.mf_body_code_name = value;
                }
                else if (header == "OURBODYTYPENAME")
                {
                    make.our_body_type_name = value;
                }
                else
                try
                {
                    fitment.init_fitment(header, t.Text.ToString());
                }
                catch (Exception ee) { MessageBox.Show(ee.Message); }
                db.SaveChanges();
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }
    }
}
