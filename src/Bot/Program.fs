open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.Entities
open DSharpPlus.SlashCommands
open Microsoft.Extensions.DependencyInjection
open dotenv.net
open Microsoft.Extensions.Configuration

type Configuration =
    { DiscordToken: string
      GuildId: uint64 }

DotEnv.Load()

let config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("IAM_")
        .Build()
        .Get<Configuration>()


type CommandTest() =
    inherit ApplicationCommandModule()

    member val Configuration: Configuration = { DiscordToken = ""; GuildId = 0UL } with get, set


    [<SlashCommand("test", "Testing slash command")>]
    member public this.Test(ctx: InteractionContext) =
        task {
            do!
                ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    DiscordInteractionResponseBuilder()
                        .WithContent(this.Configuration.GuildId.ToString())
                )
        }

let serviceProvider =
    ServiceCollection().AddSingleton<Configuration>(config).BuildServiceProvider()

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

slash.RegisterCommands<CommandTest>(config.GuildId)

client.add_MessageCreated (fun _ evt -> task { printfn $"{evt.Message.Content}" })

task {
    do! client.ConnectAsync()
    do! Task.Delay -1
}
|> Task.WaitAll
