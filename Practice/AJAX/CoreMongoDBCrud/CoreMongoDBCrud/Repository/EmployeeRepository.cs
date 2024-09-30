using CoreMongoDBCrud.IRepository;
using CoreMongoDBCrud.Models;
using MongoDB.Driver;

namespace CoreMongoDBCrud.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private MongoClient mongoClient = null;
        private IMongoDatabase database = null;
        private IMongoCollection<Employee> employeeTable = null;

        public EmployeeRepository()
        {
            mongoClient = new MongoClient("mongodb://localhost:27017");
            database = mongoClient.GetDatabase("OfficeDB");
            employeeTable = database.GetCollection<Employee>("Employees");
        }

        public string Delete(string employeeId)
        {
            employeeTable.DeleteOne(x => x.Id == employeeId);

            return "Deleted";
        }

        public Employee Get(string employeeId)
        {
            return employeeTable.Find(x => x.Id == employeeId).FirstOrDefault();
        }

        public List<Employee> Gets()
        {
            return employeeTable.Find(FilterDefinition<Employee>.Empty).ToList();
        }

        public Employee Save(Employee employee)
        {
            var empObj = employeeTable.Find(x => x.Id == employee.Id).FirstOrDefault();

            if (empObj == null)
            {
                employeeTable.InsertOne(employee);
            }
            else
            {
                employeeTable.ReplaceOne(x => x.Id == employee.Id, employee);
            }

            return employee;
        }
    }
}
