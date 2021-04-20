using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;

namespace Smpt4devSample.UiTests
{
    [TestClass]
    public class ContactUsFormTests
    {
        [ClassInitialize]
        public static async Task SetupTests(TestContext testContext)
        {
            var chromeDriverInstaller = new ChromeDriverInstaller();
            await chromeDriverInstaller.Install();
        }

        [TestMethod]
        public void TestContactUsForm()
        {
            // output should look like 9014dbca920e40dfa7c760a4a4808759@localhost
            var emailAddress = $"{Guid.NewGuid().ToString("N")}@localhost";

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            using (var driver = new ChromeDriver(chromeOptions))
            {
                driver.Navigate().GoToUrl("https://localhost:5001");
                driver.FindElementById("firstName").SendKeys("Jon");
                driver.FindElementById("lastName").SendKeys("Doe");

                driver.FindElementById("emailAddress").SendKeys(emailAddress);

                driver.FindElementById("question").SendKeys("Hello World!");

                driver.FindElementsByCssSelector("form button").First().Click();

                Assert.AreEqual("https://localhost:5001/Home/ThankYou", driver.Url);
                Assert.IsTrue(driver.PageSource.Contains("Thank you for contacting us"));
            }

            // more to come

            using (var client = new ImapClient())
            {
                client.Connect("localhost", 143);
                client.Authenticate(userName: "", password: "");

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var emailToSiteOwner = inbox.GetMessage(inbox.Count - 1);
                var emailToSubmitter = inbox.GetMessage(inbox.Count - 2);
                MimeKit.MimeMessage tempMessage = null;

                // swap emails if necessary
                if(
                    !emailToSiteOwner.Subject.Contains("Contact Us submission by") && 
                    emailToSubmitter.Subject.Contains("Contact Us submission by")
                )
                {
                    tempMessage = emailToSiteOwner;
                    emailToSiteOwner = emailToSubmitter;
                    emailToSubmitter = tempMessage;
                }

                Assert.AreEqual("Contact Us submission by Jon Doe", emailToSiteOwner.Subject);
                Assert.IsTrue(emailToSiteOwner.TextBody.Contains("First Name: Jon"));
                Assert.IsTrue(emailToSiteOwner.TextBody.Contains("Last Name: Doe"));
                Assert.IsTrue(emailToSiteOwner.TextBody.Contains($"Email Address: {emailAddress}"));
                Assert.IsTrue(emailToSiteOwner.TextBody.Contains("Hello World!"));

                Assert.AreEqual("Thank you for contacting us", emailToSubmitter.Subject);
                Assert.IsTrue(emailToSubmitter.TextBody.Contains("Thank you Jon Doe for reaching out to us."));

                client.Disconnect(true);
            }
        }
    }
}
