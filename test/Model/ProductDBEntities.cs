using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public partial class ProductDBEntitie : DbContext
    {
        public ProductDBEntitie()
            :base("ProductDBEntitie")
        { }

        public virtual DbSet<Additionally> Additionally { get; set; }
        public virtual DbSet<Additionally0> Additionally0 { get; set; }
        public virtual DbSet<BrandName> BrandName { get; set; }
        public virtual DbSet<ForName> ForName { get; set; }
        public virtual DbSet<Make> Make { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<Fitment> Fitment { get; set; }
    }
}
