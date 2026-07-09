using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Aduit
{
    public interface IAduitJobs
    {
        Task AuditLogCleanJob();
    }
}
