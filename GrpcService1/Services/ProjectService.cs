using Grpc.Core;
using GrpcService1;
namespace GrpcService1.Services
{
    public class ProjectService:Project.ProjectBase
    {
        public ProjectService() 
        {
            
        }
        public override Task<ProjectReply> Add(ProjectRequest request, ServerCallContext context)
        {
            return base.Add(request, context);
        }
    }
}
