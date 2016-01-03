using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace KokoroBot
{
    class Tired
    {
        static string Tillerino(string userID)
        {
            switch (userID)
            {
                case "131884818274320384":      //Kard
                    return "Tsunamaru - Daidai Genome [Insane] HDDT future you: 329pp  95%: 262pp | 98%: 309pp | 99%: 335pp | 100%: 370pp | 2:00 ★ 6.01 ♫ 202.5 AR9.67";
                    break;
                case "95543627391959040":       //Part
                    return "Panda Eyes & Teminite - Highscore [_part's madness] FLDT future you: Kardshark's praiser pp 95%: 344pp | 98%: 405pp | 99%: 489pp | 100%: 522pp | 2:00 ★ 7.25 ♫ 110 AR10.33";
                    break;
                case "95568415057522688":       //Tironas
                    return "Reol - Asymmetry [cRyo[Skystar]'s Farewell] future you: 50pp  95%: 304pp | 98%: 340pp | 99%: 363pp | 100%: 395pp | 4:11 ★ 6.43 ♫ 184 AR9.5";
                    break;
                default:
                    int _index = Program.rng.Next(randomTillerinoTexts.Length);
                    return randomTillerinoTexts[_index];
                    break;
            }
        }

        static string[] randomTillerinoTexts = new string[]
        {
            "DragonForce - Cry Thunder [Legend] HDHR future you: -10pp  95%: 817pp | 98%: 887pp | 99%: 929pp | 100%: 984pp | 5:11 ★ 8.5 ♫ 130 AR10",
            "Panda Eyes & Teminite - Highscore [Tironas[Fixing]] future you: 210pp 95%: 180pp | 98%: 207pp | 99%: 230pp | 100%: 270pp | 2:00 ★ 5.84 ♫ 110 AR10"
        };

        internal async static Task handleCommands(MessageEventArgs e, DiscordClient client, Channel currentChannel)
        {
            switch(e.Message.Text)
            {
                case "-touhou":
                    await client.SendMessage(currentChannel, "is THAT another TOUHOU map???");
                    break;
                case "!r":
                    await client.SendMessage(currentChannel, Tillerino(e.Message.User.Id.ToString()));
                    break;
                default:
                    break;
            }
        }
    }
}
