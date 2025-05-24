using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.DTOs.Rate
{
    public class RateResponseDto : AddRateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CarModel { get; set; }
    }
}
