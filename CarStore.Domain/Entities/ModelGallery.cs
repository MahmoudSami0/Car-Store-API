using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarStore.Domain.Entities
{
    public class ModelGallery
    {
        public Guid Id { get; set; }
        public Guid CarModelId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDeleted { get; set; }

        public CarModel CarModel { get; set; }
    }
}
