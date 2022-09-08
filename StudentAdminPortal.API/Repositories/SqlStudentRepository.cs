using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DataModels;

namespace StudentAdminPortal.API.Repositories
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly StudentAdminContext context;

        public SqlStudentRepository(StudentAdminContext context)
        {
            this.context = context;
        }

        public async Task<bool> Exists(Guid studentId)
        {
            return await context.Student.AnyAsync(x => x.Id == studentId);
        }

        public async Task<List<Gender>> GetGendersAsync()
        {
            return await context.Gender.ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(Guid studentId)
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address)).FirstOrDefaultAsync(x => x.Id == studentId);
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address)).ToListAsync();
        }

        public async Task<Student> UpdateStudentAsync(Guid studentId, Student request)
        {
            var existStudent = await GetStudentByIdAsync(studentId);

            if(existStudent is not null)
            {
                existStudent.FirstName = request.FirstName;
                existStudent.LastName = request.LastName;
                existStudent.DateOfBirth = request.DateOfBirth;
                existStudent.Email = request.Email;
                existStudent.Mobile = request.Mobile;
                existStudent.GenderId = request.GenderId;
                existStudent.Address.PhysicalAddress = request.Address.PhysicalAddress;
                existStudent.Address.PostalAddress = request.Address.PostalAddress;

                await context.SaveChangesAsync();
                return existStudent;
            }

            return null;
        }
    }
}
