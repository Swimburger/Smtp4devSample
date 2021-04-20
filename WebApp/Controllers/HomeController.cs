using System.Diagnostics;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Smpt4devSample.WebApp.Models;

namespace Smpt4devSample.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(ContactFormViewModel contactFormViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            using var smtpClient = new SmtpClient()
            {
                Host = "localhost",
                Port = 25
            };
            var submitterFullName = $"{contactFormViewModel.FirstName} {contactFormViewModel.LastName}";
            
            // send email to website owner
            smtpClient.Send("website@localhost", "niels@localhost", $"Contact Us submission by {submitterFullName}", 
$@"New Contact Us submission:

    First Name: {contactFormViewModel.FirstName}
    Last Name: {contactFormViewModel.LastName}
    Email Address: {contactFormViewModel.EmailAddress}
    Question:
    {contactFormViewModel.Question}"
            );

            // send email to submitter
            smtpClient.Send("niels@localhost", contactFormViewModel.EmailAddress, "Thank you for contacting us", 
                $"Thank you {submitterFullName} for reaching out to us.\nWe have received your inquiry and will respond to you in 24 hours."
            );

            return RedirectToAction(nameof(ThankYou));
        }

        public IActionResult ThankYou() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
