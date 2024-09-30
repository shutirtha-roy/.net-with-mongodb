using CoreMongoDBCrud.IRepository;
using CoreMongoDBCrud.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreMongoDBCrud.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository = null;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetEmployees()
        {
            var employees = _employeeRepository.Gets();
            return Json(employees);
        }

        public JsonResult SaveEmployees(Employee employee)
        {
            var emp = _employeeRepository.Save(employee);
            return Json(emp);
        }

        public JsonResult DeleteEmployees(string empId)
        {
            string message = _employeeRepository.Delete(empId);
            return Json(message);
        }
    }
}
