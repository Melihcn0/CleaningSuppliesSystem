using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class LocationCity
    {
        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public ICollection<LocationDistrict> Districts { get; set; } = new List<LocationDistrict>();
    }
}
