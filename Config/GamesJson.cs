using Newtonsoft.Json;

namespace Cardagin.Config
{
    struct GameAliasesJson
    {
        [JsonProperty("Aliases")]
        public GamesAliasJson[] Aliases { get; private set; }
    }

    struct GamesAliasJson
    {
        [JsonProperty("game")]
        public string Game { get; private set; }
        [JsonProperty("alias")]
        public string[] Alias { get; private set; }
    }
}
