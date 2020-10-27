namespace Sales.Common.Models
{
using System;
using System.ComponentModel.DataAnnotations;
   public  class Product
    {

        [Key]
        public int Productid { get; set; }
        [Required]
        public string Description { get; set; }
        public Decimal Price { get; set; }
        public int IsAvailable { get; set; }
        public DateTime PublishOn { get; set; }
        public override string ToString()
        {
            return this.Description;
        }

    }
}
