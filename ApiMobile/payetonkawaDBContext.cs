using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using BCrypt.Net;
using System.Data.SqlClient;

namespace ApiMobile
{
    public static class payetonkawaDBContext
    {
        public static string connectionString ;
      
        public static string SelectAllProduct()
        {

            List<Product> productList = new();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Ouverture de la connexion à la base de données
                connection.Open();

                // Création d'une commande SQL pour récupérer tous les produits
                SqlCommand command = new SqlCommand(@"SELECT * FROM Product inner join Description on Product.Description = Description.id inner join Rate on rate.id = Product.rate
                                                    Inner join tax on Product.Tax = tax.id", connection);

                // Exécution de la commande SQL et récupération du résultat dans un objet SqlDataReader
                SqlDataReader reader = command.ExecuteReader();

                // Parcours des enregistrements retournés par la commande SQL
                while (reader.Read())
                {
                    // Création d'un objet Product à partir des données de la ligne courante du résultat
                    Product product = new Product()
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        rate = (int)reader["Rate"],
                        Weight = (decimal)reader["Weight"],
                        tax = (int)reader["Tax"],
                        Description = new Description()
                        {
                            ISO = (string)reader["ISO"],
                            Short = (string)reader["Short"],
                            Long = (string)reader["Long"]
                        },
                        Rate = new Rate()
                        {
                            Currency = (string)reader["Currency"],
                            price = (decimal)reader["Price"]
                        },
                        Tax = new Tax()
                        {
                            code = (string)reader["Code"],
                            Percentage = (decimal)reader["Percentage"]
                        },
                        MediaList = new List<Media>()
                    };

                    // Ajout du produit à une liste
                    productList.Add(product);
                }

                // Fermeture du lecteur de résultats
                reader.Close();
            }
            if (productList.Count == 0) return "no products found";
            // Conversion de la liste d'objets en tableau de JSON
            //string[] productJsonArray = productList.Select(p => JsonConvert.SerializeObject(p)).ToArray();
            return JsonConvert.SerializeObject(productList);
        }

        public static string SelectProductById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Product  
                               inner join Description on Product.Description = Description.id inner join Rate on rate.id = Product.rate Inner join tax on Product.Tax = tax.id
                               WHERE Product.id = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Product product = new Product()
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        rate = (int)reader["Rate"],
                        Weight = (decimal)reader["Weight"],
                        tax = (int)reader["Tax"],
                        Description = new Description()
                        {
                            ISO = (string)reader["ISO"],
                            Short = (string)reader["Short"],
                            Long = (string)reader["Long"]
                        },
                        Rate = new Rate()
                        {
                            Currency = (string)reader["Currency"],
                            price = (decimal)reader["Price"]
                        },
                        Tax = new Tax()
                        {
                            code = (string)reader["Code"],
                            Percentage = (decimal)reader["Percentage"]
                        },
                        MediaList = new List<Media>()
                    };
                    return JsonConvert.SerializeObject(product);
                }
            }

            return JsonConvert.SerializeObject("no produit found");
        }
        public static async Task <bool> VerifyToken(string token)
        {
           using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT mail FROM Users WHERE token_Auth_API = @token";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@token", token);

            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            }
            return false;
        }
        public static async Task<User> VerifyUserExist(string mail)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users WHERE mail = @mail";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@mail", mail);

            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    id = (int)reader["id"],
                    mail = (string)reader["mail"],
                    password = (string)reader["password"],
                    token_Auth_API = (string)reader["token_Auth_API"]
                };

                return user;
            }

            return null;
        }
       /* public async Task<User> AddUserAsync(User user)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Users (mail, password, token_Auth_API) OUTPUT INSERTED.id VALUES (@mail, @password, @token_Auth_API)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@mail", user.mail);
            command.Parameters.AddWithValue("@password", user.password);
            command.Parameters.AddWithValue("@token_Auth_API", user.token_Auth_API);

            user.id = (int)await command.ExecuteScalarAsync();

            return user;
        }*/
        public static async Task<User> AddUserAsync(User user)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Hasher le mot de passe avec un sel aléatoire
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password, salt);

            var query = "INSERT INTO Users (mail, password, token_Auth_API) OUTPUT INSERTED.id VALUES (@mail, @password, @token_Auth_API)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@mail", user.mail);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@token_Auth_API", user.token_Auth_API);

            user.id = (int)await command.ExecuteScalarAsync();

            return user;
        }

        public static  async Task<User> AuthenticateUserAsync(string mail, string password)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users WHERE mail = @mail";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@mail", mail);

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                // L'utilisateur n'a pas été trouvé dans la base de données
                return null;
            }

            await reader.ReadAsync();
            var id = reader.GetInt32(reader.GetOrdinal("id"));
            var hashedPassword = reader.GetString(reader.GetOrdinal("password"));
            var tokenAuthApi = reader.GetString(reader.GetOrdinal("token_Auth_API"));

            if (!BCrypt.Net.BCrypt.Verify(password, hashedPassword))
            {
                // Le mot de passe fourni ne correspond pas au mot de passe stocké dans la base de données
                return null;
            }

            // Le mot de passe fourni correspond au mot de passe stocké dans la base de données
            return new User { id = id, mail = mail, password = hashedPassword, token_Auth_API = tokenAuthApi };
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GenerateToken(string email)
        {
            string token = GenerateRandomString(10); // Génère une chaîne aléatoire de 8 caractères
            return token;
        }
        public static async Task<bool> UpdateUser(User user)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            // Hasher le mot de passe avec un sel aléatoire
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password, salt);
            var query = "UPDATE Users SET token_Auth_API = @token_Auth_API,mail = @mail, password = @password WHERE id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@token_Auth_API", user.token_Auth_API);
            command.Parameters.AddWithValue("@mail", user.mail);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@id", user.id);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("Aucun utilisateur trouvé avec cet email.");
            }

            return rowsAffected > 0;
        }
    /*    public async Task<bool> UpdateUserMailAsync(User user)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Users SET mail = @mail WHERE id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@mail", user.mail);
            command.Parameters.AddWithValue("@id", user.id);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
        public async Task<bool> UpdateUserPasswordAsync(User user)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Users SET password = @passwordWHERE id = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@password", user.password);
            command.Parameters.AddWithValue("@id", user.id);
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }*/


    }
}
