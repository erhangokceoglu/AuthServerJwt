using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MiniApp1.Api.ClaimsRequirements
{
    public class BirthDateRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }

        public BirthDateRequirement(int age)
        {
            Age = age;
        }

        public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
            {
                var birthDate = context.User.FindFirst("BirthDate");
                if (birthDate == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                var today = DateTime.Now;
                var age = today.Year - Convert.ToDateTime(birthDate.Value).Year;

                if (age <= requirement.Age)
                {
                    context.Fail();
                    return Task.CompletedTask;

                }
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
    }
}
