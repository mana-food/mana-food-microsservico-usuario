using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using Xunit;
using ManaFood.Application.Shared;

namespace ManaFood.Tests.Shared;

public class ValidationBehaviorTests
{
    private class TestRequest : IRequest<string> { public string Value { get; set; } = string.Empty; }
    private class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator() { RuleFor(x => x.Value).NotEmpty(); }
    }

    [Fact]
    public async Task Should_Invoke_Next_When_No_Validators()
    {
        var behavior = new ValidationBehavior<TestRequest, string>(Enumerable.Empty<IValidator<TestRequest>>());
        var nextCalled = false;
        var next = new RequestHandlerDelegate<string>(() => { nextCalled = true; return Task.FromResult("ok"); });
        var result = await behavior.Handle(new TestRequest { Value = "abc" }, next, CancellationToken.None);
        nextCalled.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Should_Throw_When_Validation_Fails()
    {
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var next = new RequestHandlerDelegate<string>(() => Task.FromResult("ok"));
        var act = async () => await behavior.Handle(new TestRequest { Value = "" }, next, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Should_Invoke_Next_When_Validation_Passes()
    {
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var next = new RequestHandlerDelegate<string>(() => Task.FromResult("ok"));
        var result = await behavior.Handle(new TestRequest { Value = "valid" }, next, CancellationToken.None);
        result.Should().Be("ok");
    }
}
