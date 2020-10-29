namespace Sales.Common.Models
{
using System;
using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {

        [Key]
        public int Productid { get; set; }
        [Required]
        [StringLength (50)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks {get; set;}

        [Display(Name ="Image")]
        public string ImagePath { get; set; }

        [DisplayFormat(DataFormatString ="{0:c2}", ApplyFormatInEditMode =false)]
        public Decimal Price { get; set; }
        [Display(Name = "Is Available")]
        public int IsAvailable { get; set; }
        [Display(Name = "Publish On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }

        [NotMapped]

        public byte[] ImageArray { get; set; }

        public string ImageFullPath 
        {
            get
            {
                if ( string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }
                return $"http://jakuantayper-001-site9.atempurl.com/{this.ImagePath.Substring(1)}";
            }
            
        }
        public override string ToString()
        {
            return this.Description;
        }

    }
}
