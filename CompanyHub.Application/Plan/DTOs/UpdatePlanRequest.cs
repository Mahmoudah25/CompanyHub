using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan.DTOs
{
    public class UpdatePlanRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxUsers { get; set; }
        public int MaxRoles { get; set; }
        public int StrorageLimitMB { get; set; }
    }
}
