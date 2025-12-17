using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Crypto;

namespace ConsoleApp
{
    internal class Program
    {
        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_init(int buildNumber);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_create(string hostname, int port, out IntPtr instance);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_login();

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]

        public static extern int aw_enter( string world );

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_term();

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_sdk_build();

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_state_change();

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_string(int attribute, string value);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_string_set(int attribute, string value);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_int(int attribute );

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_int_set(int attribute, int value);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_bool_set(int attribute, int value);

        [DllImport("ClassicSdk.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int aw_wait(int milliseconds);

        const int AW_LOGIN_NAME = 0;
        const int AW_LOGIN_PASSWORD = 1;
        const int AW_LOGIN_OWNER = 2;
        const int AW_LOGIN_PRIVILEGE_PASSWORD = 3;
        const int AW_LOGIN_PRIVILEGE_NUMBER = 4;
        const int AW_LOGIN_PRIVILEGE_NAME = 5;
        const int AW_LOGIN_APPLICATION = 6;
        const int AW_LOGIN_EMAIL = 7;

        const int AW_MY_X = 198;
        const int AW_MY_Y = 199;
        const int AW_MY_Z = 200;
        const int AW_MY_YAW = 201;
        const int AW_MY_PITCH = 202;
        const int AW_MY_TYPE = 203;
        const int AW_MY_GESTURE = 204;
        const int AW_MY_STATE = 205;
        const int AW_ENTER_GLOBAL = 328;



        static void Main(string[] args)
        {
            Console.WriteLine("Yert");

        // 🔑 Install crypto hooks BEFORE aw_init
        BouncyCastleHooks.Install();

            int rc = aw_init(101);
            Console.WriteLine( rc );

            IntPtr instance;
            rc = aw_create("auth.deltaworlds.com", 6671, out instance);

            if( rc != 0 )
            {
                Console.WriteLine("aw_create FAILED rc = " + rc);
                System.Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("aw_create SUCCESS");
            }

            aw_int_set(AW_LOGIN_OWNER, 250);
            aw_string_set(AW_LOGIN_PRIVILEGE_PASSWORD, "455669");
            aw_string_set(AW_LOGIN_APPLICATION, "SomeTestApp");
            aw_string_set(AW_LOGIN_NAME, "ThisIsATestBot");

            rc = aw_login();

            if( rc != 0 )
            {
                Console.WriteLine("aw_login failed rc = " + rc);
                System.Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("aw_login SUCCESS");
            }

            // Note: in the sdk bools are ints of 0 or 1
            aw_bool_set(AW_ENTER_GLOBAL, 0); // set to 1 for global (requires CT access).

            rc = aw_enter("BotDev");

            if (rc != 0)
            {
                Console.WriteLine("aw_enter FAILED rc " + rc);
                System.Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("aw_enter SUCCESS");
            }
            
            // set position and avatar
            aw_int_set(AW_MY_X, 0);
            aw_int_set(AW_MY_Y, 0);
            aw_int_set(AW_MY_Z, 0);
            aw_int_set(AW_MY_YAW, 0);
            aw_int_set(AW_MY_TYPE, 0);

            rc = aw_state_change();

            if ( rc != 0 )
            {
                Console.WriteLine("aw_state_change FAILED rc " + rc);
                System.Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("aw_state_change SUCCESS");
            }

            while ( true )
            {
                aw_wait(100);
            }
        }
    }
}
