﻿namespace WebAPI.Models
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
