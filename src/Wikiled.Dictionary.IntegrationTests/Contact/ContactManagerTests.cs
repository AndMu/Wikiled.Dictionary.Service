using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Sercice.Shared.Contact;
using Wikiled.Service.Shared.Contact;

namespace Wikiled.IntegrationTests.Contact
{
    [TestFixture]
    public class ContactManagerTests
    {
        [Test]
        public Task Construct()
        {
            var instance = new ContactManager();
            var form = new ContactForm
                           {
                               From = "andrius",
                               App = "Test",
                               Subject = "Test Subject",
                               Address = "andrius@wikiled.com",
                               Message = "Hi"
                           };
            return instance.Send(form);
        }
    }
}