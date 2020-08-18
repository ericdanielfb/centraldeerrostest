using AutoMapper;
using CentralDeErros.Core.Extensions;
using CentralDeErros.Model.Models;
using CentralDeErros.Services;
using CentralDeErros.Transport.MicrosserviceDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CentralDeErros.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MicrosserviceController : ControllerBase
    {
        private readonly MicrosserviceService _service;
        private readonly IMapper _mapper;
        private readonly TokenGeneratorService _tokenGeneratorService;

        public MicrosserviceController(MicrosserviceService service, IMapper mapper, TokenGeneratorService tokenGeneratorService)
        {
            _service = service;
            _mapper = mapper;
            _tokenGeneratorService = tokenGeneratorService;
        }

        [ClaimsAuthorize("Admin", "Read")]
        [HttpGet]
        public ActionResult<IEnumerable<MicrosserviceDTO>> GetAllMicrosservices()
        {
            return Ok
                 (_mapper.Map<IEnumerable<MicrosserviceDTO>>
                 (_service.List()));
        }

        [HttpGet("{clientId}")]
        public ActionResult<MicrosserviceDTO> GetMicrosserviceById(Guid? microsserviceClientId)
        {
            if (microsserviceClientId is null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Microsservice microsservice = _service.Fetch((Guid)microsserviceClientId);
                if (microsservice is null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(_mapper.Map<MicrosserviceDTO>(microsservice));
                }
            }
        }

        [ClaimsAuthorize("Admin", "Delete")]
        [HttpDelete("{clientId}")]
        public ActionResult DeleteMicrosserviceById(Guid? microsserviceClientId)
        {
            if (microsserviceClientId is null)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Microsservice microsservice = _service.Fetch((Guid)microsserviceClientId);
                if (microsservice is null)
                {
                    return NoContent();
                }
                else
                {
                    _service.Delete(microsservice.ClientId);
                    return Ok();
                }
            }
        }


        [ClaimsAuthorize("Admin", "Create")]
        [HttpPost]
        public ActionResult<MicrosserviceRegisterDTO> SaveMicrosservice([FromBody] MicrosserviceNameOnlyDTO microsserviceName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return Created(
                    nameof(SaveMicrosservice),
                    _mapper.Map<MicrosserviceRegisterDTO>(
                        _service.Register(microsserviceName.Name))
                    );
            }
        }
        [ClaimsAuthorize("Admin", "Update")]
        [HttpPut]
        public ActionResult<MicrosserviceDTO> UpdateMicrosserviceName([FromBody] MicrosserviceDTO microsservice)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Microsservice response = _service.Fetch(microsservice.ClientId);
                if (response is null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(
                        _mapper.Map<MicrosserviceDTO>(
                            _service.Update(_mapper.Map<Microsservice>(microsservice))
                        )
                    );
                }
            }
        }

        [ClaimsAuthorize("Admin", "Update")]
        [HttpPost("RegenerateSecret")]
        public ActionResult<MicrosserviceRegisterDTO> RegenerateClientSecret([FromBody] MicrosserviceClientIdOnlyDTO microsserviceClientId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            { 
                Microsservice response = _service.Fetch(microsserviceClientId.ClientId);
                if (response is null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(_mapper.Map<MicrosserviceDTO>(_service.GenerateClientSecret(response)));
                }
            }
        }
    }
}