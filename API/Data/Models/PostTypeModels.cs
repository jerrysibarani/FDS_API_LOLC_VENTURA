using API.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class PostTypeModels
    {
        public string? GroupPostType { get; set; }
        public List<POSTS>? PostHelps { get; set; }

    }

}
