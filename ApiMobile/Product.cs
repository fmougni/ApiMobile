using Microsoft.OData.Edm;
using System.Data.SqlClient;

namespace ApiMobile
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public int rate { get; set; }
        public decimal Weight { get; set; }
        public int tax { get; set; }
        public Description Description { get;  set; }
        public Rate Rate { get;  set; }
        public Tax Tax { get;  set; }
        public List<Media> MediaList { get; set; }

    }
}
        




  

