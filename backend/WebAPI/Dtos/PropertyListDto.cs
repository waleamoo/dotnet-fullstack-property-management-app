using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.Dtos
{
    public class PropertyListDto
    {
        public int Id { get; set; }
        public int SellRent { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string FurnishingType { get; set; } = string.Empty;
        public int Price { get; set; }
        public int BHK { get; set; }
        public int BuiltArea { get; set; }
        public string City { get; set; } = string.Empty; 
        public string Country { get; set; } = string.Empty;
        public bool ReadyToMove { get; set; }
        public DateTime EstPossessionOn { get; set; }
        public string Photo { get; set; } = string.Empty;
    }
}
