using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.DTOs.Rate
{
    public class AddRateDto
    {
        public Guid UserId { get; set; }
        public Guid CarModelId { get; set; }
        public float RateValue { get; set; }
    }
}
