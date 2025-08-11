using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos
{
    public class UpdateBannerDto
    {
        public int Id { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Statistic1Title { get; set; }
        public string Statistic1 { get; set; }
        public string Statistic2Title { get; set; }
        public string Statistic2 { get; set; }
        public string Statistic3Title { get; set; }
        public string Statistic3 { get; set; }
        public string Statistic4Title { get; set; }
        public string Statistic4 { get; set; }
    }
}
