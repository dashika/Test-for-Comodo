using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Additionally
    {
        public Additionally()
        {
            this.Product = new HashSet<Product>();
        }
        [Key]
        public int id { get; set; }
        public Nullable<double> our_price { get; set; }
        public Nullable<short> key_field { get; set; }
        public Nullable<int> position_my { get; set; }
        public string short_teg { get; set; }
        public string detail_teg { get; set; }
        public Nullable<short> metka { get; set; }
        public string key_virt { get; set; }
        public string steps { get; set; }
        public string cf_use { get; set; }
        public Nullable<int> id_resource { get; set; }

        public string series { get; set; }
        public string product { get; set; }
        public string template_my { get; set; }
        public string for_product_name { get; set; }
        public string finish { get; set; }
        public string form { get; set; }
        public string tipo_type { get; set; }
        public string with_mesh { get; set; }
        public string real_lenth_on_car { get; set; }


        public virtual Resource Resource { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
