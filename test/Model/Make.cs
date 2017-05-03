using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Model
{
    public  class Make
    {
        public Make()
        {
            this.Fitment = new HashSet<Fitment>();
        }
        [Key]
        public int id { get; set; }
        public string make1 { get; set; }
        public string model { get; set; }
        public string our_make { get; set; }
        public string body_type_name { get; set; }
        public string our_body_type_name { get; set; }
        public string mf_body_code_name { get; set; }
        public string our_model { get; set; }

       public virtual ICollection<Fitment> Fitment { get; set; }
    }
}
