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
    public class TrailController : Controller
    {
        private readonly ITrailRepository _tRepository;
        private readonly IMapper _mapper;

        public TrailController(ITrailRepository tRepository,IMapper mapper)
        {
            _tRepository = tRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrails()
        {
            var trails = _tRepository.GetTrails();
            
            if(trails != null)
            {
                var trailDTOList = new List<TrailDTO>();

                foreach (var trail in trails)
                {
                    trailDTOList.Add(_mapper.Map<TrailDTO>(trail));
                }

                return Ok(trailDTOList);
            }
            else
            {
                return NotFound(new { message = "There is no data" });
            }
        }

        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int id)
        {
            var trail = _tRepository.GetTrail(id);

            if(trail != null)
            {
                var trailDTO = _mapper.Map<TrailDTO>(trail);

                return Ok(trailDTO);
            }
            else
            {
                return NotFound(new { message = "There is no data" });
            }
        }

        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {
            var trails = _tRepository.GetTrailsInNationalPark(nationalParkId);

            if (trails != null)
            {
                var trailDTOList = new List<TrailDTO>();

                foreach (var trail in trails)
                {
                    trailDTOList.Add(_mapper.Map<TrailDTO>(trail));
                }

                return Ok(trailDTOList);
            }
            else
            {
                return NotFound(new { message = "There is no data" });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTrail([FromBody] TrailUpsertDTO trailDTO)
        {
           if(trailDTO == null)
           {
                return NotFound(new { message = "There is no data" });
           }

            if (_tRepository.TrailExists(trailDTO.Name))
            {
                return BadRequest(new { message = "There is a trail exists with the given name!" });
            }

            var trail = _mapper.Map<Trail>(trailDTO);

            var succeed = _tRepository.CreateTrail(trail);

            if (succeed)
            {
                return CreatedAtRoute("GetTrail", new { id = trail.Id }, trail);
            }
            else
            {
                return BadRequest(new { message = "Data could not be created!" });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTrail(int id, [FromBody] TrailUpsertDTO trailDTO)
        {
            if(trailDTO == null || id != trailDTO.Id)
            {
                return BadRequest(new { message = "Data could not be updated! Id's not matched or request's body is empty!" });
            }

            var trail = _mapper.Map<Trail>(trailDTO);

            var succeed = _tRepository.UpdateTrail(trail);

            if (succeed)
            {
                return Ok(new { message = "Data updated successfully!" });
            }
            else
            {
                return BadRequest(new { message = "Data could not be updated!" });
            }
        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_tRepository.TrailExists(trailId))
            {
                return NotFound(new { message = "Data could not be found!" });
            }

            var trailObj = _tRepository.GetTrail(trailId);
            if (!_tRepository.DeleteTrail(trailObj))
            {
                return StatusCode(500, new { message = "Something went wrong when deleting the record!" });
            }

            return Ok(new { message = "Data deleted successfully!" });

        }

    }
}