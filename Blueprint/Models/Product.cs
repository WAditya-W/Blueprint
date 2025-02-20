﻿using System.Diagnostics.CodeAnalysis;

namespace Blueprint.Models
{
    [ExcludeFromCodeCoverage]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
