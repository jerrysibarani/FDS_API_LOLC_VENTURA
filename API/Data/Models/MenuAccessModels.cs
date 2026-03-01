using API.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class MenuAccessModels
    {
        public ACCESS? MainMenu { get; set; }
        public List<ACCESS>? SubMenus { get; set; }
    }
}
