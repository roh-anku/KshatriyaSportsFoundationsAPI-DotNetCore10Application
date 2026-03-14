using Dapper;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos.Student;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Text.Json;

namespace KshatriyaSportsFoundations.API.Repositories.Repository
{
    public class StudentService: IStudentService
    {
        private IDbConnection dbConnection;
        private readonly IDistributedCache _Cache;

        public StudentService(IConfiguration configuration, IDistributedCache cache)
        {
            dbConnection = new SqlConnection(configuration.GetConnectionString("KshatriyaSportsFoundationsDbConnectionString"));
            _Cache = cache;
        }

        public async Task<List<EnquiryDomain>> GetStudentDetails(int pageNo = 1, int pageSize = 1000)
        {
            //this redis implementation is for test purpose only - Azure Redis Cache service is disabled/not configured yet(had been tested with real purchased version)
            string cacheKey = "studentDetails";
            var cachedStudentDetails = await _Cache.GetStringAsync(cacheKey);

            if (cachedStudentDetails != null)
            {
                return JsonSerializer.Deserialize<List<EnquiryDomain>>(cachedStudentDetails) ?? new List<EnquiryDomain>();
            }

            string sql = "select * FROM Enquiries order by Fullfilled asc, RegistrationDate desc;";
            var studentDetailsDomain = await dbConnection.QueryAsync<EnquiryDomain>(sql);

            //pagination
            var walksSkips = (pageNo - 1) * pageSize;

            var studentDetails = studentDetailsDomain.Skip(walksSkips).Take(pageSize).ToList();

            //setting cache for student details
            string json = JsonSerializer.Serialize(studentDetails);
            await _Cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return studentDetails;
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
