using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;



namespace SimpleDemo
{
    class Program
    {
       
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                string version = GL.GetString(StringName.Version);
                Console.WriteLine(version);
                if (version.StartsWith("4.")) game.Run(0); //game.Run(60, 60);
                else Debug.WriteLine("Requested OpenGL version not available.");
            }

        }
    }
}
