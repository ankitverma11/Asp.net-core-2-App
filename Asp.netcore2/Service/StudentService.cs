using Asp.netcore2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.netcore2.Service
{
    public class StudentService : IStudentDetailService
    {
        public IEnumerable<StudentDetails> GetAllStudentDetails()
        {
            try
            {
                return new List<StudentDetails>
                {
                    new StudentDetails { studentName="Ankit" , Subject1="Math" , Subject2="Science" , Grade="A+"},
                      new StudentDetails { studentName="Ankur" , Subject1="English" , Subject2="CS" , Grade="A"},
                        new StudentDetails { studentName="Amit" , Subject1="History" , Subject2="Hindi" , Grade="B+"},
                          new StudentDetails { studentName="Alam" , Subject1="Urdu" , Subject2="Home Science" , Grade="B"},
                            new StudentDetails { studentName="Deepak" , Subject1="Math" , Subject2="IT" , Grade="C"}
                };
            }
            catch (Exception ex)
            {
              throw ex;
            }

        }
    }
}
