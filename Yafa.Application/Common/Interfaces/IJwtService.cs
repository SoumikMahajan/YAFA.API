using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Yafa.Domain.Dtos;

namespace Yafa.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UserClaims userClaims,bool rememberme);
        string GenerateScalarToken(string userId, string role);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
