using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bookings.Models
{
    public class User
    {
        [ScaffoldColumn(false)]
        public Nullable<int> UserId { get; set; }

        [Display(Name = "Nick")]
        [Required(ErrorMessage = "Musisz wprowadzić nick")]
        public string Nick { get; set; }

        [Display(Name = "Imię")]
        [Required(ErrorMessage = "Musisz wprowadzić imię")]
        public string Name { get; set; }

        [Display(Name = "Nawisko")]
        [Required(ErrorMessage = "Musisz wprowadzić nazwisko")]
        public string Surname { get; set; }

        [ScaffoldColumn(false)]
        public string Role { get; set; }

        [Display(Name = "Hasło")]
        [Required]
        [StringLength(250, ErrorMessage = "{0} musi zawierać co najmniej następującą liczbę znaków: {2}.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Powtórz hasło")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasło i jego potwierdzenie są niezgodne !!!")]
        public string PasswordRepeat { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Musisz wprowadzić adres e-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public bool Active { get; set; }

        [ScaffoldColumn(false)]
        public bool Block { get; set; }

        [ScaffoldColumn(false)]
        public string ImageLink { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }

    }

}