using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penni.Domain.Entities
{
    public class UserRole
    {
        public string UserId { get; set; }
        public Users User { get; set; }

        public int RoleId { get; set; }
        public Roles Role { get; set; }
    }

}
