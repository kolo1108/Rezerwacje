using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bookings.Models
{
    public class Booking
    {
        [ScaffoldColumn(false)]
        public Nullable<int> BookingId { get; set; }
        public Nullable<int> UserId { get; set; }

        [Display(Name = "Nazwa spotkania")]
        [Required(ErrorMessage = "Musisz wprowadzić nazwę spotkania")]
        public string Name { get; set; }

        [Display(Name = "Opis spotkania")]
        [Required(ErrorMessage = "Musisz wprowadzić opis spotkania")]
        [StringLength(50)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "Wprowadź Datę")]
        public Nullable<System.DateTime> CreateBook { get; set; }

        [Display(Name = "Data od")]
        //[RegularExpression("\\d+", ErrorMessage = "Zły format daty. Prawidłowy format to: 2016-07-05 19:18:00")]
        public Nullable<System.DateTime> DateOf { get; set; }

        [Display(Name = "Data do")]
        public Nullable<System.DateTime> DateTo { get; set; }

        [ScaffoldColumn(false)]
        public string Members { get; set; }

        public virtual User User { get; set; }
    }
}
