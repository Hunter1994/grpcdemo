using ProtoBuf.Grpc;
using Shared.Contracts;

namespace GrpcService1.Services
{
    public class EmployeeService : IEmployeeService
    {
        public async Task<ResponseDto> SayHelloAsync(RequestDto request, CallContext context = default)
        {
            return new ResponseDto()
            {
                Message = request.Name
            };
        }
    }
}
