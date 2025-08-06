using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penni.Application.Common.Interfaces
{
    public interface IDatabaseInitializer
    {
        public void CreateDatabase(string defaultConnection);
        public void MigrateDatabase(string defaultConnection);
        public Task SeedAdminUserAsync();
        public Task SeedRolesAsync();
    }
}
