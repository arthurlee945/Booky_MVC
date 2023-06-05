using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Booky_Temp.Model
{
	public class Category
	{
		//Id will primarily registered as key but if not need [Key] -> Key data anotation
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(30, ErrorMessage = "Please keep your name under 30 characters")]
		[DisplayName("Category Name")]
		public string Name { get; set; } = "";
		[DisplayName("Display Order")]
		[Range(1, 100, ErrorMessage = "Display Order Must be between 1-100")]
		public int DisplayOrder { get; set; }
	}
}
