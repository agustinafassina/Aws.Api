using Amazon.EC2.Model;
using Amazon.RDS.Model;
using AwsApi.Contracts.Responses;

namespace AwsApi.Mappers
{
    public static class ContractMapping
    {
        public static Ec2Response ToResponse(this Instance instance) => new()
        {
            InstanceId = instance.InstanceId,
            State = instance.State?.Name?.Value
        };

        public static RdsResponse ToResponse(this DBInstance dbInstance) => new()
        {
            DBInstanceIdentifier = dbInstance.DBInstanceIdentifier,
            DBInstanceArn = dbInstance.DBInstanceArn
        };
    }
}
