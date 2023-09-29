using System.ComponentModel.DataAnnotations;

namespace app.Models;

public class User{
    [Key]
    public long idSystem{get;set;}
    public string? username{get;set;}
    public string? password{get;set;}
    public string? role{get;set;}
}
