using ApiMobile;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUnitaire
{
    [TestClass]
    public class TestUnit
    { 
     public static readonly string connectionString = @"Data Source = DESKTOP - 9HGR9KM\\SQLEXPRESS; Initial Catalog = PayeTonKawa; Integrated Security = True; ";
 
        [TestMethod]
        public void TESTT()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreNotEqual(0, payetonkawaDBContext.connectionString.Length);
        }
    }
}
