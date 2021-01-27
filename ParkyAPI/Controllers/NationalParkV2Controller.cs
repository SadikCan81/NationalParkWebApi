using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/NationalPark")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    [ApiController]
    public class NationalParkV2Controller : Controller
    {
        private readonly INationalParkRepository _npRepository;
        private readonly IMapper _mapper;

        public NationalParkV2Controller(INationalParkRepository npRepository, IMapper mapper)
        {
            _npRepository = npRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetNationalParks()
        {
            var nationalParks = _npRepository.GetNationalParks();

            if (nationalParks != null)
            {
                var objDto = new List<NationalParkDTO>();
                foreach (var item in nationalParks)
                {
                    objDto.Add(_mapper.Map<NationalParkDTO>(item));
                }

                return Ok("V2 controller worked!!!");
            }
            else
            {
                return NotFound(new { message = "There is no data" });
            }
        }
    }
}