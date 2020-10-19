using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeakersController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public SpeakersController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<SpeakerModel[]>> Get()
        {
            try
            {
                var results = await _repository.GetAllSpeakersAsync();

                return _mapper.Map<SpeakerModel[]>(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SpeakerModel>> Get(int id)
        {
            try
            {
                var speaker = await _repository.GetSpeakerAsync(id);
                if (speaker == null) return NotFound("Speaker does not exist");

                return _mapper.Map<SpeakerModel>(speaker);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SpeakerModel>> Post(SpeakerModel model)
        {
            try
            {
                var speaker = _mapper.Map<Speaker>(model);
                _repository.Add(speaker);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction("Get", "Speakers", values: new { id = speaker.SpeakerId });

                    return Created(url, _mapper.Map<SpeakerModel>(speaker));
                } else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var speaker = await _repository.GetSpeakerAsync(id);
                if (speaker == null) return NotFound();

                _repository.Delete(speaker);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                } else
                {
                    return BadRequest("Failed to delete the speaker.");
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                throw;
            }
        }
    }
}
