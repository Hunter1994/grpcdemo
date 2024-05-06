using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcService1.Services
{
    public class CompanyService:Company.CompanyBase
    {
        public CompanyService()
        { 
        }
        public override async Task<CompanyReply> Get(CompanyRequest request, ServerCallContext context)
        {
            return new CompanyReply()
            {
                Data = Value.ForStruct(new Struct
                {
                    Fields =
                            {
                                ["enabled"] = Value.ForBool(true),
                                ["metadata"] = Value.ForList(
                                    Value.ForString("value1"),
                                    Value.ForString("value2"))
                            }
                })
            };

        }

    }
}
