using System;
using System.Collections.Generic;
using WinConStartup.RunLogic;
using WinConStartup.RunLogic.Security;

namespace WinConStartup
{
    // ReSharper disable once ArrangeTypeModifiers
	class Program
	{
		private const string Configfile = "udat.dat";
		private static string asUser = "default";
		private static Dictionary<string, string> _ColorDictionary = new Dictionary<string, string>()
		{
			{"OKGREEN", "\x033[95m"},
			{"FAIL", "\x033[91m"},
			{"WARNING", "\x033[93m"},
			{"ENDC", "\x033[0m"}
		};

		private static readonly Passwd _pass = new Passwd(Configfile);
		private static readonly CustomLoader _loader = new CustomLoader(Configfile);
		private static readonly RunnerPool _pool = new RunnerPool(true);
		static void Main(string[] args)
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].StartsWith("-"))
                {
                    if (args.Length > 2 + i && args[i].Equals("-adduser"))
                    {
                        ++i;
                        string user = args[i];
                        ++i;
                        string passwd = args[i];
                        _pass.CreateUser(user, passwd);
                    }
	                if (args.Length > 1 + i && (args[i].Equals("-user") || args[i].Equals("-asuser")))
	                {
		                ++i;
		                asUser = args[i];
	                }
                }
                else
                {
                    RunItem item = _loader.LoadRunItem(args[i]);
                    if (item != null)
                    {
                        _pool.Add(item);
                    }
                }
            }
			if (RequestUserPassword())
			{
				Console.WriteLine("CHARMAP [ {0}COLORIZER{1} ] TEST {0}RUN{1} !", _ColorDictionary["OKGREEN"],
					_ColorDictionary["ENDC"]);
				Console.ReadLine();
			}
			else
			{
				Console.WriteLine("NONE");
				Console.ReadLine();
			}
            _pool.RunPool();
        }

	    private static bool RequestUserPassword()
	    {
			Console.WriteLine("Program Starter v{0}. Copyright \u00A9 {1}", "0.1.0.01", "Peer Bölts");
		    Console.Write("Password: ");
		    string userInput = Console.ReadLine();
		    return _pass.ComparePass(asUser, userInput);
	    }
    }
}
