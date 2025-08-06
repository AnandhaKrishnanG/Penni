using Penni.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penni.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<LoginJwtResponseDto> LoginAsync(LoginDto loginDto);
    }

}
