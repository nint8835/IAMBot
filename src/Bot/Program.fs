open DSharpPlus
open DSharpPlus.SlashCommands
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open System.Threading.Tasks
open dotenv.net

open IAMBot.Commands
open IAMBot.IAMReference

type Configuration =
    { DiscordToken: string
      GuildId: uint64 }


[<EntryPoint>]
let main (_: string[]) =
    DotEnv.Load()

    let config =
        ConfigurationBuilder()
            .AddEnvironmentVariables("IAM_")
            .Build()
            .Get<Configuration>()


    let reference = Reference.Load() |> Async.RunSynchronously

    let serviceProvider =
        ServiceCollection()
            .AddSingleton<Configuration>(config)
            .AddSingleton<ReferenceFile>(reference)
            .BuildServiceProvider()

    let client =
        new DiscordClient(DiscordConfiguration(Token = config.DiscordToken, TokenType = TokenType.Bot))

    let slash =
        client.UseSlashCommands(SlashCommandsConfiguration(Services = serviceProvider))

    slash.RegisterCommands<ActionCommands>(config.GuildId)

    task {
        do! client.ConnectAsync()
        do! Task.Delay -1
    }
    |> Task.WaitAll

    0
