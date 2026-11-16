using DSharpPlus;
using DSharpPlus.Commands;
using System.Text;
using Newtonsoft.Json;
using Cardagin.Config;
using Cardagin.Games;
using DSharpPlus.Entities;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using Cardagin.Games.Commands;
using Microsoft.Extensions.Logging;

namespace Cardagin
{
    public class Bot
    {
        public static DiscordClient? Client { get; private set; }
        public CommandsExtension? Commands { get; private set; }
        public static ILogger Logger { get; private set; }
        public static Dictionary<DiscordChannel, Game> ActiveGames = new Dictionary<DiscordChannel, Game>();
        public static readonly string configPath = "C:\\Projects\\Cardagain\\Config\\config.json";
        public static readonly string gameAliasPath = "C:\\Projects\\Cardagain\\Config\\games.json";
        public static Dictionary<string, GameType> gameAliases { get; private set; } = new();

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead(configPath))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            if (string.IsNullOrWhiteSpace(configJson.Token))
            {
                Console.WriteLine("[Error] No discord token found. Please provide a token via the Token object in the config.json file.");
                Environment.Exit(1);
            }

            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(configJson.Token, SlashCommandProcessor.RequiredIntents);

            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                //Add the group commands here
                extension.AddCommands([typeof(BlackjackCommands), typeof(GameCommands)]);
                SlashCommandProcessor commandProcessor = new(new SlashCommandConfiguration()
                {
                    NamingPolicy = new KebabCaseNamingPolicy()
                });

                extension.AddProcessor(commandProcessor);
            });

            Client = builder.Build();
            Logger = Client.Logger;

            json = string.Empty;
            using (var fs = File.OpenRead(gameAliasPath))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            var gameAliasesJson = JsonConvert.DeserializeObject<GameAliasesJson>(json);
            foreach (GamesAliasJson game in gameAliasesJson.Aliases)
            {
                foreach (string alias in game.Alias)
                {
                    if (Enum.TryParse<GameType>(game.Game, out GameType res))
                    {
                        gameAliases.Add(alias, res);
                    } else {
                        Console.Error.WriteLine($"Failed to parse {game.Game} into valid GameType Enum");
                    }
                } 
            }

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
