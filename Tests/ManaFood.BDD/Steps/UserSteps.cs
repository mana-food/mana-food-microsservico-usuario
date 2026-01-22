using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.UpdateUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserById;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByEmail;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByCpf;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetAllUsers;
using ManaFood.Application.Mappings;
using ManaFood.Application.Services;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using AutoMapper;
using Moq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ManaFood.BDD.Steps;

[Binding]
public class UserSteps
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IMapper _mapper;
    private readonly UserValidationService _userValidationService;
    
    private CreateUserHandler _createUserHandler = null!;
    private GetUserByIdHandler _getUserByIdHandler = null!;
    private GetUserByEmailHandler _getUserByEmailHandler = null!;
    private GetUserByCpfHandler _getUserByCpfHandler = null!;
    private GetAllUsersdHandler _getAllUsersHandler = null!;
    private UpdateUserHandler _updateUserHandler = null!;
    private DeleteUserHandler _deleteUserHandler = null!;
    
    private CreateUserCommand? _createUserCommand;
    private UpdateUserCommand? _updateUserCommand;
    private DeleteUserCommand? _deleteUserCommand;
    private Guid _userId;
    private string _userEmail = string.Empty;
    private string _userCpf = string.Empty;
    
    private UserDto? _userResponse;
    private IEnumerable<UserDto>? _usersListResponse;
    private Exception? _exception;

    public UserSteps()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMapper>();
        });
        _mapper = config.CreateMapper();
        
        _userValidationService = new UserValidationService(_mockUserRepository.Object);
    }

    #region Given Steps

    [Given(@"que eu tenho os dados de um novo usuário válido")]
    public void DadoQueEuTenhoOsDadosDeUmNovoUsuarioValido(Table table)
    {
        var data = table.Rows[0];
        _createUserCommand = new CreateUserCommand(
            Email: data["Valor"],
            Name: table.Rows[1]["Valor"],
            Cpf: table.Rows[2]["Valor"],
            Password: table.Rows[3]["Valor"],
            Birthday: DateOnly.Parse(table.Rows[4]["Valor"]),
            UserType: int.Parse(table.Rows[5]["Valor"])
        );

        _mockUserRepository
            .Setup(x => x.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.Create(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken ct) => 
            {
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                return user;
            });

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que já existe um usuário com email ""(.*)""")]
    public void DadoQueJaExisteUmUsuarioComEmail(string email)
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Usuário Existente",
            Cpf = "11144477735",
            Password = "HashedPassword123",
            Birthday = new DateOnly(1990, 1, 1),
            UserType = UserType.CUSTOMER,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Email")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
    }

    [Given(@"eu tento criar um novo usuário com os dados")]
    public void DadoEuTentoCriarUmNovoUsuarioComOsDados(Table table)
    {
        var data = table.Rows[0];
        _createUserCommand = new CreateUserCommand(
            Email: data["Valor"],
            Name: table.Rows[1]["Valor"],
            Cpf: table.Rows[2]["Valor"],
            Password: table.Rows[3]["Valor"],
            Birthday: DateOnly.Parse(table.Rows[4]["Valor"]),
            UserType: int.Parse(table.Rows[5]["Valor"])
        );

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que já existe um usuário com CPF ""(.*)""")]
    public void DadoQueJaExisteUmUsuarioComCPF(string cpf)
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "usuario.existente@example.com",
            Name = "Usuário Existente",
            Cpf = cpf,
            Password = "HashedPassword123",
            Birthday = new DateOnly(1990, 1, 1),
            UserType = UserType.CUSTOMER,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Cpf")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
    }

    [Given(@"que existe um usuário com ID ""(.*)""")]
    public void DadoQueExisteUmUsuarioComID(string userIdStr)
    {
        _userId = Guid.Parse(userIdStr);
        
        var existingUser = new User
        {
            Id = _userId,
            Email = "joao.silva@example.com",
            Name = "João da Silva",
            Cpf = "11144477735",
            Password = "HashedPassword123",
            Birthday = new DateOnly(1990, 5, 15),
            UserType = UserType.CUSTOMER,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRepository
            .Setup(x => x.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockUserRepository
            .Setup(x => x.Update(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken ct) => user);

        _mockUserRepository
            .Setup(x => x.Delete(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _getUserByIdHandler = new GetUserByIdHandler(_mockUserRepository.Object, _mapper);
        _updateUserHandler = new UpdateUserHandler(_mockUserRepository.Object, _mapper, _userValidationService);
        _deleteUserHandler = new DeleteUserHandler(_mockUserRepository.Object);
    }

    [Given(@"que existe um usuário com email ""(.*)""")]
    public void DadoQueExisteUmUsuarioComEmail(string email)
    {
        _userEmail = email;
        
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Maria Santos",
            Cpf = "98765432100",
            Password = "HashedPassword456",
            Birthday = new DateOnly(1995, 8, 20),
            UserType = UserType.CUSTOMER,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Email")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _getUserByEmailHandler = new GetUserByEmailHandler(_mockUserRepository.Object, _mapper);
    }

    [Given(@"que existe um usuário com CPF ""(.*)""")]
    public void DadoQueExisteUmUsuarioComCPF(string cpf)
    {
        _userCpf = cpf;
        
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "carlos.pereira@example.com",
            Name = "Carlos Pereira",
            Cpf = cpf,
            Password = "HashedPassword789",
            Birthday = new DateOnly(1985, 3, 10),
            UserType = UserType.ADMIN,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Cpf")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _getUserByCpfHandler = new GetUserByCpfHandler(_mockUserRepository.Object, _mapper);
    }

    [Given(@"que existem (.*) usuários cadastrados no sistema")]
    public void DadoQueExistemUsuariosCadastradosNoSistema(int count)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = $"usuario{i}@example.com",
                Name = $"Usuário {i}",
                Cpf = $"1234567890{i}",
                Password = $"HashedPassword{i}",
                Birthday = new DateOnly(1990 + i, 1, 1),
                UserType = UserType.CUSTOMER,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        _mockUserRepository
            .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _getAllUsersHandler = new GetAllUsersdHandler(_mockUserRepository.Object, _mapper);
    }

    [Given(@"eu atualizo o nome para ""(.*)""")]
    public void DadoEuAtualizoONomePara(string newName)
    {
        _updateUserCommand = new UpdateUserCommand(
            Id: _userId,
            Email: "joao.silva@example.com",
            Name: newName,
            Cpf: "11144477735",
            Password: "SenhaForte@123",
            Birthday: new DateOnly(1990, 5, 15),
            UserType: 1
        );

        var existingUserForUpdate = new User
        {
            Id = _userId,
            Email = "joao.silva@example.com",
            Name = "João da Silva",
            Cpf = "11144477735",
            Password = "HashedPassword123",
            Birthday = new DateOnly(1990, 5, 15),
            UserType = UserType.CUSTOMER,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Mock para buscar o usuário existente por ID
        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Id")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUserForUpdate);

        // Mock para validar unicidade de email/CPF (retorna null = não existe outro)
        _mockUserRepository
            .Setup(x => x.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(
                expr => expr.ToString().Contains("Email") || expr.ToString().Contains("Cpf")), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
    }

    [Given(@"que eu tento criar um usuário com CPF inválido ""(.*)""")]
    public void DadoQueEuTentoCriarUmUsuarioComCPFInvalido(string cpfInvalido)
    {
        _createUserCommand = new CreateUserCommand(
            Email: "teste@example.com",
            Name: "Teste",
            Cpf: cpfInvalido,
            Password: "SenhaForte@123",
            Birthday: new DateOnly(1990, 1, 1),
            UserType: 1
        );

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que eu tento criar um usuário com email inválido ""(.*)""")]
    public void DadoQueEuTentoCriarUmUsuarioComEmailInvalido(string emailInvalido)
    {
        _createUserCommand = new CreateUserCommand(
            Email: emailInvalido,
            Name: "Teste",
            Cpf: "11144477735",
            Password: "SenhaForte@123",
            Birthday: new DateOnly(1990, 1, 1),
            UserType: 1
        );

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que eu tento criar um usuário com senha fraca ""(.*)""")]
    public void DadoQueEuTentoCriarUmUsuarioComSenhaFraca(string senhaFraca)
    {
        _createUserCommand = new CreateUserCommand(
            Email: "teste@example.com",
            Name: "Teste",
            Cpf: "11144477735",
            Password: senhaFraca,
            Birthday: new DateOnly(1990, 1, 1),
            UserType: 1
        );

        _mockUserRepository
            .Setup(x => x.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que eu tento criar um usuário com data de nascimento futura ""(.*)""")]
    public void DadoQueEuTentoCriarUmUsuarioComDataDeNascimentoFutura(string dataFutura)
    {
        _createUserCommand = new CreateUserCommand(
            Email: "teste@example.com",
            Name: "Teste",
            Cpf: "11144477735",
            Password: "SenhaForte@123",
            Birthday: DateOnly.Parse(dataFutura),
            UserType: 1
        );

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    [Given(@"que eu tento criar um usuário com data de nascimento ""(.*)""")]
    public void DadoQueEuTentoCriarUmUsuarioComDataDeNascimento(string dataNascimento)
    {
        _createUserCommand = new CreateUserCommand(
            Email: "teste@example.com",
            Name: "Teste",
            Cpf: "11144477735",
            Password: "SenhaForte@123",
            Birthday: DateOnly.Parse(dataNascimento),
            UserType: 1
        );

        _createUserHandler = new CreateUserHandler(
            _mockUserRepository.Object,
            _mapper,
            _userValidationService
        );
    }

    #endregion

    #region When Steps

    [When(@"eu solicito a criação do usuário")]
    public async Task QuandoEuSolicitoACriacaoDoUsuario()
    {
        try
        {
            var validator = new CreateUserValidator();
            var validationResult = await validator.ValidateAsync(_createUserCommand!);
            
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            _userResponse = await _createUserHandler.Handle(_createUserCommand!, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu busco o usuário por ID")]
    public async Task QuandoEuBuscoOUsuarioPorID()
    {
        try
        {
            var query = new GetUserByIdQuery(_userId);
            _userResponse = await _getUserByIdHandler.Handle(query, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu busco o usuário por email")]
    public async Task QuandoEuBuscoOUsuarioPorEmail()
    {
        try
        {
            var query = new GetUserByEmailQuery(_userEmail);
            _userResponse = await _getUserByEmailHandler.Handle(query, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu busco o usuário por CPF")]
    public async Task QuandoEuBuscoOUsuarioPorCPF()
    {
        try
        {
            var query = new GetUserByCpfQuery(_userCpf);
            _userResponse = await _getUserByCpfHandler.Handle(query, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu solicito a listagem de todos os usuários")]
    public async Task QuandoEuSolicitoAListagemDeTodosOsUsuarios()
    {
        try
        {
            var query = new GetAllUsersQuery();
            _usersListResponse = await _getAllUsersHandler.Handle(query, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu solicito a atualização do usuário")]
    public async Task QuandoEuSolicitoAAtualizacaoDoUsuario()
    {
        try
        {
            _userResponse = await _updateUserHandler.Handle(_updateUserCommand!, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [When(@"eu solicito a exclusão do usuário")]
    public async Task QuandoEuSolicitoAExclusaoDoUsuario()
    {
        try
        {
            _deleteUserCommand = new DeleteUserCommand(_userId);
            await _deleteUserHandler.Handle(_deleteUserCommand, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    #endregion

    #region Then Steps

    [Then(@"o sistema deve criar o usuário com sucesso")]
    public void EntaoOSistemaDeveCriarOUsuarioComSucesso()
    {
        _exception.Should().BeNull();
        _userResponse.Should().NotBeNull();
    }

    [Then(@"o usuário deve ter um ID gerado")]
    public void EntaoOUsuarioDeveTerUmIDGerado()
    {
        _userResponse!.Id.Should().NotBeEmpty();
    }

    [Then(@"a senha deve estar criptografada")]
    public void EntaoASenhaDeveEstarCriptografada()
    {
        _userResponse!.Password.Should().NotBe("SenhaForte@123");
        _userResponse.Password.Should().NotBeNullOrEmpty();
    }

    [Then(@"o email deve ser ""(.*)""")]
    public void EntaoOEmailDeveSer(string expectedEmail)
    {
        _userResponse!.Email.Should().Be(expectedEmail);
    }

    [Then(@"o sistema deve retornar um erro de email duplicado")]
    public void EntaoOSistemaDeveRetornarUmErroDeEmailDuplicado()
    {
        _exception.Should().NotBeNull();
        _exception!.Message.Should().Contain("email");
    }

    [Then(@"a mensagem deve conter ""(.*)""")]
    public void EntaoAMensagemDeveConter(string expectedMessage)
    {
        _exception.Should().NotBeNull();
        _exception!.Message.Should().Contain(expectedMessage);
    }

    [Then(@"o sistema deve retornar um erro de CPF duplicado")]
    public void EntaoOSistemaDeveRetornarUmErroDeCPFDuplicado()
    {
        _exception.Should().NotBeNull();
        _exception!.Message.Should().Contain("CPF");
    }

    [Then(@"o sistema deve retornar os dados do usuário")]
    public void EntaoOSistemaDeveRetornarOsDadosDoUsuario()
    {
        _exception.Should().BeNull();
        _userResponse.Should().NotBeNull();
    }

    [Then(@"o nome do usuário deve ser ""(.*)""")]
    public void EntaoONomeDoUsuarioDeveSer(string expectedName)
    {
        _userResponse!.Name.Should().Be(expectedName);
    }

    [Then(@"o email do usuário deve ser ""(.*)""")]
    public void EntaoOEmailDoUsuarioDeveSer(string expectedEmail)
    {
        _userResponse!.Email.Should().Be(expectedEmail);
    }

    [Then(@"o sistema deve retornar (.*) usuários")]
    public void EntaoOSistemaDeveRetornarUsuarios(int expectedCount)
    {
        _exception.Should().BeNull();
        _usersListResponse.Should().NotBeNull();
        _usersListResponse.Should().HaveCount(expectedCount);
    }

    [Then(@"todos devem ter ID, nome e email válidos")]
    public void EntaoTodosDevemTerIDNomeEEmailValidos()
    {
        foreach (var user in _usersListResponse!)
        {
            user.Id.Should().NotBeEmpty();
            user.Name.Should().NotBeNullOrEmpty();
            user.Email.Should().NotBeNullOrEmpty();
        }
    }

    [Then(@"o sistema deve atualizar o usuário com sucesso")]
    public void EntaoOSistemaDeveAtualizarOUsuarioComSucesso()
    {
        _exception.Should().BeNull();
        _userResponse.Should().NotBeNull();
    }

    [Then(@"o sistema deve deletar o usuário com sucesso")]
    public void EntaoOSistemaDeveDeletarOUsuarioComSucesso()
    {
        _exception.Should().BeNull();
        _mockUserRepository.Verify(x => x.Delete(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"o sistema deve retornar um erro de validação")]
    public void EntaoOSistemaDeveRetornarUmErroDeValidacao()
    {
        _exception.Should().NotBeNull();
        _exception.Should().BeOfType<FluentValidation.ValidationException>();
    }

    [Then(@"a mensagem deve indicar CPF inválido")]
    public void EntaoAMensagemDeveIndicarCPFInvalido()
    {
        var validationException = _exception as FluentValidation.ValidationException;
        validationException.Should().NotBeNull();
        validationException!.Errors.Should().Contain(e => e.PropertyName == "Cpf");
    }

    [Then(@"a mensagem deve indicar email inválido")]
    public void EntaoAMensagemDeveIndicarEmailInvalido()
    {
        var validationException = _exception as FluentValidation.ValidationException;
        validationException.Should().NotBeNull();
        validationException!.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Then(@"a mensagem deve indicar senha fraca")]
    public void EntaoAMensagemDeveIndicarSenhaFraca()
    {
        var validationException = _exception as FluentValidation.ValidationException;
        validationException.Should().NotBeNull();
        validationException!.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Then(@"a mensagem deve indicar data de nascimento inválida")]
    public void EntaoAMensagemDeveIndicarDataDeNascimentoInvalida()
    {
        var validationException = _exception as FluentValidation.ValidationException;
        validationException.Should().NotBeNull();
        validationException!.Errors.Should().Contain(e => e.PropertyName == "Birthday");
    }

    [Then(@"a mensagem deve indicar que o usuário deve ser maior de idade")]
    public void EntaoAMensagemDeveIndicarQueOUsuarioDeveSerMaiorDeIdade()
    {
        var validationException = _exception as FluentValidation.ValidationException;
        validationException.Should().NotBeNull();
        validationException!.Errors.Should().Contain(e => e.PropertyName == "Birthday");
    }

    #endregion
}