using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Resource
    {
        public Resource()
        {
            this.Additionally = new HashSet<Additionally>();
        }
        [Key]
        public int id { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string img3 { get; set; }
        public string img4 { get; set; }
        public string pdf { get; set; }
        public string icon { get; set; }

       public virtual ICollection<Additionally> Additionally { get; set; }
    }
}
