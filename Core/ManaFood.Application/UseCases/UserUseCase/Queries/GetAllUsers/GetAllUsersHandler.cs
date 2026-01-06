using AutoMapper;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetAllUsers;
public class GetAllUsersdHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetAllUsersdHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAll(cancellationToken);
        return _mapper.Map<List<UserDto>>(users);
    }
}