using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos.Product
{
    public class ChangeProductStatusResponse
    {
        public int Id { get; set; }
        public bool IsArchived { get; set; }
    }
}
