using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
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
                var path = MauiBlazorWebViewHandler.UrlRelativePathToFilePath(resource.ResourceUri!);
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
