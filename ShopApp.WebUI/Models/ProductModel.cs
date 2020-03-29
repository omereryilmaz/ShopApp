using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 20, ErrorMessage = "Ürün açıklması min 10 karakter max 100 karakter olmalıdır")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Lütfen fiyat bilgisi giriniz.")]
        [Range(1,10000)]
        public decimal? Price { get; set; }

        public List<Category> SelectedCategories { get; set; }
    }
}
