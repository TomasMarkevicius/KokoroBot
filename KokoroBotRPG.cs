﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Discord;

namespace KokoroBot
{
    class KokoroBotRPG
    {
        Dictionary<long, KRPG_player> players;
        Random rng;
        const int CURRENT_VERSION=1;
        HashAlgorithm hshfnc = HashAlgorithm.Create("SHA");

        public KokoroBotRPG()
        {
            players = new Dictionary<long, KRPG_player>();
            rng = new Random((int)(DateTime.Now.ToFileTimeUtc() % int.MaxValue));
        }

        public void Load()
        {
            try {
                if (File.Exists("kokororpg"))
                {
                    BinaryReader br = new BinaryReader(File.OpenRead("kokororpg"));
                    int count = br.ReadInt32();
                    int vers = br.ReadInt32();
                    if (vers == 1)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            long id = br.ReadInt64();
                            float xp = br.ReadSingle();
                            int level = br.ReadInt32();
                            float hp = br.ReadSingle();
                            var plr = new KRPG_player(id);
                            plr.xp = xp;
                            plr.level = level;
                            plr.hp = hp;
                            players.Add(id, plr);
                        }
                    }

                    br.Close();
                }
            }
            catch(IOException)
            {
                Console.WriteLine("Did someone say IOException?");
            }
        }

        public void Save()
        {
            try {
                BinaryWriter bw = new BinaryWriter(File.Open("kokororpg", FileMode.OpenOrCreate));
                bw.Write(players.Count);
                bw.Write(CURRENT_VERSION);
                var player_col = players.ToArray();
                for (int i = 0; i < player_col.Length; i++)
                {
                    bw.Write(player_col[i].Value.ID);
                    bw.Write(player_col[i].Value.xp);
                    bw.Write(player_col[i].Value.level);
                    bw.Write(player_col[i].Value.hp);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Did someone say IOException?");
            }
        }

        public async Task HandleCommands(MessageEventArgs e, DiscordClient client, Channel currentChannel)
        {
            try {
                string log = "";
                string clean_message = e.Message.Text.Substring(1);
                if (!players.ContainsKey(e.User.Id))
                {
                    players.Add(e.User.Id, new KRPG_player(e.User.Id));
                }
                KRPG_player player = players[e.User.Id];
                if (clean_message.Contains("@"))
                {
                    string command = clean_message.Split('@')[0];
                    command = command.Trim(' ');
                    string target = clean_message.Split('@')[1];
                    target = target.Trim(' ');
                    var validUsers = e.Server.Members.Where((User u) =>
                   {
                       return u.Name == target;
                   });
                    var target_usr = validUsers.First();
                    if (target_usr != null)
                    {
                        if(target_usr.Status == UserStatus.Offline)
                        {
                            await client.SendMessage(currentChannel, "But nobody came.." );
                            return;
                        }
                        if (!players.ContainsKey(target_usr.Id))
                        {
                            players.Add(target_usr.Id, new KRPG_player(target_usr.Id));
                        }
                        KRPG_player target_plr;
                        if (target_usr.Id == player.ID)
                        {
                            target_plr = player;
                        }
                        else
                        {
                            target_plr = players[target_usr.Id];
                        }
                        if(command == "level")
                        {
                            log += target + " is level "+target_plr.level+" | "+target_plr.xp+'/'+ (target_plr.level * target_plr.level) + '\n';
                            await client.SendMessage(currentChannel, log);
                            return;
                        }
                        var bytes = Encoding.UTF8.GetBytes(target_usr + command);
                        byte hash_b = hshfnc.ComputeHash(bytes)[0];
                        float variation = 0.4f + 0.01f * rng.Next(60);
                        float hash = hash_b / 2;
                        float scale = 0.01f * (hash - 28);
                        float damagescale = 100.0f - (100.0f * (1.0f / ((player.level + 1))));
                        float absolutedamage = (damagescale * variation) * scale;
                        //await client.SendMessage(currentChannel, e.User.Name + ' ' + command + "s " + target + '.');
                        if (absolutedamage > 0)
                        {
                            log += "It deals " + absolutedamage.ToString() + " damage." + '\n';
                        }
                        else
                        {
                            log += "It heals for " + absolutedamage.ToString() + " damage." + '\n';
                        }
                        target_plr.hp -= absolutedamage;
                        if (target_plr.hp <= 0)
                        {
                            target_plr.hp = float.MaxValue;
                            if (target_plr != player)
                            {
                                float xpgain = target_plr.level * 2;
                                player.xp += xpgain;
                                log += target + " has been killed and resurrected. +" + xpgain + "XP" + '\n';
                            }
                            else
                            {
                                log += target + " has killed themselves and resurrected. +1 suicide attempt" + '\n';
                            }
                        }
                        else
                        {
                            log += target + " has " + target_plr.hp + " left." + '\n';
                        }
                    }
                }
                else
                {
                    // TODO: Item logic etc.
                }

                if (player.xp > player.level * player.level)
                {
                    player.xp = player.xp - (player.level * player.level);
                    player.level++;
                    log += e.User.Name + " has leveled up to Level " + player.level + '.' + '\n';
                }
                if (log != "")
                {
                    await client.SendMessage(currentChannel, log);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("welp.");
            }
            }
    }

    class KRPG_player
    {
        public long ID;
        public float xp;
        public int level;
        public float hp
        {
            get
            {
                return _hp;
            }
            set
            {
                if(value > 10.0 * Math.Pow(1.4, level))
                {
                    _hp = (float)(10.0 * Math.Pow(1.4, level));
                }
                else
                {
                    _hp = value;
                }
            }
        }
        float _hp = 0;
            
        public KRPG_player(long ID)
        {
            this.ID = ID;
            xp = 0;
            level = 1;
            hp = level * 10;
        }
    }
}
