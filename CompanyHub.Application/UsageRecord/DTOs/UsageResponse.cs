using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.UsageRecord.DTOs
{
    public class UsageResponse
    {
        public int UsersCount { get; set; }
        public int RolesCount { get; set; }
        public int NotificatiomCount { set; get; }
        public int ApiCallsCount { get; set; }
        public decimal StorageUsedGb { get; set; }
    }
}
