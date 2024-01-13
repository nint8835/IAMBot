namespace IAMBot.Commands

open DSharpPlus
open DSharpPlus.Entities
open DSharpPlus.SlashCommands
open System.Collections.Generic
open System.Threading.Tasks

open IAMBot.IAMReference

type PrefixAutocompleteProvider() =

    member this.Provider(ctx: AutocompleteContext) : Task<IEnumerable<DiscordAutoCompleteChoice>> =
        let enteredText = (string ctx.OptionValue).ToLower()

        task {
            return
                ctx.Services.GetService(typeof<ReferenceFile>) :?> ReferenceFile
                |> Array.filter (fun service ->
                    enteredText.Length = 0
                    || service.Prefix.Contains enteredText
                    || service.ServiceName.ToLower().Contains enteredText)
                |> Array.map (fun service -> DiscordAutoCompleteChoice(service.Prefix, service.Prefix))
                |> Array.truncate 10
                |> Array.toSeq
        }

type ActionAutocompleteProvider() =

    member this.Provider(ctx: AutocompleteContext) : Task<IEnumerable<DiscordAutoCompleteChoice>> =
        let enteredText = (string ctx.OptionValue).ToLower()

        let prefixOption = ctx.Options |> Seq.tryFind (fun option -> option.Name = "prefix")

        task {
            return
                ctx.Services.GetService(typeof<ReferenceFile>) :?> ReferenceFile
                |> Array.collect (fun service -> service.Actions |> Array.map (fun action -> (service, action)))
                |> Array.filter (fun (service, _) ->
                    match prefixOption with
                    | None -> true
                    | Some prefixOption -> service.Prefix = (string prefixOption.Value).ToLower())
                |> Array.filter (fun (_, action) ->
                    enteredText.Length = 0 || action.Action.ToLower().Contains enteredText)
                |> Array.map snd
                |> Array.map (fun action -> action.Action)
                |> Array.distinct
                |> Array.map (fun action -> DiscordAutoCompleteChoice(action, action))
                |> Array.truncate 10
                |> Array.toSeq
        }

type ActionCommands() =
    inherit ApplicationCommandModule()

    member val ReferenceFile: ReferenceFile = [||] with get, set

    [<SlashCommand("action", "Gets details for a given IAM action")>]
    member public this.Action
        (ctx: InteractionContext)
        ([<Autocomplete(typeof<PrefixAutocompleteProvider>); Option("prefix", "Prefix of the AWS service")>] prefix:
            string)
        ([<Autocomplete(typeof<ActionAutocompleteProvider>);
           Option("action", "Name of the action, without the service prefix")>] actionName: string)
        =
        let service =
            this.ReferenceFile |> Array.tryFind (fun service -> service.Prefix = prefix)

        let action =
            match service with
            | None -> None
            | Some service -> service.Actions |> Array.tryFind (fun action -> action.Action = actionName)

        let resp =
            match action with
            | None -> DiscordEmbedBuilder().WithColor(DiscordColor.Red).WithTitle("Action not found")
            | Some action ->
                DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Aquamarine)
                    .WithTitle($"`{service.Value.Prefix}:{action.Action}`")
                    .WithDescription(action.Description)
                    .AddField("Service", service.Value.ServiceName)


        task {
            do!
                ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    DiscordInteractionResponseBuilder().AddEmbed(resp)
                )
        }
