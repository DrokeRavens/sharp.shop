using Core.Extensions;
using Core.Services;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Exceptions;

namespace WebApi.Features.Authentication.Application;

public class CustomerAuthentication
{
    public class Command : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
    }

    public class Result
    {
        public string Token { get; set; }
        public Guid RefreshToken { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ShopDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public Handler(ShopDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var passwordHash = request.Password.Hash();

            var customerDb = await _dbContext.Customers.SingleOrDefaultAsync(customer =>
                                 customer.Email.Equals(request.Email.ToLower()) &&
                                 customer.Password.Equals(passwordHash), cancellationToken)
                             ?? throw new PasswordOrEmailIsInvalidException();

            customerDb.RefreshToken = Guid.NewGuid();
            customerDb.DeviceId = request.DeviceId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Result
            {
                Token = _tokenService.GenerateToken(customerDb),
                RefreshToken = customerDb.RefreshToken
            };
        }
    }
}