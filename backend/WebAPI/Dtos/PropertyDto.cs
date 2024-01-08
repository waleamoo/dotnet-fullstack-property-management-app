using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.Dtos
{
    public class PropertyDto
    {
        public int SellRent { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PropertyTypeId { get; set; }
        public int BHK { get; set; }
        public int FurnishingTypeId { get; set; }
        public int Price { get; set; }
        public int BuiltArea { get; set; }
        public int CityId { get; set; }
        public int TotalFloors { get; set; }
        public bool ReadyToMove { get; set; }
        public int CarpetArea { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public int FloorNo { get; set; }
        public string MainEntrance { get; set; } = string.Empty;
        public int Security { get; set; } = 0;
        public bool Gated { get; set; }
        public int Maintenance { get; set; } = 0;
        public DateTime EstPossessionOn { get; set; }
        public int Age { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
    }
}
