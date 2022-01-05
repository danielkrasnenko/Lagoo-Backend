using FluentValidation.Results;
using Lagoo.BusinessLogic.Common.Exceptions.Base;
using Microsoft.AspNetCore.Identity;

namespace Lagoo.BusinessLogic.Common.Exceptions.Api;

/// <summary>
///   An Exception gets thrown from a middleware with 400 status code
/// </summary>
public class BadRequestException : BaseException
{
    private const string DefaultErrorMessage = "One or more validation rules were not passed";
    
    public BadRequestException() : base(DefaultErrorMessage)
    {
        Failures = new Dictionary<string, string[]>();
    }

    public BadRequestException(IEnumerable<ValidationFailure> validationFailures) : this()
    {
        var groupedFailures = validationFailures.GroupBy(vf => vf.PropertyName, vf => vf.ErrorMessage);
        
        foreach (var failure in groupedFailures)
        {
            var propertyName = failure.Key;
            var propertyFailures = failure.ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }

    public BadRequestException(IdentityResult identityResult) : this()
    {
        Failures = identityResult.Errors
            .GroupBy(ir => ir.Code)
            .ToDictionary(g => g.Key, g => g.Select(ie => ie.Description).ToArray());
    }

    public BadRequestException(params string[] errors) : this()
    {
        Failures.Add(string.Empty, errors);
    }

    public IDictionary<string, string[]> Failures { get; }

    public bool ShowErrorCodes { get; set; }
}