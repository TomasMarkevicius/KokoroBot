﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;

namespace KokoroBot
{
    class Program
    {

        static Channel currentChannel;
        static Random rng = new Random();
        static bool mute = false;
        static void Main(string[] args)
        {
            loadFiles();
            var client = new DiscordClient();

            //Display all log messages in the console
            client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            //Echo back any message received, provided it didn't come from the bot itself
            client.MessageCreated += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    currentChannel = e.Channel;
                    if(e.Member.Name == "part")
                    {
                        if(e.Message.Text == "-mute")
                        {
                            mute = !mute;
                            await client.SendMessage(currentChannel, "KokoroBot is now mute: " + mute.ToString());
                        }
                        else if(e.Message.Text == "-save")
                        {
                            saveFiles();
                            await client.SendMessage(currentChannel, "I have saved everything :3");
                        }
                    }
                    if (!mute)
                    {
                        switch (e.Message.Text)
                        {
                            case "-waifu":
                                await client.SendMessage(currentChannel, "KokoroBot is your waifu now.");
                                break;
                            case "-brainpower":
                                await client.SendMessage(currentChannel, "Huehuehue.");
                                await client.SendMessage(currentChannel, "You know...");
                                await client.SendMessage(currentChannel, @">youtube https://www.youtube.com/watch?v=0bOV4ExHPZY");
                                break;
                            case "-praise":
                                await client.SendMessage(currentChannel, "ALL PRAISE KARD (/O.o)/");
                                break;
                            case "-kardfacts":
                                await client.SendMessage(currentChannel, kardFacts());
                                break;
                            default:
                                break;
                        }
                    }
                }
            };

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            client.Run(async () =>
            {
                //Connect to the Discord server using our email and password
                await client.Connect(Sensitive.email, Sensitive.passwd);
                //If we are not a member of any server, use our invite code (made beforehand in the official Discord Client)
                if (!client.Servers.Any())
                    await client.AcceptInvite("");
            });

        }

        static string kardFacts()
        {
            return kardFactsStrings[rng.Next(0, kardFactsStrings.Length)];
        }

        static string[] kardFactsStrings;

        static void saveFiles()
        {
            File.WriteAllLines("kardfacts.txt", kardFactsStrings);
        }

        static void loadFiles()
        {
            kardFactsStrings = File.ReadAllLines("kardfacts.txt");
        }
    }
}
