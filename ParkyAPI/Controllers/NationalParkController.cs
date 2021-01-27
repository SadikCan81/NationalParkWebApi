using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _npRepository;
        private readonly IMapper _mapper;

        public NationalParkController(INationalParkRepository npRepository, IMapper mapper)
        {
            _npRepository = npRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDTO>))]
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

                return Ok(objDto);
            }
            else
            {
                return NotFound(new { message = "There is no data" });
            }
        }

        [HttpGet("{id:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult GetNationalPark(int id)
        {
            var nationalPark = _npRepository.GetNationalPark(id);

            if (nationalPark != null)
            {
                return Ok(_mapper.Map<NationalParkDTO>(nationalPark));
            }
            else
            {
                return NotFound(new { message = "There is no data for this id!" });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(new { message = "Data could not be created!" });
            }

            if (_npRepository.NationalParkExists(nationalParkDTO.Name))
            {
                return BadRequest(new { message = "Data is already exists!" });
            }

            var nationalPark = _mapper.Map<NationalPark>(nationalParkDTO);

            var succeed = _npRepository.CreateNationalPark(nationalPark);

            if (succeed)
            {
                return CreatedAtRoute("GetNationalPark", new { version = HttpContext.GetRequestedApiVersion().ToString(), id = nationalPark.Id }, nationalPark);
            }
            else
            {
                return BadRequest(new { message = "Data could not be created!" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public IActionResult UpdateNationalPark(int id, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkDTO.Id != id)
            {
                return BadRequest(new { message = "Data could not be found!" });
            }

            var nationalPark = _mapper.Map<NationalPark>(nationalParkDTO);            

            var succeed = _npRepository.UpdateNationalPark(nationalPark);

            if (succeed)
            {
                return Ok(new { message = "Data is successfully updated!" });
            }
            else
            {
                return BadRequest(new { message = "Data could not be updated!" });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult DeleteNationalPark(int id)
        {
            var nationalPark = _npRepository.GetNationalPark(id);
            if (nationalPark == null)
            {
                return BadRequest(new { message = "Data could not be found!" });
            }

            var succeed = _npRepository.DeleteNationalPark(id);

            if (succeed)
            {
                return Ok(new { message = "Data is successfully deleted!" });
            }
            else
            {
                return BadRequest(new { message = "Data could not be deleted!" });
            }
        }
    }
}