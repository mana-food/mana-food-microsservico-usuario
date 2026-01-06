using AutoMapper;
using ManaFood.Application.Dtos;
using ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.UpdateUser;
using ManaFood.Domain.Entities;

namespace ManaFood.Application.Mappings;

public sealed class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<CreateUserCommand, User>();
        CreateMap<UpdateUserCommand, User>();
        CreateMap<DeleteUserCommand, User>();
        CreateMap<User, UserDto>();
    }
}
