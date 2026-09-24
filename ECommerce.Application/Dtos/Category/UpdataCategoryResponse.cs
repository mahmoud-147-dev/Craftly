using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Category
{
    public class UpdataCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
