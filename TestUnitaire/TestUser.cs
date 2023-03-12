using ApiMobile;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace TestUnitaire
{
    [TestClass]
    public class TestUser
    {
        public static readonly string connectionString = "Data Source=DESKTOP-9HGR9KM\\SQLEXPRESS;Initial Catalog=PayeTonKawa;Integrated Security=True;";
        [TestMethod]
        public async Task AuthenticateUserAsync_goodPassword()
        {
            payetonkawaDBContext.connectionString = connectionString;
            var result = await payetonkawaDBContext.AuthenticateUserAsync("fakrimougni@gmail.com", "Payetonkawa");
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task AuthenticateUserAsync_wrongPassword()
        {
            payetonkawaDBContext.connectionString = connectionString;
            var result = await payetonkawaDBContext.AuthenticateUserAsync("fakrimougni@gmail.com", "Payetonkawax");
            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void SelectProductById_goodId()
        {
            payetonkawaDBContext.connectionString = connectionString;
            var result =  payetonkawaDBContext.SelectProductById(1);
            // Assert
           Assert.AreNotEqual("no produit found",result);
        }
      
        [TestMethod]
        public void SelectAllProduct()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreNotEqual("no product found",payetonkawaDBContext.SelectAllProduct());
        }
        [TestMethod]
        public async Task VerifyUserExist_WrongMail()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreEqual(null, await payetonkawaDBContext.VerifyUserExist("wrongmail@gmail.com"));
        }
        [TestMethod]
        public async Task VerifyUserExist_GoodMail()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreNotEqual(null, await payetonkawaDBContext.VerifyUserExist("fakrimougni@gmail.com"));
        }
        [TestMethod]
        public void TestGenerateRandomString()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreEqual(10,  payetonkawaDBContext.GenerateRandomString(10).Length);
        }
        [TestMethod]
        public async Task TestVerifyTokenExistAsync()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreEqual(true, await payetonkawaDBContext.VerifyToken("6SV5iEG1ZZ"));
        }
        [TestMethod]
        public async Task TestVerifyTokenNoExistAsync()
        {
            payetonkawaDBContext.connectionString = connectionString;
            Assert.AreEqual(false, await payetonkawaDBContext.VerifyToken("ZZZZZZZZZZZ"));
        }

        [TestMethod]
        public async Task TestUpdatetoken()
        {
            payetonkawaDBContext.connectionString = connectionString;
            User update = new();
            update.mail = "user1@mail.com";
            update.id = 2;
            update.password = "password1";
            update.token_Auth_API = "token1";
            var rows = await payetonkawaDBContext.UpdateUser(update);
            Assert.AreEqual(true, rows);
        }
    }
}