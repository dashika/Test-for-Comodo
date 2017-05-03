using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Additionally0
    {
        public Additionally0()
        {
            this.Product = new HashSet<Product>();
        }

        public Nullable<bool> valid { get; set; }
        public Nullable<double> core { get; set; }
        public Nullable<int> id_template { get; set; }
        public Nullable<int> id_forname { get; set; }
        public string shipping_type { get; set; }
        public string data_source { get; set; }
        [Key]
        public int id { get; set; }

        public virtual ForName ForName { get; set; }
        public virtual Template Template { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
