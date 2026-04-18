using API.Data.Entities;
using API.Data.Models;

namespace API.IServices
{
    public interface IPostsService
    {

        public Task<IReadOnlyList<POSTS>> GetListPost(CancellationToken cancellationToken = default);
        public Task<POSTS?> GetListPostById(int PostId, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<POSTS>> GetListPostByType(string PostType, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<POSTS>> GetListPostByHeader(string PostHeader, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<POSTS>> GetListPostByGroup(string PostGroup, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<PostValueModels>> GetListPostTypeHeaderNull(string PostType, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<PostValueModels>> GetListPostTypeHeader(string PostType, string PostHeader, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<string>> GetListPostType(CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<PostTypeModels>> GetListPostGroupType(CancellationToken cancellationToken = default);
    }
}
