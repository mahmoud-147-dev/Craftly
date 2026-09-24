using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } 
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreateAT { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;
        public int ReservedQuantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
        public ICollection<InventoryItem> InventoryItems { get; set; }=new List<InventoryItem>();
        public ICollection<OrderItem> OrderItems { get; set; }=new List<OrderItem>();
    }
}
