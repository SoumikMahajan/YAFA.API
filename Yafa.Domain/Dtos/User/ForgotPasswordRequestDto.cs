using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos.User
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
