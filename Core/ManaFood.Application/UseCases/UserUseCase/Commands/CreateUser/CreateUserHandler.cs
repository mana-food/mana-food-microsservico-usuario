using AutoMapper;
using ManaFood.Application.Dtos;
using ManaFood.Domain.Entities;
using ManaFood.Application.Interfaces;
using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly UserValidationService _userValidationService;

    public CreateUserHandler(IUserRepository repository, IMapper mapper, UserValidationService userValidationService)
    {
        _repository = repository;
        _mapper = mapper;
        _userValidationService = userValidationService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {

        var user = _mapper.Map<User>(request);

        await _userValidationService.ValidateUniqueEmailAndCpfAsync(user, cancellationToken);

        await _repository.Create(user, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}
