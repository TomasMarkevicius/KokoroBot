using System;
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
        static bool restart = false;
        static void Main(string[] args)
        {
            if (!restart)
            {
                loadFiles();
            }
            else
            {
                restart = false;
            }
            {
                var client = new DiscordClient();
                //Display all log messages in the console
                client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

                //Echo back any message received, provided it didn't come from the bot itself
                client.MessageCreated += async (s, e) =>
                {
                    Console.WriteLine(e.Message.User.Name + ": " + e.Message.Text);
                    if (!e.Message.IsAuthor)
                    {
                        currentChannel = e.Channel;
                        if (e.Member.UserId == "95543627391959040")
                        {
                            if (e.Message.Text == "-mute")
                            {
                                mute = !mute;
                                await client.SendMessage(currentChannel, "KokoroBot is now mute: " + mute.ToString());
                            }
                            else if (e.Message.Text == "-save")
                            {
                                saveFiles();
                                await client.SendMessage(currentChannel, "I have saved everything :3");
                            }
                            else if (e.Message.Text == "-dc")
                            {
                                await client.Disconnect();
                            }
                        }
                        else if (e.Member.Name == "part")
                        {
                            await client.SendMessage(currentChannel, "I don't like you. B-b-baka. >.<");
                            restart = true;
                            await client.Disconnect();
                        }
                        if (!mute)
                        {
                            if (e.Message.Text.Length > 0)
                            {
                                string[] splitmessage = e.Message.Text.Split(' ');
                                if (splitmessage[0] == "-kardfacts")
                                {
                                    if (splitmessage.Length > 2)
                                    {
                                        if (splitmessage[1] == "add")
                                        {
                                            try
                                            {
                                                string finalstr = "";
                                                for (int i = 2; i < splitmessage.Length; i++)
                                                {
                                                    if (i != 2)
                                                        finalstr += ' ' + splitmessage[i];
                                                    else
                                                        finalstr = splitmessage[i];
                                                }
                                                if (finalstr.Length > 5)
                                                {
                                                    kardFactsStrings.Add(finalstr);
                                                    await client.SendMessage(currentChannel, "A new fact about Kard has been added. (Yay ^-^):");
                                                    await client.SendMessage(currentChannel, finalstr);
                                                }
                                                else
                                                {
                                                    throw new IOException("Hue.");
                                                }
                                            }
                                            catch (Exception except)
                                            {
                                                await client.SendMessage(currentChannel, "That hurt <.< Don't do this again, ok? :3");
                                            }

                                        }
                                    }
                                    else
                                    {
                                        await client.SendMessage(currentChannel, kardFacts());
                                    }
                                }
                            }

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
                                case "-getclientid":
                                    await client.SendMessage(currentChannel, e.Message.UserId);
                                    break;
                                case "-part":
                                    await client.SendMessage(currentChannel, "part is the baka who created this bot. Fun fact: a");
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
            if (!restart)
            { 
            saveFiles();
            }
            else
            {
                Main(new string[] { });
            }
        }

        static string kardFacts()
        {
            return kardFactsStrings[rng.Next(0, kardFactsStrings.Count)];
        }

        static List<string> kardFactsStrings;

        static void saveFiles()
        {
            File.WriteAllLines("kardfacts.txt", kardFactsStrings.ToArray());
        }

        static void loadFiles()
        {
            kardFactsStrings = new List<string>(File.ReadAllLines("kardfacts.txt"));
        }
    }
}
