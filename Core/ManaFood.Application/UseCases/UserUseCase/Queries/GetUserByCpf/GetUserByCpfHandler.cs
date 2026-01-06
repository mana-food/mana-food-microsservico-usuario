using AutoMapper;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByCpf;
public class GetUserByCpfHandler : IRequestHandler<GetUserByCpfQuery, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetUserByCpfHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByCpfQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetBy(c => c.Cpf == request.Cpf && !c.Deleted, cancellationToken);
        return _mapper.Map<UserDto>(user);
    }
}