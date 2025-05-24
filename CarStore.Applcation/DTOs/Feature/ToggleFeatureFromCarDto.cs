using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Applcation.DTOs.Feature
{
    public class ToggleFeatureFromCarDto
    {
        public Guid CarId { get; set; }
        public Guid FeatureId { get; set; }
    }
}
