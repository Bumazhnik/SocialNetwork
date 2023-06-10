using System.ComponentModel.DataAnnotations; 

namespace SocialNetwork.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
