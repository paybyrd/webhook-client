﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Paybyrd.Clients.Webhook.Abstractions;
using Paybyrd.Clients.Webhook.Tests.Fixtures;

namespace Paybyrd.Clients.Webhook.Tests;

public class WebhookClientTests : IClassFixture<ServiceProviderFixture>, IClassFixture<AuthorizationFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;
    private readonly AuthorizationFixture _authorizationFixture;

    public WebhookClientTests(ServiceProviderFixture serviceProviderFixture, AuthorizationFixture authorizationFixture)
    {
        _serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));
        _authorizationFixture = authorizationFixture ?? throw new ArgumentNullException(nameof(authorizationFixture));
    }

    [Fact]
    public async Task GivenWebhookClient_WhenQuerySettings_ShouldReturnAList()
    {
        await using var sp =_serviceProviderFixture.BuildServiceProvider();
        var client = sp.GetRequiredService<IWebhookClient>();
        var querySettings = Substitute.For<IQueryWebhookSettings>();
        var settings = await client.Settings(_authorizationFixture.Authorization).QueryAsync(querySettings);
        settings.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenWebhookClient_WhenCreateSettings_ShouldReturnTheNewSettings()
    {
        await using var sp = _serviceProviderFixture.BuildServiceProvider();
        var client = sp.GetRequiredService<IWebhookClient>();
        var createWebhookSettings = Substitute.For<ICreateWebhookSettings>();
        createWebhookSettings.Events.Returns(new[] { "order.paid" });
        createWebhookSettings.PaymentMethods.Returns(new[] { "card" });
        createWebhookSettings.Url.Returns("https://webapp-stub-api-v2-stg-ktzwiryfzguhy.azurewebsites.net/api/v1/hooks");
        createWebhookSettings.Username.Returns(Guid.NewGuid().ToString());
        createWebhookSettings.Password.Returns(Guid.NewGuid().ToString());
        var settings = await client.Settings(_authorizationFixture.Authorization).CreateAsync(createWebhookSettings);
        settings.Should().NotBeNull();

        settings.Id.Should().NotBeNull();
        settings.Url.Should().Be(createWebhookSettings.Url);
        settings.Username.Should().Be(createWebhookSettings.Username);
        settings.Password.Should().Be(createWebhookSettings.Password);
        settings.Events.Should().BeEquivalentTo(createWebhookSettings.Events);
        settings.PaymentMethods.Should().BeEquivalentTo(createWebhookSettings.PaymentMethods);
    }

    [Fact]
    public async Task GivenWebhookClient_WhenQueryWebhooks_ShouldReturnAList()
    {
        await using var sp = _serviceProviderFixture.BuildServiceProvider();
        var client = sp.GetRequiredService<IWebhookClient>();
        var queryWebhooks = Substitute.For<IQueryWebhooks>();
        var webhooks = await client.Webhooks(_authorizationFixture.Authorization).QueryAsync(queryWebhooks);
        webhooks.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenWebhookClient_WhenQueryWebhookAttempts_ShouldReturnAList()
    {
        await using var sp = _serviceProviderFixture.BuildServiceProvider();
        var client = sp.GetRequiredService<IWebhookClient>();
        var queryWebhookAttempts = Substitute.For<IQueryWebhookAttempts>();
        queryWebhookAttempts.WebhookId.Returns("384f80b6-5339-4706-856b-57d95cbf2d20");
        var webhookAttempts = await client.Webhooks(_authorizationFixture.Authorization).QueryAsync(queryWebhookAttempts);
        webhookAttempts.Should().NotBeEmpty();
    }

}
