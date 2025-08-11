using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class ServiceIcon
    {
        public int Id { get; set; }
        public string IconName { get; set; }
        public string IconUrl { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShown { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public ICollection<Service> Services { get; set; } = new List<Service>();

    }
}
