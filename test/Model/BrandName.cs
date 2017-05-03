using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class BrandName
    {
        public BrandName()
        {
            this.Fitment = new HashSet<Fitment>();
            this.Product = new HashSet<Product>();
        }
        [Key]
        public int id { get; set; }
        [MaxLength(450)]
        [Index("IX_BrandName", 1, IsUnique = true)]
        public string name { get; set; }
        
        public virtual ICollection<Fitment> Fitment { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
