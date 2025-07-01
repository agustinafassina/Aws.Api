using AwsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AwsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IEc2Service _ec2Service;
        private readonly IRdsService _rdsService;

        public AwsController(IEc2Service ec2Service, IRdsService rdsService)
        {
            _ec2Service = ec2Service;
            _rdsService = rdsService;
        }

        [HttpGet("ec2")]
        public async Task<IActionResult> GetInstances()
        {
            var instances = await _ec2Service.GetInstancesAsync();
            var result = instances.Select(i => new
            {
                InstanceId = i.InstanceId,
                State = i.State.Name.Value,
                Type = i.InstanceType.Value,
                PublicIp = i.PublicIpAddress,
                LaunchTime = i.LaunchTime
            });

            return Ok(result);
        }

        [HttpGet("rds")]
        public async Task<IActionResult> GetDbInstances()
        {
            var dbs = await _rdsService.GetDbInstancesAsync();
            var result = dbs.Select(d => new
            {
                d.DBInstanceIdentifier,
                d.DBInstanceClass,
                d.Engine,
                d.DBInstanceStatus,
                d.Endpoint?.Address
            });

            return Ok(result);
        }

        [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("v.1.0.0");
        }
    }
}