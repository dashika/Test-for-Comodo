using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Fitment
    {
        [Key]
        public int id { get; set; }
        public string sku { get; set; }
        public int id_make { get; set; }
        public Nullable<short> year { get; set; }
        public string submodel { get; set; }
        public Nullable<short> body_num_doors { get; set; }
        public string universal_sku { get; set; }
        public string required_sku { get; set; }
        public string option_sku { get; set; }
        public string available { get; set; }
        public string available1 { get; set; }
        public string available2 { get; set; }
        public string fitment_note { get; set; }
        public string our_real_model { get; set; }
        public Nullable<bool> processing { get; set; }
        public Nullable<int> id_brand_name { get; set; }
        public String num_door { get; set; }
        public String length { get; set; }
        public string mmy { get; set; }

        public virtual BrandName BrandName { get; set; }
        public virtual Make Make { get; set; }
        public virtual Product Product { get; set; }

        internal void init_fitment(string header, string value)
        {
                if (value != "" & value != null)
                    switch (header.ToUpper().Replace(" ", ""))
                    {
                        case "SKU":
                            {
                                sku = value;
                                break;
                            }
                        case "YEAR":
                            {
                                year = Convert.ToInt16(value);
                                break;
                            }
                        case "SUBMODEL":
                            {
                                submodel = value;
                                break;
                            }
                        case "_BODYNUMDOORS_":
                            {
                                body_num_doors = Convert.ToInt16(value);
                                break;
                            }
                        case "BRAND":
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
                                        brand = (from c in db.BrandName where c.name.Equals(value) select c).First();
                                    }
                                }
                                id_brand_name = brand.id;
                                BrandName = brand;
                                break;
                            }
                        case "AVAILABLE":
                            {
                                available = value;
                                break;
                            }

                        case "AVAILABLE1":
                            {
                                available1 = value;
                                break;
                            }
                        case "AVAILABLE2":
                            {
                                available2 = value;
                                break;
                            }
                        case "FITMENTNOTE":
                            {
                                fitment_note = value;
                                break;
                            }
                        case "OURREALMODEL":
                            {
                                our_real_model = value;
                                break;
                            }
                        case "PROCESSING":
                            {
                                processing = Convert.ToBoolean(value);
                                break;
                            }
                        case "NUMDOOR":
                            {
                                num_door = value;
                                break;
                            }

                        case "LENGTH_B":
                            {
                                length = value;
                                break;
                            }
                        case "MMY":
                            {
                                mmy = value;
                                break;
                            }
                    }

        }
    }
}
