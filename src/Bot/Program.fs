open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.SlashCommands
open IAMBot.Commands
open Microsoft.Extensions.DependencyInjection
open dotenv.net
open Microsoft.Extensions.Configuration
open IAMBot.IAMReference

type Configuration =
    { DiscordToken: string
      GuildId: uint64 }

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
    new DiscordClient(
        DiscordConfiguration(
            Token = config.DiscordToken,
            TokenType = TokenType.Bot,
            Intents = (DiscordIntents.AllUnprivileged ||| DiscordIntents.MessageContents)
        )
    )

let slash =
    client.UseSlashCommands(SlashCommandsConfiguration(Services = serviceProvider))

slash.RegisterCommands<ActionCommands>(config.GuildId)

client.add_MessageCreated (fun _ evt -> task { printfn $"{evt.Message.Content}" })

task {
    do! client.ConnectAsync()
    do! Task.Delay -1
}
|> Task.WaitAll
