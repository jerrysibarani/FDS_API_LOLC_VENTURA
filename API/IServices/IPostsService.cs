using API.Data.Entities;
using API.Data.Models;

namespace API.IServices
{
    public interface IPostsService
    {

        public Task<List<POSTS>> GetListPost();
        public Task<POSTS?> GetListPostById(int postId);
        public Task<List<POSTS>> GetListPostByType(string postType);
        public Task<List<POSTS>> GetListPostByHeader(string postHeader);
        Task<List<POSTS>> GetListPostByGroup(string postGroup);
        public Task<List<PostValueModels>> GetListPostTypeHeaderNull(string postType);
        public Task<List<PostValueModels>> GetListPostTypeHeader(string postType, string postHeader);
        public Task<List<string>> GetListPostType();
        public Task<List<PostTypeModels>> GetListPostGroupType();
    }
}
