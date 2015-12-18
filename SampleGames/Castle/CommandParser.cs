using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle
{

    internal enum CommandVerb
    {
        Error,
        Look,
        Take,
        Drop,
        Open,
        Inventory,
        Drink,
        Wear,
        Read,
        Wave,
        Show,
        Play,
        Quit,
        Room,
        Warp,
        Point
        

    }


    internal class Command
    {
        public CommandVerb Verb { get; private set; }
        public String Subject { get; private set; }

        public Command(CommandVerb verb, String subject)
        {
            this.Verb = verb;
            this.Subject = subject;
        }

        public static Command Parse(string userStatement)
        {
            Command newCommand = null;
            Char[] split = new Char[1];
            split[0] = ' ';
            String[] commands = userStatement.Split(split);
            CommandVerb verb = ParseVerb(commands[0]);
            if (commands.Length < 2)
            {
                newCommand = new Command(verb, String.Empty);
            }
            else
            {
                newCommand = new Command(verb, commands[1]);
            }
            return newCommand;
            

        }
        private static CommandVerb ParseVerb(string userStatment)
        {
            CommandVerb verb = CommandVerb.Error;
            if(userStatment.StartsWith("look", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Look;
            }
            else if (userStatment.StartsWith("take", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Take;
            }
            else if (userStatment.StartsWith("get", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Take;
            }
            else if (userStatment.StartsWith("drop", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Drop;
            }
            else if (userStatment.StartsWith("open", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Open;
            }
            else if (userStatment.StartsWith("inv", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Inventory;
            }
            else if (userStatment.StartsWith("drink", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Drink;
            }
            else if (userStatment.StartsWith("wear", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Wear;
            }
            else if (userStatment.StartsWith("read", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Read;
            }
            else if (userStatment.StartsWith("room", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Room;
            }
            else if (userStatment.StartsWith("warp", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Warp;
            }
            else if (userStatment.StartsWith("point", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Point;
            }
            else if (userStatment.StartsWith("wave", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Wave;
            }
            else if (userStatment.StartsWith("quit", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Quit;
            }
            else if (userStatment.StartsWith("show", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Show;
            }
            else if (userStatment.StartsWith("play", StringComparison.CurrentCultureIgnoreCase))
            {
                verb = CommandVerb.Play;
            }
            return verb;
        }

    }

}

