using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Utilities;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Services
{
    public class ResourceService : BaseService<ResourceModel>, IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceService(IResourceRepository resourceRepository)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
        }

        public Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            return _resourceRepository.DeleteUnusedResourcesAsync(expression);
        }

        public List<ResourceModel> GetDiaryResources(string content)
        {
            var resources = new List<ResourceModel>();
            string pattern = @"(?<=\(|"")(appdata:///\S+?)(?=\)|"")"; ;

            MatchCollection matches = Regex.Matches(content, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                resources.Add(new()
                {
                    ResourceType = GetResourceType(match.Value),
                    ResourceUri = match.Value,
                });
            }

            return resources;
        }

        public ResourceType GetResourceType(string uri)
        {
            var mime = StaticContentProvider.GetResponseContentTypeOrDefault(uri);
            var type = mime.Split('/')[0];

            return type switch
            {
                "image" => ResourceType.Image,
                "audio" => ResourceType.Audio,
                "video" => ResourceType.Video,
                _ => ResourceType.Unknown
            };
        }
    }
}
