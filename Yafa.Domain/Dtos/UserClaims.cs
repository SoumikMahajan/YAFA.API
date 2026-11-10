using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos
{
    public class UserClaims
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string RolesId { get; set; } = string.Empty;
        public string ProfileImagePath { get; set; } = string.Empty;
        public string ThumbImagePath { get; set; } = string.Empty;
    }
}
