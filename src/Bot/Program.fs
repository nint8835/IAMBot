open System.Threading.Tasks
open Discord
open Discord.WebSocket
open dotenv.net
open Microsoft.Extensions.Configuration

let Log (msg: LogMessage) : Task =
    printfn $"%s{msg.ToString()}"
    Task.CompletedTask

type Configuration = { DiscordToken: string }

DotEnv.Load()

let config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("IAM_")
        .Build()
        .Get<Configuration>()

let socketConfig = DiscordSocketConfig()
socketConfig.GatewayIntents <- GatewayIntents.AllUnprivileged ||| GatewayIntents.MessageContent

let client = new DiscordSocketClient(socketConfig)
client.add_Log Log

client.add_MessageReceived (fun msg ->
    printfn $"%s{msg.Content}"
    Task.CompletedTask)

client.LoginAsync(TokenType.Bot, config.DiscordToken)
|> Async.AwaitTask
|> Async.RunSynchronously

client.StartAsync() |> Async.AwaitTask |> Async.RunSynchronously

Task.Delay -1 |> Async.AwaitTask |> Async.RunSynchronously
