using DSharpPlus;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeamBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string fileName = "config.json";
            ConfigJson config;

            try
            {
                using FileStream openStream = File.OpenRead(fileName);
                config = await JsonSerializer.DeserializeAsync<ConfigJson>(openStream, options);
            }
            catch (FileNotFoundException)
            {
                await CreateConfig(fileName);

                await Task.Delay(15000);
                return;
            }

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        static async Task CreateConfig(string fileName)
        {
            Console.WriteLine("Config file not found! Auto generated new config at:\n" + Directory.GetCurrentDirectory() + "\\" + fileName);

            var config = new ConfigJson() { Token = "[Your token here]" };
            var options = new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            using FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, config, options);
        }
    }

    public class ConfigJson
    {
        public string Token { get; set; }
    }
}
