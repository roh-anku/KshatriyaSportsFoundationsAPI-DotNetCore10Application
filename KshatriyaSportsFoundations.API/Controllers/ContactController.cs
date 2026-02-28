using AutoMapper;
using KshatriyaSportsFoundations.API.EnumsAndConstants.Constants;
using KshatriyaSportsFoundations.API.Models.Domain;
using KshatriyaSportsFoundations.API.Models.Dtos;
using KshatriyaSportsFoundations.API.Models.Dtos.Contact;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KshatriyaSportsFoundations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;

        public ContactController(IContactService contactService, IMapper mapper)
        {
            _contactService = contactService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("SendEnquiry")]
        public async Task<IActionResult> SendEnquiry([FromBody] SendEnquiryRequestDto sendEnquiryRequest)
        {
            var response = new GenericAPIResponseDto();

            if (ModelState.IsValid)
            {
                EnquiryDomain enquiryDomain = _mapper.Map<EnquiryDomain>(sendEnquiryRequest);

                enquiryDomain.RegistrationDate = DateTime.Now;
                var result = await _contactService.SaveEnquiry(enquiryDomain);

                if (result == null)
                {
                    response.Status = false;
                    response.Message = "Something went wrong, please try again after sometime!";
                    return StatusCode(500, response);
                }
                response.Status = true;
                response.Message = "Thank you, we received your Enquiry! our team will reach out to you shortly.";
                response.Data = null; //lets not overload the response with "result" - change if needed by UI proj.

                return Ok(response);
            }
            return BadRequest();
        }
    }
}
