using AutoMapper;
using KshatriyaSportsFoundations.API.EnumsAndConstants.Constants;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos;
using KshatriyaSportsFoundations.API.Models.Dtos.Student;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KshatriyaSportsFoundations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        public StudentController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("StudentDetails")]
        [Authorize(Roles = AuthConstants.User+","+AuthConstants.Admin)]
        public async Task<IActionResult> GetStudentDetails([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 1000)
        {
            GenericAPIResponseDto responseDto = new();

            try
            {
                var studentDetailsDomain = await _studentService.GetStudentDetails(pageNo, pageSize);

                if (studentDetailsDomain.Count > 0)
                {
                    int totalCount = await _studentService.GetTotalRecordsCount();

                    var studentDetailsDtos = _mapper.Map<List<StudentDetailsResponseDto>>(studentDetailsDomain);

                    responseDto.Status = true;
                    responseDto.Data = studentDetailsDtos;
                    responseDto.TotalRecordsCount = totalCount;

                    return Ok(responseDto);
                }
                responseDto.Status = true;
                responseDto.Message = "No records found.";
                return NotFound(responseDto);
            }
            catch (Exception ex)
            {
                responseDto.Status = false;
                responseDto.Message = ex.Message;

                return StatusCode(500, responseDto);
            }
        }

        [HttpPut]
        [Route("SaveStudentDetails")]
        [Authorize(Roles = AuthConstants.Admin)]
        public async Task<IActionResult> SaveStudentDetails([FromBody] List<StudentDetailsRequestDto> requestDtos)
        {
            GenericAPIResponseDto response = new();

            try
            {
                var studentDetailsDomain = _mapper.Map<List<EnquiryDomain>>(requestDtos);

                bool result = await _studentService.SaveStudentDetails(studentDetailsDomain);

                response.Status = result;
                response.Message = "Student details successfully saved";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
