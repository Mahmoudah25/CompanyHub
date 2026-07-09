using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth
{
    public interface IAuthJob
    {
        Task ToeknClaenJob();
        Task EmailVerficationCleanJob();
    }
}
