using Dapper;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Student;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace KshatriyaSportsFoundations.API.Repositories.Repository
{
    public class StudentService: IStudentService
    {
        private IDbConnection dbConnection;
        public StudentService(IConfiguration configuration)
        {
            dbConnection = new SqlConnection(configuration.GetConnectionString("KshatriyaSportsFoundationsDbConnectionString"));
        }

        public async Task<List<EnquiryDomain>> GetStudentDetails(int pageNo = 1, int pageSize = 1000)
        {
            string sql = "select * from Enquiries;";
            var studentDetails = await dbConnection.QueryAsync<EnquiryDomain>(sql);

            //pagination
            var walksSkips = (pageNo - 1) * pageSize;

            return studentDetails.Skip(walksSkips).Take(pageSize).ToList();
        }

        public async Task<bool> SaveStudentDetails(List<EnquiryDomain> studentDetails)
        {
            string sql = "UPDATE Enquiries SET AdminComments=@AdminComments, Fullfilled=@Fullfilled WHERE EnquiryId=@EnquiryId";

            var rowsAffected = await dbConnection.ExecuteAsync(sql, studentDetails);

            return rowsAffected > 0;
        }

        public async Task<int> GetTotalRecordsCount()
        {
            string sql = "select count(*) from Enquiries;";
            var result =await dbConnection.QueryAsync<int>(sql);
            return result.Single();
        }
    }
}
