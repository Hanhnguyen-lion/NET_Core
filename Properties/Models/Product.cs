using System.ComponentModel.DataAnnotations;
using MyApi.Helper;

namespace MyApi.Models{
    public class Product: IValidatableObject {
        public int ProductId {get;set;}

        [Required]
        public required string ProductName {get;set;}
        public string? Description {get;set;}

        [Required]
        [Range (1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Quantity {get;set;}
        public decimal Price {get;set;}
        public decimal Total {get;set;}

        [Required]
        public DateTime? OrderDate{get;set;}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext){
            if (this.OrderDate > DateTime.Now){
                yield return new ValidationResult($"Please enter Order Date smaller than {DateTime.Now.ToString("MM/dd/yyyy")}");
            }
        }
    }
}
