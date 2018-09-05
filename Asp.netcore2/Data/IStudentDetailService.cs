using System.Collections.Generic;

namespace Asp.netcore2.Data
{
    //we will be implementing this interface in our Service
    public interface IStudentDetailService
    {
        IEnumerable<StudentDetails> GetAllStudentDetails();
    }
}
