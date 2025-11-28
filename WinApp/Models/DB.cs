using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    using BsonData;
    public static class DB
    {
        static public MainDatabase Main { get; private set; }
        static public void Start(string path)
        {
            Main = new MainDatabase("MainDB");
            Main.Connect(path);
        }
        static public void End()
        {
            Main.Disconnect();
        }

        static public Collection Accounts => Main.GetCollection("accounts");
    }
}
