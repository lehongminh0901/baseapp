using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using demoasm2.Areas.Identity.Data;

namespace Testdemo.Models
{
    public class Cart
    {
        public string UId { get; set; }
        public string BookIsbn { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public AppUser? User { get; set; }
        public Book? Book { get; set; }
    }

}