using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.DTOs.Car
{
    public class AddBrandDto
    {
        public string BrandName { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
