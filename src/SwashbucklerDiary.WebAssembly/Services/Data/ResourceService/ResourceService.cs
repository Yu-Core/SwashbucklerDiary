using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class ResourceService : Rcl.Services.ResourceService
    {
        public ResourceService(IResourceRepository resourceRepository) : base(resourceRepository)
        {
        }

        protected override void DeleteResourceFiles(List<ResourceModel> resources)
        {
            foreach (var resource in resources)
            {
                var path = resource.ResourceUri!;
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
