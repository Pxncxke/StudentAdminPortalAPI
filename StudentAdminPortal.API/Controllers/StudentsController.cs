using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;

        public StudentsController(IStudentRepository studentRepository, IMapper mapper)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            var students = await studentRepository.GetStudentsAsync();

         

            return Ok(mapper.Map<List<Student>>(students));
        }

        [HttpGet]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> GetStudentByIdAsync([FromRoute]Guid studentId)
        {
            var students = await studentRepository.GetStudentByIdAsync(studentId);

            if(students is null)
            {
                return NotFound();
            }
         
            return Ok(mapper.Map<Student>(students));


        }

        [HttpPut]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        {
           
            if (await studentRepository.Exists(studentId))
            {

                var updateStudent = await studentRepository.UpdateStudentAsync(studentId, mapper.Map<DataModels.Student>(request));

                if(updateStudent is not null)
                {
                    return Ok(mapper.Map<Student>(updateStudent));
                }
            }
          
            return NotFound();
        }
    }
}
