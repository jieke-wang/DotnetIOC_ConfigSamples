﻿using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetIOC_ConfigSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("AppSetting.json", false, true);
            IConfiguration configuration = configurationBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(IConfiguration), configuration);
            serviceCollection.AddScoped<A>();
            serviceCollection.AddScoped<B>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            B b = serviceProvider.GetRequiredService<B>();
            //b.Show();
            //b.Show2();
            b.Show3();

            Console.WriteLine("Hello World!");
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
