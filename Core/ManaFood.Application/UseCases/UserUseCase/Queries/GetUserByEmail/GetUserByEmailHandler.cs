using AutoMapper;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByEmail;
public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetUserByEmailHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetBy(c => c.Email == request.Email && !c.Deleted, cancellationToken);
        return _mapper.Map<UserDto>(user);
    }
}