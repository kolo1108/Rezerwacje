using Bookings.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bookings.DAL
{
    public class MyContext : DbContext
    {
         public MyContext()
            : base("MyDatabase")
        {

        }
        public DbSet<User> Users {get; set;}
        public DbSet<Booking> Bookings {get; set;}

    }
}