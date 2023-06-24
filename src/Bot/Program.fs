open System.Threading.Tasks
open Discord
open Discord.WebSocket
open dotenv.net
open Microsoft.Extensions.Configuration

let Log (msg: LogMessage) : Task = task { printfn $"%s{msg.ToString()}" }


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

client.add_MessageReceived (fun msg -> task { printfn $"%s{msg.Content}" })

task {
    do! client.LoginAsync(TokenType.Bot, config.DiscordToken)
    do! client.StartAsync()
    do! Task.Delay -1
}
|> Task.WaitAll
