using Ddap.Core;
using FluentAssertions;
using Xunit;

namespace Ddap.Tests.Core;

public class AutoReloadOptionsTests
{
    [Fact]
    public void AutoReloadOptions_Should_Have_Default_Values()
    {
        // Arrange & Act
        var options = new AutoReloadOptions();

        // Assert
        options.Enabled.Should().BeFalse();
        options.IdleTimeout.Should().Be(TimeSpan.FromMinutes(5));
        options.Strategy.Should().Be(ReloadStrategy.InvalidateAndRebuild);
        options.Behavior.Should().Be(ReloadBehavior.ServeOldSchema);
        options.ChangeDetection.Should().Be(ChangeDetection.CheckHash);
        options.OnBeforeReloadAsync.Should().BeNull();
        options.OnAfterReloadAsync.Should().BeNull();
        options.OnReloadErrorAsync.Should().BeNull();
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_Enabled()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.Enabled = true;

        // Assert
        options.Enabled.Should().BeTrue();
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_IdleTimeout()
    {
        // Arrange
        var options = new AutoReloadOptions();
        var timeout = TimeSpan.FromMinutes(10);

        // Act
        options.IdleTimeout = timeout;

        // Assert
        options.IdleTimeout.Should().Be(timeout);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_Strategy()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.Strategy = ReloadStrategy.HotReload;

        // Assert
        options.Strategy.Should().Be(ReloadStrategy.HotReload);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_Strategy_To_RestartExecutor()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.Strategy = ReloadStrategy.RestartExecutor;

        // Assert
        options.Strategy.Should().Be(ReloadStrategy.RestartExecutor);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_Behavior()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.Behavior = ReloadBehavior.BlockRequests;

        // Assert
        options.Behavior.Should().Be(ReloadBehavior.BlockRequests);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_Behavior_To_QueueRequests()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.Behavior = ReloadBehavior.QueueRequests;

        // Assert
        options.Behavior.Should().Be(ReloadBehavior.QueueRequests);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_ChangeDetection()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.ChangeDetection = ChangeDetection.AlwaysReload;

        // Assert
        options.ChangeDetection.Should().Be(ChangeDetection.AlwaysReload);
    }

    [Fact]
    public void AutoReloadOptions_Should_Allow_Setting_ChangeDetection_To_CheckTimestamps()
    {
        // Arrange
        var options = new AutoReloadOptions();

        // Act
        options.ChangeDetection = ChangeDetection.CheckTimestamps;

        // Assert
        options.ChangeDetection.Should().Be(ChangeDetection.CheckTimestamps);
    }

    [Fact]
    public async Task AutoReloadOptions_Should_Allow_Setting_OnBeforeReloadAsync()
    {
        // Arrange
        var options = new AutoReloadOptions();
        var callbackCalled = false;
        Func<IServiceProvider, Task<bool>> callback = async (sp) =>
        {
            callbackCalled = true;
            await Task.CompletedTask;
            return true;
        };

        // Act
        options.OnBeforeReloadAsync = callback;
        var result = await options.OnBeforeReloadAsync(null!);

        // Assert
        options.OnBeforeReloadAsync.Should().NotBeNull();
        callbackCalled.Should().BeTrue();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AutoReloadOptions_Should_Allow_Setting_OnAfterReloadAsync()
    {
        // Arrange
        var options = new AutoReloadOptions();
        var callbackCalled = false;
        Func<IServiceProvider, Task> callback = async (sp) =>
        {
            callbackCalled = true;
            await Task.CompletedTask;
        };

        // Act
        options.OnAfterReloadAsync = callback;
        await options.OnAfterReloadAsync(null!);

        // Assert
        options.OnAfterReloadAsync.Should().NotBeNull();
        callbackCalled.Should().BeTrue();
    }

    [Fact]
    public async Task AutoReloadOptions_Should_Allow_Setting_OnReloadErrorAsync()
    {
        // Arrange
        var options = new AutoReloadOptions();
        var callbackCalled = false;
        Func<IServiceProvider, Exception, Task> callback = async (sp, ex) =>
        {
            callbackCalled = true;
            await Task.CompletedTask;
        };

        // Act
        options.OnReloadErrorAsync = callback;
        await options.OnReloadErrorAsync(null!, new Exception("test"));

        // Assert
        options.OnReloadErrorAsync.Should().NotBeNull();
        callbackCalled.Should().BeTrue();
    }

    [Fact]
    public void ReloadStrategy_Should_Have_All_Values()
    {
        // Arrange & Act & Assert
        Enum.IsDefined(typeof(ReloadStrategy), ReloadStrategy.InvalidateAndRebuild)
            .Should()
            .BeTrue();
        Enum.IsDefined(typeof(ReloadStrategy), ReloadStrategy.HotReload).Should().BeTrue();
        Enum.IsDefined(typeof(ReloadStrategy), ReloadStrategy.RestartExecutor).Should().BeTrue();
    }

    [Fact]
    public void ReloadBehavior_Should_Have_All_Values()
    {
        // Arrange & Act & Assert
        Enum.IsDefined(typeof(ReloadBehavior), ReloadBehavior.ServeOldSchema).Should().BeTrue();
        Enum.IsDefined(typeof(ReloadBehavior), ReloadBehavior.BlockRequests).Should().BeTrue();
        Enum.IsDefined(typeof(ReloadBehavior), ReloadBehavior.QueueRequests).Should().BeTrue();
    }

    [Fact]
    public void ChangeDetection_Should_Have_All_Values()
    {
        // Arrange & Act & Assert
        Enum.IsDefined(typeof(ChangeDetection), ChangeDetection.AlwaysReload).Should().BeTrue();
        Enum.IsDefined(typeof(ChangeDetection), ChangeDetection.CheckHash).Should().BeTrue();
        Enum.IsDefined(typeof(ChangeDetection), ChangeDetection.CheckTimestamps).Should().BeTrue();
    }
}
