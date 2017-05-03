using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using test.Model;

namespace test
{
    /// <summary>
    /// Логика взаимодействия для SetSKU.xaml
    /// </summary>
    public partial class SetSKU : Window
    {
        private ObservableCollection<Product> products;

        SetSKU()
        {
            InitializeComponent();
        }

        public SetSKU(ObservableCollection<Product> products): this()
        {
            this.products = products;
        }



        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (products != null)
            {
                Product product = new Product();
                product.sku = textBox.Text.ToString();
                using (var db = new ProductDBEntitie())
                {
                    db.Product.Add(product);
                    db.SaveChanges();
                }
                products.Add(product);
            }
              
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
