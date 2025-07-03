using Amazon.EC2.Model;
using Amazon.RDS.Model;
using AutoMapper;
using AwsApi.Contracts.Responses;

namespace AwsApi.Mappers
{
    public class ContractMapping : Profile
    {
        public ContractMapping()
        {
            CreateMap<Instance, Ec2Response>();
            CreateMap<DBInstance, RdsResponse>();
        }
    }
}
