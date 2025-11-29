using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(string userEmail, string confirmationUrl);
        Task SendPasswordResetEmail(string userEmail, string recoveryUrl);

    }
}
