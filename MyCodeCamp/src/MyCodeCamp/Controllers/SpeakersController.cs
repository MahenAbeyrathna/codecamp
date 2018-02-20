using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers 
{
    [Route("api/camps/{moniker}/speakers")]
    [ValidateModel]
    public class SpeakersController : BaseController
    {
        private ILogger _logger;
        private IMapper _mapper;
        private ICampRepository _repo;

        public SpeakersController( ICampRepository repo,
                                   ILogger<SpeakersController> logger,
                                   IMapper mapper )
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet()]
        public  IActionResult Get(string moniker)
        {
            var speakers = _repo.GetSpeakersByMoniker(moniker);

            return Ok(_mapper.Map<IEnumerable<SpeakerModel>> (speakers));
        }
        [HttpGet("{id}",Name = "SpeakerGet")]
        public IActionResult Get(string moniker,int id)
        {
            var speaker = _repo.GetSpeaker(id);
            if(speaker == null)
            {
                return NotFound();
            }
            if(speaker.Camp.Moniker != moniker)
            {
                return BadRequest("Speaker not in specified camp");
            }
            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }
        [HttpPost]
        public async  Task<IActionResult> Post(string moniker,[FromBody]SpeakerModel model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                var camp = _repo.GetCampByMoniker(moniker);
                if(camp == null)
                {
                    return BadRequest("could not find camp");
                }

                var speaker = _mapper.Map<Speaker>(model);

                _repo.Add(speaker);
                if(await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("SpeakerGet",new { moniker = camp.Moniker, id= speaker.Id });
                    return Created(newUri,_mapper.Map<SpeakerModel>(speaker));
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while adding speaker : {ex}");
            }
            return BadRequest("Could not add a new Speaker");
        }
        [HttpPut("{id}")]
        public async  Task<IActionResult> Put(string moniker,int id,[FromBody]CampModel model)
        {
            try
            {
                var speaker = _repo.GetSpeaker(id);
                if(speaker == null)
                {
                    return NotFound();
                }
                if(speaker.Camp.Moniker != moniker)
                {
                    return BadRequest($"Speaker and Camp not matched");
                }
                _mapper.Map(model, speaker);
                if(await _repo.SaveAllAsync())
                {
                    return Ok(_mapper.Map<CampModel>(speaker));

                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Exception thrown while updating Speaker: {ex}");
            }
            return BadRequest("Couldn't update the Speaker");
        }
    }
}
