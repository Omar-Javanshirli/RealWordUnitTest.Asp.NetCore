using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace UdemyRealWordUnitTest.Web.Model
{
    public partial class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public string Color { get; set; } = null!;
    }
}
