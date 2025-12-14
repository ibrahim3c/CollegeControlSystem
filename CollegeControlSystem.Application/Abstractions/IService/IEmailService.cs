using Microsoft.AspNetCore.Http;

namespace Bookify.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? files = null);

}