open System.Threading.Tasks
open Discord
open Discord.Interactions
open Discord.WebSocket
open Microsoft.Extensions.DependencyInjection
open dotenv.net
open Microsoft.Extensions.Configuration

let Log (msg: LogMessage) : Task = task { printfn $"%s{msg.ToString()}" }


type Configuration =
    { DiscordToken: string
      GuildId: uint64 }

DotEnv.Load()

let config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("IAM_")
        .Build()
        .Get<Configuration>()

type public CommandTest() =
    inherit InteractionModuleBase<InteractionContext>()

    [<SlashCommand("test", "Get a test response")>]
    member public this.Test() =
        task { do! this.Context.Interaction.RespondAsync "Pong!" }


let socketConfig = DiscordSocketConfig()
socketConfig.GatewayIntents <- GatewayIntents.AllUnprivileged ||| GatewayIntents.MessageContent

let serviceProvider =
    ServiceCollection()
        .AddSingleton(socketConfig)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton<InteractionService>()
        .BuildServiceProvider()

let client = serviceProvider.GetService<DiscordSocketClient>()
let interactionService = serviceProvider.GetService<InteractionService>()

client.add_Log Log
client.add_MessageReceived (fun msg -> task { printfn $"%s{msg.Content}" })

client.add_Ready (fun _ ->
    task {
        let! commands = interactionService.RegisterCommandsToGuildAsync(config.GuildId, true)
        printfn $"{commands}"

        printfn "Commands registered"
    })

client.add_InteractionCreated (fun interaction ->
    task {
        let ctx = SocketInteractionContext(client, interaction)
        interactionService.ExecuteCommandAsync(ctx, serviceProvider) |> Task.WaitAll
    })

task {
    do! client.LoginAsync(TokenType.Bot, config.DiscordToken)

    let! result = interactionService.AddModuleAsync<CommandTest>(serviceProvider)
    printfn $"%A{result.SlashCommands}"

    do! client.StartAsync()
    do! Task.Delay -1
}
|> Task.WaitAll
