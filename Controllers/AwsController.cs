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
        private readonly ILogger<AwsController> _logger;

        public AwsController(IEc2Service ec2Service, IRdsService rdsService, IMapper mapper, ILambdaService lambdaService, ICostsService costsService, ILogger<AwsController> logger)
        {
            _ec2Service = ec2Service;
            _rdsService = rdsService;
            _mapper = mapper;
            _lambdaService = lambdaService;
            _costsService = costsService;
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = "Auth0")]
        [HttpGet("ec2")]
        public async Task<IActionResult> GetInstances()
        {
            try
            {
                _logger.LogInformation("Getting EC2 instances");
                List<Amazon.EC2.Model.Instance> instances = await _ec2Service.GetInstancesAsync();
                IEnumerable<Ec2Response> response = _mapper.Map<IEnumerable<Ec2Response>>(instances);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EC2 instances. Error: {ErrorMessage}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving EC2 instances.", error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = "Auth0")]
        [HttpGet("rds")]
        public async Task<IActionResult> GetDbInstances()
        {
            try
            {
                _logger.LogInformation("Getting RDS instances");
                List<Amazon.RDS.Model.DBInstance> dbs = await _rdsService.GetDbInstancesAsync();
                IEnumerable<RdsResponse> response = _mapper.Map<IEnumerable<RdsResponse>>(dbs);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RDS instances. Error: {ErrorMessage}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving RDS instances.", error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = "Auth0App2")]
        [HttpGet("lambdas")]
        public async Task<IActionResult> GetLambdaFunctions()
        {
            try
            {
                _logger.LogInformation("Getting Lambda functions");
                List<string> functions = await _lambdaService.ListFunctionsAsync();
                return Ok(functions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Lambda functions. Error: {ErrorMessage}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving Lambda functions.", error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = "Auth0App2")]
        [HttpGet("costs-project")]
        public async Task<IActionResult> GetCostByProject([FromQuery] string projectTag)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectTag))
                {
                    _logger.LogWarning("GetCostByProject called without projectTag parameter");
                    return BadRequest(new { message = "The parameter 'projectTag' is required." });
                }

                _logger.LogInformation("Getting costs for project: {ProjectTag}", projectTag);
                GetCostAndUsageResponse costs = await _costsService.GetCostsByTagAsync(projectTag);
                return Ok(costs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting costs for project {ProjectTag}. Error: {ErrorMessage}", projectTag, ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving project costs.", error = ex.Message });
            }
        }

        [HttpGet("costs")]
        public async Task<IActionResult> GetCostAllProject()
        {
            try
            {
                _logger.LogInformation("Getting all costs");
                GetCostAndUsageResponse costs = await _costsService.GetAllCostsAsync();
                return Ok(costs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all costs. Error: {ErrorMessage}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving all costs.", error = ex.Message });
            }
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok("v.1.0.0");
        }

        [Authorize(AuthenticationSchemes = "Auth0App2")]
        [HttpGet("costs-report")]
        public async Task<IActionResult> GetReportByTag([FromQuery] string projectTag)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectTag))
                {
                    _logger.LogWarning("GetReportByTag called without projectTag parameter");
                    return BadRequest(new { message = "The parameter 'projectTag' is required." });
                }

                _logger.LogInformation("Generating cost report for project: {ProjectTag}", projectTag);
                GetCostAndUsageResponse costs = await _costsService.GetCostsReport(projectTag);
                return Ok(costs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cost report for project {ProjectTag}. Error: {ErrorMessage}", projectTag, ex.Message);
                return StatusCode(500, new { message = "An error occurred while generating the cost report.", error = ex.Message });
            }
        }
    }
}