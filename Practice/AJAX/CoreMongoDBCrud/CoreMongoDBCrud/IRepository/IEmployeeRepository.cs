﻿using CoreMongoDBCrud.Models;

namespace CoreMongoDBCrud.IRepository
{
    public interface IEmployeeRepository
    {
        Employee Save(Employee employee);
        Employee Get(string employeeId);
        List<Employee> Gets();
        string Delete(string employeeId);
    }
}
