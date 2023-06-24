open dotenv.net
open Microsoft.Extensions.Configuration

type Configuration = { DiscordToken: string }

DotEnv.Load()

let config =
    ConfigurationBuilder()
        .AddEnvironmentVariables("IAM_")
        .Build()
        .Get<Configuration>()

printf $"{config.DiscordToken}"
