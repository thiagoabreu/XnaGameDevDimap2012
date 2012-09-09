using System;
using System.Collections.Generic;
using System.Linq;

namespace P2PTank
{
	#region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (PeerToPeerGame game = new PeerToPeerGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}

