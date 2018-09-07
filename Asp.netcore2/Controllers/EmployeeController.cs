using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.netcore2.Models;

namespace Asp.netcore2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        UserDataAccessLayer _dbcontext = new UserDataAccessLayer();

        [HttpGet]
        public IEnumerable<Employee> GetEmployeeDetail()
        {
            List<Employee> emplist = new List<Employee>();
            emplist = _dbcontext.GetEmployeeDetails().ToList(); ;
            return emplist;
        }
    }
}