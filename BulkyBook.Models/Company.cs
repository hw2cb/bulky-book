using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Название компании")]
        public string Name { get; set; }
        [Display(Name = "Улица")]
        public string StreetAddress { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }
        [Display(Name = "Область")]
        public string Region { get; set; }
        [Display(Name = "Почтовый индекс")]
        public string PostalCode { get; set; }
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Авторизация")]
        public bool IsAuthorizedCompany { get; set; }
    }
}
