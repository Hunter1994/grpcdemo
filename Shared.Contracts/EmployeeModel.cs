using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts;
[DataContract]
public class RequestDto
{
    [DataMember(Order = 1)]
    public string Name { get; set; }
}

[DataContract]
public class ResponseDto
{
    [DataMember(Order = 1)]
    public string Message { get; set; }
}

[ServiceContract]
public interface IEmployeeService
{
    [OperationContract]
    Task<ResponseDto> SayHelloAsync(RequestDto request,
        CallContext context = default);
}


