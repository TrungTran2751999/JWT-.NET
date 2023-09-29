using app.Models;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

public interface IStudentService{
    Task<List<Student>?> GetAll();
    Task<Student?> GetById(long idSystem);
    Task<bool?> Save(Student student);
    Task<bool?> Update(Student student);
}