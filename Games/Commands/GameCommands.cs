using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cardagin.Games.Commands
{
    public class GameCommands : BaseCommandModule
    {
        [Command("start")]
        public async Task StartGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await ctx.Channel.SendMessageAsync("A game is already active in this channel. Please finish the current game before starting a new one.");
                return;
            }

            await ctx.Channel.SendMessageAsync("Please specify a game to start. Usage: `!start <game_name>`");
        }

        [Command("start")]
        public async Task StartGame(CommandContext ctx, string gameName)
        {

            if (!ctx.Channel.IsThread) {
                await ctx.Channel.SendMessageAsync("Please only start games in threads.");
                return;
            }

            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await ctx.Channel.SendMessageAsync("A game is already active in this channel. Please finish the current game before starting a new one.");
                return;
            }

            Game game = new EmptyGame();
            bool isRecognized = true;

            switch (gameName.ToLower())
            {
                case "blackjack":
                    game = new Blackjack();
                    break;
                case "21":
                    game = new Blackjack();
                    break;
                case "bj":
                    game = new Blackjack();
                    break;
                default:
                    await ctx.Channel.SendMessageAsync($"Game '{gameName}' not recognized.");
                    isRecognized = false;
                    return;
            }

            if (isRecognized)
            {
                game.Start(ctx.Channel);
                await ctx.Channel.SendMessageAsync($"Starting a new game of {game.Name}! For the game to begin there must be at least {game.MinPlayers} and at most {game.MaxPlayers}");
            }

            // Placeholder implementation for starting a game
        }

        [Command("end")]
        public async Task EndGame(CommandContext ctx)
        {
            Bot.ActiveGames[ctx.Channel].End();
            await ctx.Channel.SendMessageAsync("The current game has been ended.");
        }

        [Command("join")]
        public async Task JoinGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                Game game = Bot.ActiveGames[ctx.Channel];
                if (game.Players.Count >= game.MaxPlayers)
                {
                    await ctx.Channel.SendMessageAsync($"Sorry {ctx.User.Mention}, the game is already full.");
                    return;
                }

                if (game.Players.ContainsKey(ctx.User.Id))
                {
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you have already joined the game.");
                    return;
                }

                game.Players.Add(ctx.User.Id, new Sessions.UserInfo(ctx));
                await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has joined the game! {game.MaxPlayers - game.Players.Count} slot{(game.MaxPlayers - game.Players.Count == 1 ? "" : "s")} left. {(game.MaxPlayers - game.Players.Count <= 0 ? " The game is now full no more players can join!" : "")}");
            } else {
                await ctx.Channel.SendMessageAsync("There is no active game in this channel to join.");
            }
        }

        [Command("leave")]
        public async Task LeaveGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                Game game = Bot.ActiveGames[ctx.Channel];
                if (game.Players.ContainsKey(ctx.User.Id))
                {
                    game.Players.Remove(ctx.User.Id);
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has left the game. {game.Players.Count} player{(game.Players.Count == 1 ? "" : "s")} remaining.");
                } else {
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you are not part of the current game.");
                }

                return;
            }

            await ctx.Channel.SendMessageAsync("There is no active game in this channel.");
        }

        [Command("deal")]
        public async Task DealCards(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                if (Bot.ActiveGames[ctx.Channel].Players.Count < Bot.ActiveGames[ctx.Channel].MinPlayers)
                {
                    await ctx.Channel.SendMessageAsync("Cannot deal cards as there are not enough players in the game.");
                    return;
                } else if (Bot.ActiveGames[ctx.Channel].Players.Count > Bot.ActiveGames[ctx.Channel].MaxPlayers)  {
                    await ctx.Channel.SendMessageAsync("Cannot deal cards as there are too many players in the game.");
                    return;
                }

                    Bot.ActiveGames[ctx.Channel].Deal();
            } else {
                await ctx.Channel.SendMessageAsync("There is no active game in this channel to deal cards for.");
            }
        }
    }
}