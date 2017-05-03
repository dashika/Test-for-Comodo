using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public partial class ForName
    {
        public ForName()
        {
            this.Additionally0 = new HashSet<Additionally0>();
        }
        [Key]
        public int id { get; set; }
        public string forname1 { get; set; }
        [MaxLength(450)]
        [Index("IX_Forname2", 1, IsUnique = true)]
        public string forname2 { get; set; }

        public virtual ICollection<Additionally0> Additionally0 { get; set; }
    }
}
