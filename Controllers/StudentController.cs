using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;


namespace app.Controllers;
// [ApiController]
[Route("student")]
public class StudentController : Controller
{
    private ApplicationDbContext dbContext;
    public StudentController(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }
    [Authorize(Roles = "ADMIN,USER")]
    public async Task<ActionResult> Index(){
        var student = dbContext.Students.ToList();
        ViewBag.result = student;
        return View();
    }
}
