using FluentAssertions;
using NetArchTest.Rules;

namespace Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Timesheet";
        private const string ApplicationNamespace = "Timesheet.Application";
        private const string InfrastructureNamespace = "Timesheet.Infrastructure.Persistence";
        private const string PresentationNamespace = "Timesheet.Presentation";
        private const string WebNamespace = "Timesheet.Web.Api";

        [Fact]
        public void Domain_Should_Not_Have_Dependency_On_Other_Projects()
        {
            var assembly = typeof(Timesheet.Domain.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                PresentationNamespace,
                WebNamespace
            };

            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_Have_Dependency_On_Other_Projects()
        {
            var assembly = typeof(Timesheet.Application.AssemblyReference).Assembly;

            var otherProjects = new[]
            {
                InfrastructureNamespace,
                PresentationNamespace,
                WebNamespace
            };

            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            testResult.IsSuccessful.Should().BeTrue();
        }

        //[Fact]
        //public void Presentation_Should_Not_Have_Dependency_On_Other_Projects()
        //{
        //    var assembly = typeof(Timesheet.Web.Presentation.AssemblyReference).Assembly;

        //    var otherProjects = new[]
        //    {
        //        InfrastructureNamespace,
        //        WebNamespace
        //    };

        //    var testResult = Types
        //        .InAssembly(assembly)
        //        .ShouldNot()
        //        .HaveDependencyOnAll(otherProjects)
        //        .GetResult();

        //    testResult.IsSuccessful.Should().BeTrue();
        //}
    }
}