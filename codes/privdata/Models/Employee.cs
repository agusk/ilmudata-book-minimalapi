using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace privdata.Models;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = ""; // To be encrypted
    public string Phone { get; set; } = "";// To be encrypted
    public string Birthdate { get; set; } = "";// To be encrypted
}
