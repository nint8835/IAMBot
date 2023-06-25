namespace IAMBot.Commands

open System.Collections.Generic
open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.Entities
open DSharpPlus.SlashCommands
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

type ActionCommands() =
    inherit ApplicationCommandModule()

    member val ReferenceFile: ReferenceFile = [||] with get, set

    [<SlashCommand("action", "Gets details for a given IAM action")>]
    member public this.Action
        (ctx: InteractionContext)
        ([<Autocomplete(typeof<PrefixAutocompleteProvider>); Option("prefix", "Prefix of the AWS service")>] prefix:
            string)
        =
        task {
            do!
                ctx.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    DiscordInteractionResponseBuilder().WithContent(prefix)
                )
        }
