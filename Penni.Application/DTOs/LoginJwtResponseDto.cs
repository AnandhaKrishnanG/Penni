using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penni.Application.DTOs
{
    public class LoginJwtResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
