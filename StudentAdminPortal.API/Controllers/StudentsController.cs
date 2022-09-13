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
        private readonly IImageRepository imageRepository;

        public StudentsController(IStudentRepository studentRepository, IMapper mapper, IImageRepository imageRepository)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
        }
        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            var students = await studentRepository.GetStudentsAsync();



            return Ok(mapper.Map<List<Student>>(students));
        }

        [HttpGet]
        [Route("[controller]/{studentId:guid}"), ActionName("GetStudentByIdAsync")]
        public async Task<IActionResult> GetStudentByIdAsync([FromRoute] Guid studentId)
        {
            var students = await studentRepository.GetStudentByIdAsync(studentId);

            if (students is null)
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

                if (updateStudent is not null)
                {
                    return Ok(mapper.Map<Student>(updateStudent));
                }
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if (await studentRepository.Exists(studentId))
            {
                return Ok(mapper.Map<Student>(await studentRepository.DeleteStudentAsync(studentId)));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[controller]/Add")]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequest request)
        {
          var student = await studentRepository.AddStudentAsync(mapper.Map<DataModels.Student>(request));

            return CreatedAtAction(nameof(GetStudentByIdAsync), new {studentId = student.Id}, mapper.Map<Student>(student));
        }

        [HttpPost]
        [Route("[controller]/{studentId:guid}/upload-image")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid studentId, IFormFile profileImage)
        {
            if(await studentRepository.Exists(studentId))
            {
                var filename = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);

                var fileimagepath = await imageRepository.Upload(profileImage, filename);

                if (await studentRepository.UpdateProfileImage(studentId, fileimagepath))
                {
                    return Ok(fileimagepath);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
            }

            return NotFound();
        }
    }
}
