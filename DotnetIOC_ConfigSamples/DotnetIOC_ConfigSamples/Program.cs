using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetIOC_ConfigSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSetting.json", false, true);
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> {
                { "Memory:User:Name", "jack" },
                { "Memory:User:Age", "18" },
            });
            
            // 命令行支持的命令格式如下:
            // key=value
            // --key=value 或 --key value
            // /key=value 或 /key value
            // PS:等号格式与空格格式不能混用
            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IConfiguration), configuration);
            serviceCollection.AddScoped<A>();
            serviceCollection.AddScoped<B>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            //B b = serviceProvider.GetRequiredService<B>();
            //b.Show();
            //b.Show2();
            //b.Show3();

            Show(serviceProvider);

            Console.WriteLine("Hello World!");
        }

        static void Show(IServiceProvider serviceProvider)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string[] connectionStrings = configuration.GetSection("ConnectionStrings").Get<string[]>();
            Console.WriteLine(string.Join(", ", connectionStrings));
            Console.WriteLine(configuration["a:b:c"]);

            Console.WriteLine(configuration["Memory:User:Name"]);
            Console.WriteLine(configuration["Memory:User:Age"]);

            Console.WriteLine(configuration["CommandLineA"]);
            Console.WriteLine(configuration["CommandLineB"]);
            Console.WriteLine(configuration["CommandLineC"]);
            Console.WriteLine(configuration["CommandLineA:B"]);

            Console.WriteLine(configuration["EnvA"]);
            Console.WriteLine(configuration["EnvB:B"]);
            Console.WriteLine(configuration["env_file"]);
        }
    }

    public class A
    { }

    public class B
    {
        private readonly A _a;
        private readonly IConfiguration _configuration;

        public B(A a, IConfiguration configuration)
        {
            this._a = a ?? throw new ArgumentNullException(nameof(a));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Show()
        {
            string key = "ConnectionStrings";
            Console.WriteLine(this._configuration[key]);

            string[] a = this._configuration.GetSection(key).Get<string[]>();
            List<string> b = new List<string>();
            this._configuration.Bind(key, b);

            var c = this._configuration.GetValue<List<string>>(key);
            var d = this._configuration.Get<List<string>>();

            IEnumerable<IConfigurationSection> childrenConfigs = this._configuration.GetChildren();
            foreach (var childrenConfig in childrenConfigs)
            {
                Console.WriteLine(childrenConfig.Key);
                Console.WriteLine(childrenConfig.Path);
                Console.WriteLine(childrenConfig.Value);

                if (childrenConfig.Exists())
                {
                    foreach (var childrenConfig2 in childrenConfig.GetChildren())
                    {
                        Console.WriteLine(childrenConfig2.Key);
                        Console.WriteLine(childrenConfig2.Path);
                        Console.WriteLine(childrenConfig2.Value);

                        if (childrenConfig2.Exists())
                        {
                            foreach (var childrenConfig3 in childrenConfig2.GetChildren())
                            {
                                Console.WriteLine(childrenConfig3.Key);
                                Console.WriteLine(childrenConfig3.Path);
                                Console.WriteLine(childrenConfig3.Value);
                            }
                        }
                    }
                }
            }
        }

        public void Show2()
        {
            Console.WriteLine(this._configuration["ConnectionStrings"]);
            Console.WriteLine(this._configuration["ConnectionStrings:0"]);
            Console.WriteLine(this._configuration["ConnectionStrings:1"]);
            Console.WriteLine(this._configuration["a"]);
            Console.WriteLine(this._configuration["a:b"]);
            Console.WriteLine(this._configuration["a:b:c"]);
        }

        public void Show3()
        {
            Rootobject instance = new Rootobject();
            this._configuration.Bind(instance);
        }
    }


    public class Rootobject
    {
        public string[] ConnectionStrings { get; set; }
        public A1 a { get; set; }
    }

    public class A1
    {
        public B1 b { get; set; }
    }

    public class B1
    {
        public string c { get; set; }
    }

}

// webBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "dll文件");
// webBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "");
