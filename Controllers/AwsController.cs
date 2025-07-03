using AutoMapper;
using AwsApi.Contracts.Responses;
using AwsApi.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AwsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IEc2Service _ec2Service;
        private readonly IRdsService _rdsService;
        private readonly IMapper _mapper;

        public AwsController(IEc2Service ec2Service, IRdsService rdsService, IMapper mapper)
        {
            _ec2Service = ec2Service;
            _rdsService = rdsService;
            _mapper = mapper;
        }

        [HttpGet("ec2")]
        public async Task<IActionResult> GetInstances()
        {
            var instances = await _ec2Service.GetInstancesAsync();
            var response = _mapper.Map<IEnumerable<Ec2Response>>(instances);
            return Ok(response);
        }

        [HttpGet("rds")]
        public async Task<IActionResult> GetDbInstances()
        {
            var dbs = await _rdsService.GetDbInstancesAsync();
            var response = _mapper.Map<IEnumerable<RdsResponse>>(dbs);

            return Ok(response);
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("v.1.0.0");
        }
    }
}