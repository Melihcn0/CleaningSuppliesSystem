using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.LocationDtos
{
    public class ResultLocationDistrictDto
    {
        public int Id { get; set; }
        public int? CityId { get; set; }
        public string DistrictName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
