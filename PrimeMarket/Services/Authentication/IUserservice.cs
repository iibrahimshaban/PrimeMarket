using SurveyBasket.Contracts.Users;

namespace PrimeMarket.Services.Authentication
{
    public interface IUserservice
    {
        public Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken);
        public Task<Result<UserResponse>> GetAsync(string id);
        Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> Unlock(string id);
        Task<Result> ToggleStatus(string id);
    }
}
