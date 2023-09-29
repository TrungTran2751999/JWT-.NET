using System.ComponentModel.DataAnnotations;

namespace app.Models;

public class Student{
    [Key]
    public long idSystem{get;set;}
    public string? name{get;set;}
    public int age{get;set;}
    public string? phoneNumber{get;set;}
    public string? studentCode{get;set;}
    public string? studentGrade{get;set;}
}
