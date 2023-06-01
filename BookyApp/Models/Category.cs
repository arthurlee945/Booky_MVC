using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookyApp.Models
{
    public class Category
    {
        //Id will primarily registered as key but if not need [Key] -> Key data anotation
        [Key]
        public int Id { get; set; }
        [Required]
		[DisplayName("Category Name")]
		public string Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
