using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities
{
    [Table("POSTS", Schema = "public")]
    public class POSTS
    {
        [Key]
        [Required(ErrorMessage = "POST ID is required")]
        public int POST_ID { get; set; }


        [Required(ErrorMessage = "Post Name is required")]
        public string? POST_NAME { get; set; }

        [Required(ErrorMessage = "Post Value is required")]
        public string? POST_VALUE { get; set; }

        [Required(ErrorMessage = "Post Type is required")]
        public string? POST_TYPE { get; set; }

        public string? POST_HEADER { get; set; }

        public string? POST_GROUP { get; set; }
        public bool ISACTIVE { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }

    }
}
