using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.LocationDtos
{
    public class ResultLocationCityWithLocationDistrictDto
    {
        public int Id { get; set; } // JSON için Id
        public string CityName { get; set; }
        public List<string> Districts { get; set; }
    }

}
