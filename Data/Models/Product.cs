using Data.Models.AuditAndSoftDeleteModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Product : Base, IAuditable, ISoftDeletableEntity
    {
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public int StockQuantity { get; set; } 
    }
}
