using System.Threading.Tasks;
using Wikiled.Sercice.Shared.Contact;

namespace Wikiled.Service.Shared.Contact
{
    public interface IContactManager
    {
        Task Send(ContactForm form);
    }
}