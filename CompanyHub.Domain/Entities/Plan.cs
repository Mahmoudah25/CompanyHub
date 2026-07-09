using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Plan :BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } 
        public int MaxUsers { get; set; }
        public int MaxRoles { get; set; }
        public int StrorageLimitMB { get; set; }
        // relation Ships
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
