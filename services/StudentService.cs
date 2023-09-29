using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using app.Controllers;
using Microsoft.EntityFrameworkCore;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext dbContext;

    public StudentService(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }

    public async Task<List<Student>?> GetAll()
    {
        var students = dbContext.Students.ToListAsync();
        return await students;
    }

    public async Task<Student?> GetById(long idSystem)
    {
        var students = dbContext.Students.FirstOrDefaultAsync(param=>param.idSystem==idSystem);
        return await students;
    }
    [HttpPost]
    public async Task<bool?> Save(Student student)
    {
        await dbContext.Students.AddAsync(student);
        dbContext.SaveChanges();
        return true;
    }
    [HttpPut]
    public async Task<bool?> Update(Student student)
    {
        var result = await dbContext.Students.FirstOrDefaultAsync(param=>student.idSystem==param.idSystem);
        if(result==null){
            return false;
        }
        result.name = student.name;
        result.phoneNumber = student.phoneNumber;
        result.studentCode = student.studentCode;
        result.studentGrade = student.studentGrade;
        dbContext.SaveChanges();
        return true;
    }
}