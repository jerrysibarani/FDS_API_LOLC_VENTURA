using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class ResponseTotal
    {
        [Column("Total")]
        public int Total { get; set; }
    }
}
