using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.CostExplorer.Model;
using AutoMapper;
using AwsApi.Contracts.Responses;
using AwsApi.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AwsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IEc2Service _ec2Service;
        private readonly IRdsService _rdsService;
        private readonly ILambdaService _lambdaService;
        private readonly ICostsService _costsService;
        private readonly IMapper _mapper;

        public AwsController(IEc2Service ec2Service, IRdsService rdsService, IMapper mapper, ILambdaService lambdaService, ICostsService costsService)
        {
            _ec2Service = ec2Service;
            _rdsService = rdsService;
            _mapper = mapper;
            _lambdaService = lambdaService;
            _costsService = costsService;
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

        [Authorize(AuthenticationSchemes = "Auth0_App_2")]
        [HttpGet("lambdas")]
        public async Task<IActionResult> GetLambdaFunctions()
        {
            var functions = await _lambdaService.ListFunctionsAsync();
            return Ok(functions);
        }

        [HttpGet("costs")]
        public async Task<IActionResult> GetCostByProject([FromQuery] string projectTag)
        {
            if (string.IsNullOrEmpty(projectTag))
                return BadRequest("The parameter 'projectTag' is required.");

            GetCostAndUsageResponse costs = await _costsService.GetCostsByTagAsync(projectTag);
            return Ok(costs);
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("v.1.0.0");
        }
    }
}