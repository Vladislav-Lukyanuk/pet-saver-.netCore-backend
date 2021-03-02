using System;
using System.Threading.Tasks;

namespace animalFinder.Service.Interface
{
    public interface IEmailService
    {
        Task Generate(string email, string mailTemplate, object mailData, string subject, Tuple<byte[], string>[] attachments = null);
    }
}
