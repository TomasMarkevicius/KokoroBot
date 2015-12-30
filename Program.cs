using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace KokoroBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new DiscordClient();

            //Display all log messages in the console
            client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            //Echo back any message received, provided it didn't come from the bot itself
            client.MessageCreated += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                    await client.SendMessage(e.Channel, e.Message.Text);
            };

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            client.Run(async () =>
            {
                //Connect to the Discord server using our email and password
                await client.Connect(Sensitive.email, Sensitive.passwd);

                //If we are not a member of any server, use our invite code (made beforehand in the official Discord Client)
                if (!client.Servers.Any())
                    await client.AcceptInvite("0jfsaSe0IrlSaeuH");
            });
        }
    }
}
