using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Product
    {
        public bool init_product(String header, String value)
        {
            if (value != "" & value != null)
                switch (header.ToUpper().Replace(" ", ""))
                {
                    case "SKU":
                        {
                            sku = value;
                            return true;
                        }
                    case "COREDEPOSIT":
                        {
                            core_deposit = value;
                            return true;
                        }
                    case "CFSHIPPING":
                        {
                            cf_shipping = value;
                            return true;
                        }
                    case "SBOX":
                        {
                            sbox = value;
                            return true;
                        }
                    case "AVAILABLE":
                        {
                            available = value;
                            return true;
                        }
                    case "BRANDNAME":
                        {
                            BrandName brand = new BrandName();
                            brand.name = value;
                            using (var db = new ProductDBEntitie())
                            {
                                try
                                {
                                    db.BrandName.Add(brand);
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    brand.id = (from c in db.BrandName where c.name.Equals(value) select c.id).SingleOrDefault();
                                }
                            }
                            id_brand_name = brand.id;
                            BrandName = brand;
                            return true;
                        }
                    case "PACKAGE":
                        {
                            package = value;
                            return true;
                        }

                    case "PRICE":
                        {
                            price = Convert.ToDouble(value.Replace(".", ","));
                            return true;
                        }
                    case "HEIGHT":
                        {
                            height = Convert.ToDouble(value.Replace(".", ","));
                            return true;
                        }
                    case "LENGTH":
                        {
                            length = Convert.ToDouble(value.Replace(".", ","));
                            return true;
                        }
                    case "WIDTH":
                        {
                            width = Convert.ToDouble(value.Replace(".", ","));
                            return true;
                        }
                    case "WEIGHT":
                        {
                            weight = Convert.ToDouble(value.Replace(".", ","));
                            return true;
                        }
                    case "UPC":
                        {
                            upc = Convert.ToInt64(value);
                            return true;
                        }

                    case "INSTOCK":
                        {
                            in_stock = value;
                            return true;
                        }
                    case "MAINPRODUCT":
                        {
                            main_product = value;
                            return true;
                        }
                    case "VENDORID":
                        {
                            vendor_id = Convert.ToInt32(value);
                            return true;
                        }
                }
            return false;
        }

        public bool init_product(String header, String value, ref Additionally0 additionally0)
        {
            if (!init_product(header, value))
                if (value != "" & value != null)
                switch (header)
                {
                    case "SHIPPINGTYPE":
                        {
                            additionally0.shipping_type = value;
                            return true;
                        }
                    case "Templates":
                        {
                            Template temp = new Template();
                            temp.name = value;
                            using (var db = new ProductDBEntitie())
                            {
                                try
                                {
                                   db.Template.Add(temp);
                                    db.SaveChanges();
                                }
                                catch
                                {
                                    temp = (from c in db.Template where c.name.Equals(value) select c).First();
                                }
                            }
                            additionally0.id_template = temp.id;
                                return true;
                        }
                    case "Core":
                        {
                            additionally0.core = Convert.ToDouble(value.Replace(".", ","));
                                return true;
                        }
                    case "Valid":
                        {
                            additionally0.valid = Convert.ToBoolean(value);
                                return true;
                        }
                    case "DATA SOURCE":
                        {
                            additionally0.data_source = value;
                                return true;
                        }
                }
            return false;
        }

        public void init_product(String header, String value, ref Additionally additionally)
        {
            if(!init_product(header, value))
            if (value != "" & value != null)
                switch (header)
                {
                    case "OURPRICE":
                        {
                            additionally.our_price = Convert.ToDouble(value.Replace(".", ","));
                            break;
                        }
                    case "KeyField":
                        {
                            additionally.key_field = Convert.ToInt16(value);
                            break;
                        }
                    case "Positions_MY":
                        {
                            additionally.position_my = Convert.ToInt32(value);
                            break;
                        }
                    case "Short_teg":
                        {
                            additionally.short_teg = value;
                            break;
                        }
                    case "Detail_teg":
                        {
                            additionally.detail_teg = value;
                            break;
                        }
                    case "Metka":
                        {
                            additionally.metka = Convert.ToInt16(value);
                            break;
                        }
                    case "KeyVirt":
                        {
                            additionally.key_virt = value;
                            break;
                        }
                    case "Steps":
                        {
                            additionally.steps = value;
                            break;
                        }
                    case "CF_USE":
                        {
                            additionally.cf_use = value;
                            break;
                        }

                        case "Series":
                            {
                                additionally.series = value;
                                break;
                            }
                        case "Product":
                            {
                                additionally.product = value;
                                break;
                            }
                        case "Template_My":
                            {
                                additionally.template_my = value;
                                break;
                            }
                        case "for_Product Name":
                            {
                                additionally.for_product_name = value;
                                break;
                            }
                        case "Finish":
                            {
                                additionally.finish = value;
                                break;
                            }
                        case "Form":
                            {
                                additionally.form = value;
                                break;
                            }
                        case "Tipo_Type":
                            {
                                additionally.tipo_type = value;
                                break;
                            }
                        case "Real_Length_On_Car":
                            {
                                additionally.real_lenth_on_car = value;
                                break;
                            }
                        case "With_Mesh":
                            {
                                additionally.with_mesh = value;
                                break;
                            }

                    }
        }

        
        [Key]
        public string sku { get; set; }
        public string core_deposit { get; set; }
        public string cf_shipping { get; set; }
        public string sbox { get; set; }
        public string available { get; set; }
        public int id_brand_name { get; set; }
        public string package { get; set; }
        public double price { get; set; }
        public double height { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double weight { get; set; }
        public Nullable<long> upc { get; set; }
        public string in_stock { get; set; }
        public string main_product { get; set; }
        public Nullable<int> vendor_id { get; set; }
        public Nullable<int> id_additionally0 { get; set; }
        public Nullable<int> id_additionally { get; set; }

        public virtual Additionally Additionally { get; set; }
        public virtual Additionally0 Additionally0 { get; set; }
        public virtual BrandName BrandName { get; set; }
        public virtual List<Fitment> Fitment { get; set; }
    }
}
