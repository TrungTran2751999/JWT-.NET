using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using Microsoft.AspNetCore.Authorization;

namespace app.Controllers;
// [ApiController]
[Route("student")]
public class StudentController : Controller
{
    [Authorize(Roles = "ADMIN")]
    public ActionResult Index(){
        return View();
    }
    // private ApplicationDbContext db;
    // public StudentController(ApplicationDbContext db){
    //     this.db = db;
    // }

    // [HttpGet]
    // public IActionResult Get(){
    //     var student = this.db.Students.ToList();
    //     return Ok(student);
    // }

    // [HttpPost]
    // public IActionResult Post([FromBody] Student student){
    //     db.Students.Add(student);
    //     db.SaveChanges();
    //     return Ok("Success");
    // }

    // [HttpPut]
    // public IActionResult Put([FromBody] Student student){
    //     var param = db.Students.FirstOrDefault(param=>param.idSystem==student.idSystem);
    //     param.name = student.name;
    //     param.age = student.age;
    //     param.phoneNumber = student.phoneNumber;
    //     param.studentCode = student.studentCode;
    //     param.studentGrade = student.studentGrade;
    //     db.SaveChanges();
    //     return Ok(param);
    // }
}
