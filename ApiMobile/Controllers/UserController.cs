using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
//using SharpDX.Direct2D1;
using Svg;
using System.Drawing;
//using SkiaSharp;
using SkiaSharp.Views;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ZXing;
using ZXing.Common;
using QRCoder;

namespace ApiMobile.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
            payetonkawaDBContext.connectionString = _config.GetConnectionString("DefaultConnection");

        }

        [HttpPost("authentificate")]
        public async Task<ActionResult<string>> authentificate([FromBody] UserLoginModel model)
        {
            //payetonkawaDBContext.connectionString = _config.GetConnectionString("DefaultConnection"); ;

            var user = await payetonkawaDBContext.AuthenticateUserAsync(model.Mail, model.Password);
            if (user == null)
            {
                // L'utilisateur n'a pas été trouvé dans la base de données ou le mot de passe fourni est incorrect
                return BadRequest("Invalid email or password");
            }

            // Le mot de passe fourni correspond au mot de passe stocké dans la base de données
            return Ok(user.token_Auth_API);
        }
        [HttpPost("update")]
        public async Task<ActionResult<string>> Update([FromBody] User modif_user)
        {

            var rowsAffected = await payetonkawaDBContext.UpdateUser(modif_user);
            if (rowsAffected == null)
            {
                // L'utilisateur n'a pas été trouvé dans la base de données ou le mot de passe fourni est incorrect
                return BadRequest("Update fail");
            }

            // Le mot de passe fourni correspond au mot de passe stocké dans la base de données
            return Ok();
        }

        //Route inscription
        [HttpPost(Name = "Inscription")]
         public async Task<IActionResult> PostInscriptionAsync([FromBody] User new_user)
          {
             if (new_user == null)
              {
                  return BadRequest("user object is null");
              }
              //Récupère l'email de l'user et vérifie si il n'existe pas déjà en base de données
              //Si l'email existe return erreur email déja existant
              var existingUser = await payetonkawaDBContext.VerifyUserExist(new_user.mail);
              if (existingUser != null)
              {
                  return BadRequest("user with this email already exists");
              }
              //Si l'email existe return erreur email déja existant

              //si l'émail n'existe créer un token_Auth_API et l'envoyer au mail sous la forme d'un QR_Code
              // Générer le token d'authentification
              string token = payetonkawaDBContext.GenerateToken(new_user.mail);
              new_user.token_Auth_API = token; 
              // Générer le QR Code
              string widthText = "200";
              string heightText = "200";
              var url = string.Format("https://chart.googleapis.com/chart?cht=qr&chs={1}x{2}&chl={0}", token,widthText,heightText);
              WebResponse response = default(WebResponse);
              Stream stream = default(Stream);
              StreamReader streamReader = default(StreamReader);
              WebRequest request = WebRequest.Create(url);
              response = request.GetResponse();
              stream = response.GetResponseStream();
              streamReader = new StreamReader(stream);
              System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
              var path = Path.Combine(Directory.GetCurrentDirectory(), "QRCode");
              image.Save(path +@"\\"+token+".jpeg");
              response.Close();
              stream.Close();
              streamReader.Close();
              // Envoyer le QR Code par mail
              //string to = new_user.mail;
              string to = new_user.mail;  
              string subject = "Token d'authentification";
             // string body = "Voici votre token d'authentification :";
              string body = @"<p>Bien vue sur L'application mobile PayeTonKawa :Voici votre token d'authentification :</p><br/><img src='data:image/png;base64," + Convert.ToBase64String(ImageToByte(image)) + "'/>";
              SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);    
              smtp.EnableSsl = true;
              smtp.UseDefaultCredentials = false;
              smtp.Credentials = new NetworkCredential("payetonkawa667@gmail.com", "zgvjvojqxzzuiohf");
              // Chemin complet vers le fichier image
              string imagePath = Path.Combine(path, token + ".Jpeg");
              // Créer l'objet pièce jointe
              Attachment attachment = new Attachment(imagePath, MediaTypeNames.Image.Jpeg);
              try
                  {
                      MailMessage message = new MailMessage();
                          message.From = new MailAddress("payetonkawa667@gmail.com");
                          message.To.Add(to);
                          message.Subject = subject;
                          message.Body = body;
                          message.IsBodyHtml = true;
                          message.BodyEncoding = Encoding.UTF8;
                          message.SubjectEncoding = Encoding.UTF8;
                          message.Attachments.Add(attachment);
                          smtp.Send(message);

                      Console.WriteLine("Le code QR a été envoyé à l'adresse " + to);
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine("Une erreur est survenue lors de l'envoi du mail : " + ex.Message);
                  }

            //Insérer le client en base de donnée
            await payetonkawaDBContext.AddUserAsync(new_user);
              return Ok();
          }
          private byte[] ImageToByte(System.Drawing.Image img)
          {
              using (var stream = new MemoryStream())
              {
                  img.Save(stream, ImageFormat.Png);
                  return stream.ToArray();
              }
          }
    } 
    }
