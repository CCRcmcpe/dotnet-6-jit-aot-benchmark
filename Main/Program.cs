using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CoreRt;

ManualConfig config = DefaultConfig.Instance
    .AddJob(Job.Default
        .AsBaseline().WithId(".NET 6"))
    .AddJob(Job.Default
        .WithEnvironmentVariables(new EnvironmentVariable("DOTNET_TieredPGO", "1"))
        .WithRuntime(CoreRuntime.Core60).WithId(".NET 6 Dynamic PGO"))
    .AddJob(Job.Default
        .WithEnvironmentVariables(new EnvironmentVariable("DOTNET_TieredPGO", "1"),
            new EnvironmentVariable("DOTNET_TC_QuickJitForLoops", "1"),
            new EnvironmentVariable("DOTNET_ReadyToRun", "0"))
        .WithRuntime(CoreRuntime.Core60).WithId(".NET 6 Full PGO"))
    .AddJob(Job.Default
        .WithRuntime(CoreRtRuntime.CoreRt60)
        .WithToolchain(CoreRtToolchain.CreateBuilder()
            .UseCoreRtNuGet()
            .ToToolchain()).WithId(".NET 6 NativeAOT"))
    .AddExporter(HtmlExporter.Default);

BenchmarkRunner.Run(typeof(BinaryTrees), config, args);