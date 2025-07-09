using AutoMapper;
using AwsApi.Contracts.Responses;
using AwsApi.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IEc2Service _ec2Service;
        private readonly IRdsService _rdsService;
        private readonly ILambdaService _lambdaService;
        private readonly IMapper _mapper;

        public AwsController(IEc2Service ec2Service, IRdsService rdsService, IMapper mapper, ILambdaService lambdaService)
        {
            _ec2Service = ec2Service;
            _rdsService = rdsService;
            _mapper = mapper;
            _lambdaService = lambdaService;
        }

        [Authorize(AuthenticationSchemes = "Auth0")]
        [HttpGet("ec2")]
        public async Task<IActionResult> GetInstances()
        {
            var instances = await _ec2Service.GetInstancesAsync();
            var response = _mapper.Map<IEnumerable<Ec2Response>>(instances);
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Auth0")]
        [HttpGet("rds")]
        public async Task<IActionResult> GetDbInstances()
        {
            var dbs = await _rdsService.GetDbInstancesAsync();
            var response = _mapper.Map<IEnumerable<RdsResponse>>(dbs);

            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Auth0")]
        [HttpGet("lambdas")]
        public async Task<IActionResult> GetLambdaFunctions()
        {
            var functions = await _lambdaService.ListFunctionsAsync();
            return Ok(functions);
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("v.1.0.0");
        }
    }
}