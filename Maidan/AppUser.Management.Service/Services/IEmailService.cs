using AppUser.Management.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUser.Management.Service.Services
{
    public interface IEmailService
    {
        void SendEmail(MailMessage mailMessage);
    }
}
