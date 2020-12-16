using System;
using System.Runtime.InteropServices;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime.DotNetCore;

namespace AirQualityCollector
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

            try
            {
                HostFactory.Run(hostConfig =>
                {
                    // to fix Topshelf issue in linux
                    //https://github.com/Topshelf/Topshelf/issues/513
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        hostConfig.UseEnvironmentBuilder(new EnvironmentBuilderFactory(c => {
                            return new DotNetCoreEnvironmentBuilder(c);
                        }));
                    }

                    hostConfig.Service<WindowsService>(serivceConfig =>
                    {
                        serivceConfig.ConstructUsing(() => new WindowsService());
                        serivceConfig.WhenStarted(s => s.Start());
                        serivceConfig.WhenStopped(s => s.Stop());
                    });
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Service start error. {e}");
                throw;
            }
        }
	}
}
