open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.Entities
open DSharpPlus.SlashCommands
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

    [<SlashCommand("test", "Testing slash command")>]
    member public this.Test(ctx: InteractionContext) =
        task {
            do!
                ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    DiscordInteractionResponseBuilder().WithContent("Test")
                )
        }

let client =
    new DiscordClient(
        DiscordConfiguration(
            Token = config.DiscordToken,
            TokenType = TokenType.Bot,
            Intents = (DiscordIntents.AllUnprivileged ||| DiscordIntents.MessageContents)
        )
    )

let slash = client.UseSlashCommands()
slash.RegisterCommands<CommandTest>(config.GuildId)

client.add_MessageCreated (fun _ evt -> task { printfn $"{evt.Message.Content}" })

task {
    do! client.ConnectAsync()
    do! Task.Delay -1
}
|> Task.WaitAll
