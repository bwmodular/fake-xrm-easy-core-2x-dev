using System;
using Xunit;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy.Abstractions.FakeMessageExecutors;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using System.Reflection;
using FakeXrmEasy.Abstractions.Exceptions;

namespace FakeXrmEasy.Core.Tests.Middleware
{
    public class GenericMessageExecutorsTests
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;
        public GenericMessageExecutorsTests() 
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            _context = MiddlewareBuilder
                        .New()
       
                        // Add* -> Middleware configuration
                        .AddCrud()
                        .AddFakeMessageExecutors()
                        .AddGenericFakeMessageExecutors(currentAssembly)

                        // Use* -> Defines pipeline sequence
                        .UseCrud()
                        .UseMessages()

                        .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                        .Build();
                        
            _service = _context.GetOrganizationService();
        }

        [Fact]
        public void Should_execute_generic_message_with_explicit_middleware_fake_message_executor()
        {
            var context = MiddlewareBuilder
                        .New()
       
                        // Add* -> Middleware configuration
                        .AddCrud()
                        .AddFakeMessageExecutors()
                        .AddGenericFakeMessageExecutor("new_TestAction", new OldGenericFakeMessageExecutor())

                        // Use* -> Defines pipeline sequence
                        .UseCrud()
                        .UseMessages()

                        .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                        .Build();
                        
            var service = context.GetOrganizationService();

            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = service.Execute(request);
            Assert.Equal("testinput", response["output"]);
        }

        [Fact]
        public void Should_not_clear_previous_generic_fake_message_executors()
        {
            var context = MiddlewareBuilder
                        .New()

                        // Add* -> Middleware configuration
                        .AddCrud()

                        .AddGenericFakeMessageExecutor("new_TestAction", new OldGenericFakeMessageExecutor())
                        .AddGenericFakeMessageExecutors()

                        // Use* -> Defines pipeline sequence
                        .UseCrud()
                        .UseMessages()

                        .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                        .Build();

            var service = context.GetOrganizationService();

            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = service.Execute(request);
            Assert.Equal("testinput", response["output"]);
        }

        [Fact]
        public void Should_execute_generic_message_with_add_generic_fake_message_executors()
        {
            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = _service.Execute(request);
            Assert.Equal("testinput", response["output"]);
        }

        [Fact]
        public void Should_throw_exception_if_message_is_removed()
        {
            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = _service.Execute(request);
            Assert.Equal("testinput", response["output"]);

#pragma warning disable CS0618 // Type or member is obsolete
            (_context as XrmFakedContext).RemoveGenericFakeMessageExecutor("new_TestAction");
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Throws<OpenSourceUnsupportedException>(() => _service.Execute(request));
        }
    }

    class OldGenericFakeMessageExecutor : IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request.RequestName == "new_TestAction";
        }

        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            OrganizationResponse response = new OrganizationResponse();
            response["output"] = request["input"];
            return response;
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(OrganizationRequest);
        }
    }

    class NewGenericFakeMessageExecutor : IGenericFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request.RequestName == "new_TestAction";
        }

        public OrganizationResponse Execute(OrganizationRequest request, IXrmFakedContext ctx)
        {
            OrganizationResponse response = new OrganizationResponse();
            response["output"] = request["input"];
            return response;
        }

        public Type GetResponsibleRequestType()
        {
            return typeof(OrganizationRequest);
        }

        public string GetRequestName() 
        {
            return "new_TestAction";
        }
    }
}
