using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Text;
using Newtonsoft.Json;
using Cardagin.Config;
using Cardagin.Games;
using DSharpPlus.Entities;

namespace Cardagin
{
    public class Bot
    {
        public DiscordClient? Client { get; private set; }
        public CommandsNextExtension? Commands { get; private set; }
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

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.MessageContents | DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.Guilds
            };

            Client = new DiscordClient(config);

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = true,
                DmHelp = true,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Games.Commands.GameCommands>();
            Commands.RegisterCommands<Games.Commands.BlackjackCommands>();

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
